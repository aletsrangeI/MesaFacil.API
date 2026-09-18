using System.Net.Http.Json;
using Domain.Entities;
using DTO.Sync;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.BackgroundServices;

/// <summary>
/// Spec 019 & Spec 030: Motor de sincronización Edge-Cloud y mantenimiento local en segundo plano.
/// 
/// Comportamiento:
/// 1. Detección de conectividad contra el servidor central ('Sync:CloudBaseUrl') vía health check.
/// 2. Si hay conexión: transmite lotes de OutboxEvents mediante POST /api/sync/push-events
///    y marca como Synced los aceptados por el servidor.
/// 3. Si NO hay conexión (corte de ISP / offline): mantiene los eventos en Pending y opera de forma
///    autónoma en la red LAN sin interrumpir comandas ni cobros.
/// 4. Si 'Sync:CloudBaseUrl' está vacío (nodo único local/desarrollo): auto-sincroniza localmente.
/// 5. Mantenimiento SQLite automático: ejecuta VACUUM INTO una vez al día cuando corre bajo SQLite.
/// </summary>
public class CloudSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CloudSyncWorker> _logger;
    private readonly HttpClient _httpClient;

    private static readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(30);
    private DateTime _lastDailyBackupDate = DateTime.MinValue;

    public CloudSyncWorker(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<CloudSyncWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;

        var timeoutSec = _configuration.GetValue<int>("Sync:PingTimeoutSeconds", 5);
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(Math.Max(2, timeoutSec))
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _configuration.GetValue<int>("Sync:IntervalSeconds", 30);
        var interval = TimeSpan.FromSeconds(Math.Max(5, intervalSeconds));

        _logger.LogInformation("CloudSyncWorker iniciado. Ciclo de sincronización cada {Seconds}s.", interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingEventsAsync(stoppingToken);
                await PerformDailyMaintenanceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloudSyncWorker: error durante el ciclo de sincronización/mantenimiento.");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Apagado normal del host
            }
        }
    }

    private async Task ProcessPendingEventsAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var batchSize = _configuration.GetValue<int>("Sync:BatchSize", 100);
        var cloudBaseUrl = _configuration["Sync:CloudBaseUrl"]?.Trim().TrimEnd('/');

        var pendientes = await context.OutboxEvents
            .Where(e => e.SyncStatus == OutboxSyncStatus.Pending)
            .OrderBy(e => e.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);

        if (pendientes.Count == 0) return;

        // Caso A: Modo Nodo Único / Sin servidor central configurado
        if (string.IsNullOrEmpty(cloudBaseUrl))
        {
            foreach (var evento in pendientes)
            {
                evento.SyncStatus = OutboxSyncStatus.Synced;
                evento.SyncedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync(ct);
            _logger.LogInformation("CloudSyncWorker: {Count} eventos marcados como sincronizados en nodo local autónomo.", pendientes.Count);
            return;
        }

        // Caso B: Edge conectado a servidor Cloud central
        var isCloudReachable = await CheckCloudConnectivityAsync(cloudBaseUrl, ct);
        if (!isCloudReachable)
        {
            _logger.LogWarning("CloudSyncWorker: Servidor Cloud no disponible ({Url}). Operando en modo Offline LAN. {Count} eventos en cola outbox.",
                cloudBaseUrl, pendientes.Count);
            return;
        }

        // Transmitir lote de eventos hacia POST /api/sync/push-events
        var requestDto = new PushEventsRequestDTO
        {
            OrigenNodo = Environment.MachineName,
            Events = pendientes.Select(p => new SyncEventDTO
            {
                Id = p.Id,
                AggregateType = p.AggregateType,
                AggregateId = p.AggregateId,
                EventType = p.EventType,
                PayloadJson = p.PayloadJson,
                CreatedAt = p.CreatedAt
            }).ToList()
        };

        try
        {
            var targetUrl = $"{cloudBaseUrl}/api/sync/push-events";
            var response = await _httpClient.PostAsJsonAsync(targetUrl, requestDto, ct);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<Common.Response<PushEventsResultDTO>>(cancellationToken: ct);
                var aceptadosSet = responseBody?.Data?.IdsAceptados?.ToHashSet() ?? new HashSet<Guid>();
                var duplicadosSet = responseBody?.Data?.IdsDuplicados?.ToHashSet() ?? new HashSet<Guid>();

                foreach (var evento in pendientes)
                {
                    if (aceptadosSet.Contains(evento.Id) || duplicadosSet.Contains(evento.Id))
                    {
                        evento.SyncStatus = OutboxSyncStatus.Synced;
                        evento.SyncedAt = DateTime.UtcNow;
                    }
                }

                await context.SaveChangesAsync(ct);
                _logger.LogInformation("CloudSyncWorker: {Count} eventos sincronizados exitosamente con servidor Cloud ({Url}).", pendientes.Count, cloudBaseUrl);
            }
            else
            {
                _logger.LogWarning("CloudSyncWorker: Servidor Cloud respondió HTTP {StatusCode} al enviar lote de sincronización.", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CloudSyncWorker: Error al transmitir lote de eventos hacia {Url}. Se reintentará en el siguiente ciclo.", cloudBaseUrl);
        }
    }

    private async Task<bool> CheckCloudConnectivityAsync(string cloudBaseUrl, CancellationToken ct)
    {
        try
        {
            var healthUrl = $"{cloudBaseUrl}/api/health";
            using var req = new HttpRequestMessage(HttpMethod.Get, healthUrl);
            using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task PerformDailyMaintenanceAsync(CancellationToken ct)
    {
        // Solo ejecuta una vez cada 24 horas
        if (DateTime.UtcNow.Date <= _lastDailyBackupDate)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var isSqlite = context.Database.ProviderName?.Contains("Sqlite") == true;
        if (!isSqlite) return;

        try
        {
            var backupsDir = Path.Combine(AppContext.BaseDirectory, "backups");
            if (!Directory.Exists(backupsDir))
            {
                Directory.CreateDirectory(backupsDir);
            }

            var fileName = $"backup_auto_{DateTime.UtcNow:yyyyMMdd}.db";
            var fullPath = Path.Combine(backupsDir, fileName).Replace('\\', '/');

            if (!File.Exists(fullPath))
            {
                await context.Database.ExecuteSqlRawAsync($"VACUUM INTO '{fullPath}'", ct);
                _logger.LogInformation("CloudSyncWorker: Respaldo diario SQLite generado automáticamente en {Path}.", fullPath);
            }

            _lastDailyBackupDate = DateTime.UtcNow.Date;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CloudSyncWorker: No se pudo completar el respaldo diario SQLite.");
        }
    }

    public override void Dispose()
    {
        _httpClient.Dispose();
        base.Dispose();
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.BackgroundServices;

/// <summary>
/// Spec 019 (Fase 3): motor de sincronización en segundo plano.
///
/// LIMITACIÓN CONOCIDA / FUERA DE ALCANCE: en un despliegue real Edge-Cloud, este worker
/// corre DENTRO del Edge Node de la sucursal y envía sus OutboxEvents pendientes al Cloud vía
/// POST /api/sync/push-events con reintentos exponenciales. En este entorno de un solo nodo
/// (esta misma instancia de MesaFacil.API funciona como "Cloud" y no existe un Edge Node físico
/// separado contra el cual sincronizar), este worker se implementa como un stub funcional:
/// lee los OutboxEvents pendientes generados localmente por el OutboxSaveChangesInterceptor y
/// los marca como sincronizados, dejando trazabilidad completa (CreatedAt/SyncedAt/RetryCount)
/// sin inventar una llamada de red hacia sí mismo. El contrato HTTP real (push-events/pull-catalogs)
/// SÍ está implementado en SyncController y es el que usaría un Edge Node real.
/// </summary>
public class CloudSyncWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CloudSyncWorker> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    public CloudSyncWorker(IServiceProvider serviceProvider, ILogger<CloudSyncWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingEventsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloudSyncWorker: error procesando OutboxEvents pendientes.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Apagado normal del host.
            }
        }
    }

    private async Task ProcessPendingEventsAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var pendientes = await context.OutboxEvents
            .Where(e => e.SyncStatus == OutboxSyncStatus.Pending)
            .OrderBy(e => e.CreatedAt)
            .Take(200)
            .ToListAsync(ct);

        if (pendientes.Count == 0) return;

        foreach (var evento in pendientes)
        {
            // Nodo de un solo servidor: no hay un Edge físico distinto al que reenviar, así que
            // el evento ya "vive" en la base consolidada. Se marca sincronizado directamente.
            evento.SyncStatus = OutboxSyncStatus.Synced;
            evento.SyncedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
        _logger.LogInformation("CloudSyncWorker: {Count} eventos marcados como sincronizados.", pendientes.Count);
    }
}

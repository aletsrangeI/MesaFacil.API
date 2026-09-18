using System.Net;
using System.Net.Sockets;
using Common;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

/// <summary>
/// Spec 030: Controlador de utilidades y monitoreo para el Edge Node Local.
/// Permite consultar el estado de conectividad/sincronización y disparar respaldos en caliente (VACUUM INTO).
/// </summary>
[ApiController]
[Route("api/edge")]
public class EdgeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EdgeController> _logger;

    public EdgeController(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<EdgeController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Consulta el estado operativo del Edge Node: perfil activo, proveedor de datos,
    /// cola outbox pendiente y direcciones IP locales en la red LAN.
    /// </summary>
    [HttpGet("status")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<DTO.Edge.EdgeStatusDTO>>> GetStatus(CancellationToken ct)
    {
        var executionProfile = _configuration["ExecutionProfile"] 
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
            ?? "Production";

        var isSqlite = _context.Database.ProviderName?.Contains("Sqlite") == true;
        var pendingCount = await _context.OutboxEvents.CountAsync(e => e.SyncStatus == OutboxSyncStatus.Pending, ct);
        var totalCount = await _context.OutboxEvents.CountAsync(ct);
        var cloudBaseUrl = _configuration["Sync:CloudBaseUrl"] ?? string.Empty;

        var localIps = GetLocalIPv4Addresses();

        var backupsDir = Path.Combine(AppContext.BaseDirectory, "backups");
        var backupFiles = Directory.Exists(backupsDir)
            ? Directory.GetFiles(backupsDir, "*.db")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTimeUtc)
                .Take(5)
                .Select(f => new DTO.Edge.EdgeBackupItemDTO { Name = f.Name, SizeBytes = f.Length, CreatedAtUtc = f.CreationTimeUtc })
                .ToList()
            : new();

        var response = new DTO.Edge.EdgeStatusDTO
        {
            Profile = executionProfile,
            DatabaseProvider = _context.Database.ProviderName ?? "Unknown",
            IsSqlite = isSqlite,
            CloudBaseUrl = cloudBaseUrl,
            Outbox = new DTO.Edge.EdgeOutboxStatusDTO
            {
                Pending = pendingCount,
                Total = totalCount
            },
            Network = new DTO.Edge.EdgeNetworkStatusDTO
            {
                MachineName = Environment.MachineName,
                LanAddresses = localIps,
                ServicePort = 5055
            },
            RecentBackups = backupFiles,
            ServerUtc = DateTime.UtcNow
        };

        return Ok(new Response<DTO.Edge.EdgeStatusDTO>
        {
            isSuccess = true,
            Message = "Estado del Edge Node obtenido correctamente.",
            Data = response
        });
    }

    /// <summary>
    /// Ejecuta un respaldo en caliente de la base de datos local SQLite mediante 'VACUUM INTO'.
    /// Solo disponible cuando el motor activo es SQLite.
    /// </summary>
    [HttpPost("backup")]
    [Authorize]
    public async Task<ActionResult<Response<DTO.Edge.EdgeBackupResultDTO>>> CreateBackup(CancellationToken ct)
    {
        var isSqlite = _context.Database.ProviderName?.Contains("Sqlite") == true;
        if (!isSqlite)
        {
            return BadRequest(new Response<DTO.Edge.EdgeBackupResultDTO>
            {
                isSuccess = false,
                Message = "El comando VACUUM INTO solo aplica a bases de datos SQLite en perfil Edge."
            });
        }

        try
        {
            var backupsDir = Path.Combine(AppContext.BaseDirectory, "backups");
            if (!Directory.Exists(backupsDir))
            {
                Directory.CreateDirectory(backupsDir);
            }

            var fileName = $"backup_mesafacil_{DateTime.UtcNow:yyyyMMdd_HHmmss}.db";
            var fullPath = Path.Combine(backupsDir, fileName);

            // SQLite VACUUM INTO genera una copia consistente e inmutable sin bloquear lecturas ni escrituras
            var sanitizedPath = fullPath.Replace('\\', '/');
            await _context.Database.ExecuteSqlRawAsync($"VACUUM INTO '{sanitizedPath}'", ct);

            var fileInfo = new FileInfo(fullPath);
            _logger.LogInformation("Respaldo SQLite generado con éxito en: {Path} ({Bytes} bytes)", fullPath, fileInfo.Length);

            return Ok(new Response<DTO.Edge.EdgeBackupResultDTO>
            {
                isSuccess = true,
                Message = "Respaldo SQLite generado exitosamente.",
                Data = new DTO.Edge.EdgeBackupResultDTO
                {
                    FileName = fileName,
                    FilePath = fullPath,
                    SizeBytes = fileInfo.Length,
                    CreatedAtUtc = DateTime.UtcNow
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar respaldo VACUUM INTO para SQLite.");
            return StatusCode(500, new Response<string>
            {
                isSuccess = false,
                Message = $"Error al generar respaldo de base de datos: {ex.Message}"
            });
        }
    }

    private static List<string> GetLocalIPv4Addresses()
    {
        var addresses = new List<string>();
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    addresses.Add(ip.ToString());
                }
            }
        }
        catch
        {
            // Fallback silencioso
        }
        return addresses;
    }
}

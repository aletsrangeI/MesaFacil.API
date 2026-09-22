using Domain.Entities;
using DTO.Suscripcion;
using Interface.Suscripciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence.Context;

namespace UseCases.Suscripciones;

/// <summary>
/// Spec 033: Implementación de ITenantStatusService con IMemoryCache para evaluar
/// el Kill-Switch operativo en microsegundos sin degradar el rendimiento del POS.
/// </summary>
public class TenantStatusService : ITenantStatusService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantStatusService> _logger;

    private const string CacheKeyPrefix = "tenant_status_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public TenantStatusService(
        ApplicationDbContext context,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<TenantStatusService> logger)
    {
        _context = context;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TenantStatusResult> ObtenerEstadoTenantAsync(int empresaId)
    {
        // En modo demo / gating desactivado, el kill-switch no bloquea salvo suspensión forzada
        var gatingHabilitado = _configuration.GetValue<bool>("FeatureGating:Enabled", false);

        var cacheKey = $"{CacheKeyPrefix}{empresaId}";

        if (_cache.TryGetValue(cacheKey, out TenantStatusResult? cachedResult) && cachedResult != null)
        {
            return cachedResult;
        }

        var suscripcion = await _context.EmpresasSuscripcion
            .AsNoTracking()
            .Where(s => s.IdEmpresa == empresaId)
            .OrderByDescending(s => s.FechaInicio)
            .FirstOrDefaultAsync();

        var fallbackWhatsApp = _configuration.GetValue<string>("OrionSys:SoporteWhatsApp") ?? "+52 33 0000 0000";

        TenantStatusResult resultado;

        if (suscripcion == null)
        {
            // Si no tiene registro explícito de suscripción y el gating está apagado, asumimos Activa
            resultado = new TenantStatusResult
            {
                EmpresaId = empresaId,
                EstadoSuscripcion = EstadoSuscripcionValores.Activa,
                EstaSuspendido = false,
                EnPeriodoGracia = false,
                ContactoWhatsApp = fallbackWhatsApp
            };
        }
        else
        {
            var ahora = DateTime.UtcNow;
            var estaSuspendidoPorEstado = string.Equals(suscripcion.EstadoSuscripcion, EstadoSuscripcionValores.Suspendida, StringComparison.OrdinalIgnoreCase);

            // Si gating está habilitado, también evaluamos si venció y superó el periodo de gracia
            var estaVencidoFueraDeGracia = false;
            if (gatingHabilitado && suscripcion.FechaFinVigencia < ahora)
            {
                if (!suscripcion.EnPeriodoGracia)
                {
                    estaVencidoFueraDeGracia = true;
                }
            }

            var estaSuspendido = estaSuspendidoPorEstado || estaVencidoFueraDeGracia;

            resultado = new TenantStatusResult
            {
                EmpresaId = empresaId,
                EstadoSuscripcion = estaSuspendido ? EstadoSuscripcionValores.Suspendida : suscripcion.EstadoSuscripcion,
                EstaSuspendido = estaSuspendido,
                EnPeriodoGracia = suscripcion.EnPeriodoGracia,
                FechaFinVigencia = suscripcion.FechaFinVigencia,
                MotivoSuspension = suscripcion.MotivoSuspension ?? (estaVencidoFueraDeGracia ? "Suscripción vencida fuera de periodo de gracia" : null),
                ContactoWhatsApp = !string.IsNullOrWhiteSpace(suscripcion.ContactoWhatsApp) ? suscripcion.ContactoWhatsApp : fallbackWhatsApp
            };
        }

        _cache.Set(cacheKey, resultado, CacheDuration);
        return resultado;
    }

    public void InvalidarCacheTenant(int empresaId)
    {
        var cacheKey = $"{CacheKeyPrefix}{empresaId}";
        _cache.Remove(cacheKey);
        _logger.LogInformation("[Spec 033] Caché de estado invalidada para Tenant EmpresaId: {EmpresaId}", empresaId);
    }

    public async Task<bool> ActualizarEstadoDesdeHubAsync(ActualizarEstadoLicenciaRequestDTO request)
    {
        var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Id == request.EmpresaId);
        if (empresa == null)
        {
            _logger.LogWarning("[Spec 033] Intento de actualizar licencia para EmpresaId inexistente: {EmpresaId}", request.EmpresaId);
            return false;
        }

        var suscripcion = await _context.EmpresasSuscripcion
            .FirstOrDefaultAsync(s => s.IdEmpresa == request.EmpresaId);

        var ahora = DateTime.UtcNow;

        if (suscripcion == null)
        {
            // Crear suscripción base si no existía
            var planDefault = await _context.CatPlanesSuscripcion.FirstOrDefaultAsync(p => p.IsActive)
                              ?? await _context.CatPlanesSuscripcion.FirstOrDefaultAsync();

            suscripcion = new EmpresaSuscripcion
            {
                IdEmpresa = request.EmpresaId,
                IdPlan = planDefault?.Id ?? 1,
                FechaInicio = ahora,
                FechaFinVigencia = request.FechaFinVigencia ?? ahora.AddMonths(1),
                EstadoSuscripcion = request.NuevoEstado,
                EnPeriodoGracia = request.EnPeriodoGracia ?? false,
                MotivoSuspension = request.Motivo,
                ContactoWhatsApp = request.ContactoWhatsApp,
                UltimaActualizacionHub = ahora,
                IsActive = true
            };
            _context.EmpresasSuscripcion.Add(suscripcion);
        }
        else
        {
            suscripcion.EstadoSuscripcion = request.NuevoEstado;
            if (request.FechaFinVigencia.HasValue)
                suscripcion.FechaFinVigencia = request.FechaFinVigencia.Value;
            if (request.EnPeriodoGracia.HasValue)
                suscripcion.EnPeriodoGracia = request.EnPeriodoGracia.Value;
            if (!string.IsNullOrWhiteSpace(request.Motivo))
                suscripcion.MotivoSuspension = request.Motivo;
            if (!string.IsNullOrWhiteSpace(request.ContactoWhatsApp))
                suscripcion.ContactoWhatsApp = request.ContactoWhatsApp;

            suscripcion.UltimaActualizacionHub = ahora;
            suscripcion.UpdatedAt = ahora;
        }

        await _context.SaveChangesAsync();

        // Inmediatamente purgar caché para que surta efecto en el siguiente milisegundo
        InvalidarCacheTenant(request.EmpresaId);

        _logger.LogInformation("[Spec 033] Licencia actualizada desde Hub para EmpresaId {EmpresaId}. NuevoEstado: {Estado}", request.EmpresaId, request.NuevoEstado);

        return true;
    }
}

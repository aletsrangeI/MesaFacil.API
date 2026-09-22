using System.Net;
using System.Text.Json;
using DTO.Suscripcion;
using Interface.Suscripciones;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebApi.Middlewares;

/// <summary>
/// Spec 033: Middleware de Kill-Switch para Multi-Tenant SaaS.
/// Intercepta cada petición autenticada, verifica el estado del tenant en microsegundos vía IMemoryCache
/// y devuelve HTTP 402 Payment Required si el restaurante ha sido suspendido por falta de pago.
/// </summary>
public class TenantStatusMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantStatusMiddleware> _logger;

    // Rutas que siempre deben fluir libremente sin importar si el tenant está suspendido
    private static readonly string[] RutasExentas = new[]
    {
        "/api/auth",
        "/api/health",
        "/api/internal/licensing", // Webhook para reactivación
        "/hubs/",                  // Permitir conexión SignalR para recibir evento de desbloqueo
        "/swagger",
        "/scalar",
        "/openapi"
    };

    public TenantStatusMiddleware(RequestDelegate next, ILogger<TenantStatusMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantStatusService tenantStatusService)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        // 1. Omitir rutas de infraestructura o autenticación
        foreach (var ruta in RutasExentas)
        {
            if (path.StartsWith(ruta, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
        }

        // 2. Si el usuario no está autenticado, dejar que continúe al pipeline de autorización
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        // 3. Extraer el tenant (empresa_id) del ClaimsPrincipal
        var empresaIdClaim = context.User.FindFirst("empresa_id")?.Value;
        if (string.IsNullOrEmpty(empresaIdClaim) || !int.TryParse(empresaIdClaim, out var empresaId))
        {
            await _next(context);
            return;
        }

        // 4. Evaluar estado de suscripción del tenant (Caché ultrarrápida en memoria)
        var estadoTenant = await tenantStatusService.ObtenerEstadoTenantAsync(empresaId);

        if (estadoTenant.EstaSuspendido)
        {
            // Permitir consultas de facturas pasadas o mi-suscripción para que el cliente pueda ver su estado
            if (path.Contains("/api/empresa-suscripcion/mi-suscripcion") || path.Contains("/api/planes-suscripcion"))
            {
                await _next(context);
                return;
            }

            _logger.LogWarning("[Spec 033] Petición bloqueada (402 Payment Required). EmpresaId: {EmpresaId} suspendida. Ruta: {Path}", empresaId, path);

            context.Response.StatusCode = (int)HttpStatusCode.PaymentRequired;
            context.Response.ContentType = "application/json";

            var responsePayload = new TenantSuspendidoResponseDTO
            {
                IsSuccess = false,
                ErrorCode = "SUBSCRIPTION_SUSPENDED",
                Message = "El servicio de MesaFácil para esta empresa se encuentra temporalmente suspendido por falta de pago.",
                ContactoWhatsApp = estadoTenant.ContactoWhatsApp,
                FechaFinVigencia = estadoTenant.FechaFinVigencia,
                Motivo = estadoTenant.MotivoSuspension
            };

            var json = JsonSerializer.Serialize(responsePayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
            return;
        }

        await _next(context);
    }
}

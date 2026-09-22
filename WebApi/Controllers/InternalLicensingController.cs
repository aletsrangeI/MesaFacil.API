using Common;
using DTO.Suscripcion;
using Interface.Suscripciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApi.Hubs;

namespace WebApi.Controllers;

/// <summary>
/// Spec 033: Controlador receptor de instrucciones de licenciamiento y Kill-Switch
/// emitidas por OrionSys Central Hub (Spec 034) u operaciones automatizadas de cobranza.
/// </summary>
[ApiController]
[Route("api/internal/licensing")]
public class InternalLicensingController : ControllerBase
{
    private readonly ITenantStatusService _tenantStatusService;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<MesasHub> _mesasHub;
    private readonly ILogger<InternalLicensingController> _logger;

    public InternalLicensingController(
        ITenantStatusService tenantStatusService,
        IConfiguration configuration,
        IHubContext<MesasHub> mesasHub,
        ILogger<InternalLicensingController> logger)
    {
        _tenantStatusService = tenantStatusService;
        _configuration = configuration;
        _mesasHub = mesasHub;
        _logger = logger;
    }

    /// <summary>
    /// Webhook seguro para actualizar el estado operativo de una empresa (Activa, EnGracia, Suspendida).
    /// Autenticado mediante cabecera 'X-Orion-Key'.
    /// </summary>
    [HttpPost("estado")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<bool>>> ActualizarEstado([FromBody] ActualizarEstadoLicenciaRequestDTO request)
    {
        // Validar cabecera de seguridad
        if (!ValidarApiKey())
        {
            _logger.LogWarning("[Spec 033] Intento no autorizado en webhook de licenciamiento desde IP: {IP}", HttpContext.Connection.RemoteIpAddress);
            return Unauthorized(new Response<bool>
            {
                isSuccess = false,
                Message = "No autorizado. Clave X-Orion-Key inválida o ausente."
            });
        }

        if (request.EmpresaId <= 0 || string.IsNullOrWhiteSpace(request.NuevoEstado))
        {
            return BadRequest(new Response<bool>
            {
                isSuccess = false,
                Message = "EmpresaId y NuevoEstado son obligatorios."
            });
        }

        var exito = await _tenantStatusService.ActualizarEstadoDesdeHubAsync(request);
        if (!exito)
        {
            return NotFound(new Response<bool>
            {
                isSuccess = false,
                Message = $"No se encontró la empresa con ID {request.EmpresaId}."
            });
        }

        // Emitir evento por SignalR a todas las terminales para que actualicen su estado inmediatamente
        try
        {
            await _mesasHub.Clients.All.SendAsync("TenantStatusChanged", new
            {
                empresaId = request.EmpresaId,
                nuevoEstado = request.NuevoEstado,
                motivo = request.Motivo,
                contactoWhatsApp = request.ContactoWhatsApp
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[Spec 033] Error al propagar evento SignalR de cambio de estado de tenant");
        }

        return Ok(new Response<bool>
        {
            Data = true,
            isSuccess = true,
            Message = $"Estado de suscripción actualizado con éxito a '{request.NuevoEstado}' para la empresa {request.EmpresaId}."
        });
    }

    /// <summary>
    /// Consulta el estado actual de licenciamiento de una empresa (usado por OrionSys Central Hub para comprobación y healthcheck).
    /// </summary>
    [HttpGet("estado/{empresaId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Response<TenantStatusResult>>> ObtenerEstado(int empresaId)
    {
        if (!ValidarApiKey())
        {
            return Unauthorized(new Response<TenantStatusResult>
            {
                isSuccess = false,
                Message = "No autorizado. Clave X-Orion-Key inválida o ausente."
            });
        }

        var estado = await _tenantStatusService.ObtenerEstadoTenantAsync(empresaId);
        return Ok(new Response<TenantStatusResult>
        {
            Data = estado,
            isSuccess = true,
            Message = "Estado consultado correctamente."
        });
    }

    private bool ValidarApiKey()
    {
        var configKey = _configuration.GetValue<string>("OrionSys:InternalApiKey");
        if (string.IsNullOrEmpty(configKey))
        {
            // En desarrollo local / demo si no está configurada, permitimos clave default "orionsys_internal_secret_2026"
            configKey = "orionsys_internal_secret_2026";
        }

        if (!Request.Headers.TryGetValue("X-Orion-Key", out var headerValue))
        {
            return false;
        }

        return string.Equals(headerValue.ToString(), configKey, StringComparison.Ordinal);
    }
}

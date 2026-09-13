using System.Security.Claims;
using Common;
using DTO.Seguridad;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Spec 024: Candado de Supervisor con PIN de 4 dígitos.
/// </summary>
[Authorize]
[Route("api/seguridad")]
[ApiController]
public class SeguridadSupervisorController : ControllerBase
{
    private readonly ISupervisorPinSecurityService _pinService;

    public SeguridadSupervisorController(ISupervisorPinSecurityService pinService)
    {
        _pinService = pinService;
    }

    [HttpPost("configurar-pin")]
    public async Task<ActionResult<Response<bool>>> ConfigurarPin([FromBody] ConfigurarPinRequestDTO request, CancellationToken ct)
    {
        var idUsuarioSolicitante = ObtenerIdUsuarioAutenticado();
        var response = await _pinService.ConfigurarPinAsync(request, idUsuarioSolicitante ?? 0, ct);
        return Ok(response);
    }

    /// <summary>
    /// Anónimo a nivel de [Authorize] de sesión normal del mesero (que ya está autenticado con su
    /// propio JWT al usar el POS); el candado real es el PIN de 4 dígitos del supervisor validado
    /// aquí, no un segundo login. Devuelve HTTP 200 siempre (autorizado=false en el body) para que
    /// el modal táctil del POS pueda mostrar el mensaje de error sin manejar códigos HTTP.
    /// </summary>
    [HttpPost("autorizar-supervisor-pin")]
    public async Task<ActionResult<AutorizarSupervisorPinResponseDTO>> AutorizarSupervisorPin([FromBody] AutorizarSupervisorPinRequestDTO request, CancellationToken ct)
    {
        var response = await _pinService.AutorizarAsync(request, ct);
        return Ok(response);
    }

    private int? ObtenerIdUsuarioAutenticado()
    {
        var claim = User.FindFirst("uid") ?? User.FindFirst("idUsuario") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
    }
}

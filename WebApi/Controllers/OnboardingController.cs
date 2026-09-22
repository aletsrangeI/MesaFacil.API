using Common;
using DTO.Onboarding;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OnboardingController : ControllerBase
{
    private readonly IOnboardingApplication _onboardingApplication;

    public OnboardingController(IOnboardingApplication onboardingApplication)
    {
        _onboardingApplication = onboardingApplication;
    }

    /// <summary>
    /// Consulta el estado de configuración inicial de la empresa del usuario autenticado.
    /// </summary>
    [HttpGet("estado")]
    public async Task<ActionResult<Response<OnboardingEstadoDTO>>> GetEstado(CancellationToken ct)
    {
        var empresaIdClaim = User.FindFirst("empresa_id")?.Value;
        int.TryParse(empresaIdClaim, out var empresaId);

        var response = await _onboardingApplication.ObtenerEstadoAsync(empresaId, ct);
        return Ok(response);
    }

    /// <summary>
    /// Provisiona en una sola transacción atómica toda la infraestructura básica para operar el restaurante.
    /// </summary>
    [HttpPost("provisionar")]
    public async Task<ActionResult<Response<ProvisionarRestauranteResponseDTO>>> Provisionar(
        [FromBody] ProvisionarRestauranteRequestDTO dto,
        CancellationToken ct)
    {
        var empresaIdClaim = User.FindFirst("empresa_id")?.Value;
        int.TryParse(empresaIdClaim, out var empresaId);

        var uidClaim = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(uidClaim, out var uid);

        var response = await _onboardingApplication.ProvisionarRestauranteAsync(dto, uid, empresaId, ct);

        if (!response.isSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}

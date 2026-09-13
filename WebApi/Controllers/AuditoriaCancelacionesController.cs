using Common;
using DTO.Auditoria;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>Spec 024, sección 2.4: Monitor de Auditoría y Umbral del 2% en Dashboard.</summary>
[Authorize]
[Route("api/auditoria")]
[ApiController]
public class AuditoriaCancelacionesController : ControllerBase
{
    private readonly IAuditoriaCancelacionesService _service;

    public AuditoriaCancelacionesController(IAuditoriaCancelacionesService service)
    {
        _service = service;
    }

    [HttpGet("cancelaciones-turno")]
    public async Task<ActionResult<Response<ResumenCancelacionesTurnoDTO>>> ObtenerCancelacionesTurno([FromQuery] int idTurno, CancellationToken ct)
    {
        var response = await _service.ObtenerResumenTurnoAsync(idTurno, ct);
        return Ok(response);
    }

    [HttpGet("motivos-cancelacion")]
    public async Task<ActionResult<Response<List<MotivoCancelacionDTO>>>> ObtenerMotivosCancelacion(CancellationToken ct)
    {
        var response = await _service.ObtenerMotivosAsync(ct);
        return Ok(response);
    }
}

using Common;
using DTO.Pago;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PagosController : ControllerBase
{
    private readonly IPagoApplication _pagoApplication;

    public PagosController(IPagoApplication pagoApplication)
    {
        _pagoApplication = pagoApplication;
    }

    [HttpPost("Registrar")]
    public async Task<ActionResult<Response<bool>>> RegistrarPagoAsync([FromBody] RegistrarPagoDTO dto)
    {
        var response = await _pagoApplication.RegistrarPagoAsync(dto);
        return Ok(response);
    }
}

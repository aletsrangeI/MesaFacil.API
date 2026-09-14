using Common;
using DTO.Cuenta;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CuentasController : ControllerBase
{
    private readonly ICuentaApplication _cuentaApplication;

    public CuentasController(ICuentaApplication cuentaApplication)
    {
        _cuentaApplication = cuentaApplication;
    }

    [HttpPost("Generar/{idPedido:guid}")]
    public async Task<ActionResult<Response<CuentaDTO>>> GenerarCuentaAsync(Guid idPedido)
    {
        var response = await _cuentaApplication.GenerarCuentaAsync(idPedido);
        return Ok(response);
    }
}

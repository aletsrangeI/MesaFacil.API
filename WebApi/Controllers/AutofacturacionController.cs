using Common;
using DTO.Facturacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UseCases.Facturacion;

namespace WebApi.Controllers;

/// <summary>
/// Spec 020: Portal Público de Autofacturación Comensal. Sin autenticación (igual patrón que
/// DemoController y el endpoint /api/health de Spec 019): el comensal escanea el QR del ticket
/// térmico y captura sus datos fiscales desde su propio celular, sin sesión ni JWT.
/// </summary>
[AllowAnonymous]
[Route("api/autofacturacion")]
[ApiController]
public class AutofacturacionController : ControllerBase
{
    private readonly IAutofacturacionComensalService _autofacturacionService;

    public AutofacturacionController(IAutofacturacionComensalService autofacturacionService)
    {
        _autofacturacionService = autofacturacionService;
    }

    /// <summary>
    /// Valida el GUID del ticket (QR), retornando total, fecha y si ya fue facturado o si venció
    /// su vigencia de autofacturación.
    /// </summary>
    [HttpGet("validar-ticket/{guid:guid}")]
    public async Task<ActionResult<Response<ValidarTicketResultDTO>>> ValidarTicket(Guid guid)
    {
        var result = await _autofacturacionService.ValidarTicketAsync(guid);
        return Ok(result);
    }

    /// <summary>
    /// Recibe los datos fiscales del comensal, timbra el comprobante y retorna la factura generada.
    /// </summary>
    [HttpPost("generar-factura")]
    public async Task<ActionResult<Response<FacturaVentaDTO>>> GenerarFactura([FromBody] GenerarFacturaAutofacturaRequestDTO request)
    {
        var result = await _autofacturacionService.GenerarFacturaAsync(request);
        if (!result.isSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}

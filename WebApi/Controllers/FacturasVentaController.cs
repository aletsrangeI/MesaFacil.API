using Common;
using DTO.Facturacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UseCases.Facturacion;

namespace WebApi.Controllers;

/// <summary>
/// Spec 020: emisión de CFDI 4.0 a comensales desde el POS/administración. Protegido con la
/// autenticación JWT existente del proyecto (igual que ComprasController, CxPController, etc.).
/// </summary>
[Authorize]
[Route("api/facturas-venta")]
[ApiController]
public class FacturasVentaController : ControllerBase
{
    private readonly IFacturaVentaService _facturaVentaService;

    public FacturasVentaController(IFacturaVentaService facturaVentaService)
    {
        _facturaVentaService = facturaVentaService;
    }

    /// <summary>
    /// Timbra un pedido ya cobrado con los datos fiscales del receptor proporcionados por el cajero.
    /// </summary>
    [HttpPost("timbrar-pedido")]
    public async Task<ActionResult<Response<FacturaVentaDTO>>> TimbrarPedido([FromBody] TimbrarPedidoRequestDTO request)
    {
        var result = await _facturaVentaService.TimbrarPedidoAsync(request);
        if (!result.isSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Descarga el archivo XML firmado por el SAT.
    /// </summary>
    [HttpGet("{id:int}/descargar-xml")]
    public async Task<IActionResult> DescargarXml(int id)
    {
        var result = await _facturaVentaService.DescargarXmlAsync(id);
        if (!result.isSuccess || result.Data == null)
            return NotFound(result);

        var bytes = System.Text.Encoding.UTF8.GetBytes(result.Data);
        return File(bytes, "application/xml", $"factura-{id}.xml");
    }

    /// <summary>
    /// Genera y descarga la representación impresa en PDF.
    /// </summary>
    [HttpGet("{id:int}/descargar-pdf")]
    public async Task<IActionResult> DescargarPdf(int id)
    {
        var result = await _facturaVentaService.DescargarPdfAsync(id);
        if (!result.isSuccess || result.Data == null)
            return NotFound(result);

        return File(result.Data, "application/pdf", $"factura-{id}.pdf");
    }

    /// <summary>
    /// Envía XML y PDF al comensal (stub: registra la intención de envío; el proyecto no tiene
    /// un IEmailService/proveedor SMTP configurado en ninguna otra parte de la solución).
    /// </summary>
    [HttpPost("{id:int}/enviar-correo")]
    public async Task<ActionResult<Response<bool>>> EnviarCorreo(int id)
    {
        var result = await _facturaVentaService.EnviarCorreoAsync(id);
        if (!result.isSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Cancela el CFDI ante el SAT enviando el motivo SAT (01, 02, 03, 04).
    /// </summary>
    [HttpPost("{id:int}/cancelar")]
    public async Task<ActionResult<Response<FacturaVentaDTO>>> Cancelar(int id, [FromBody] CancelarFacturaRequestDTO request)
    {
        var result = await _facturaVentaService.CancelarAsync(id, request);
        if (!result.isSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Consulta saldo de timbres y consumo histórico de la Empresa del usuario autenticado.
    /// </summary>
    [HttpGet("bolsa-timbres")]
    public async Task<ActionResult<Response<BolsaTimbresDTO>>> BolsaTimbres([FromQuery] int idEmpresa)
    {
        var result = await _facturaVentaService.ObtenerBolsaTimbresAsync(idEmpresa);
        if (!result.isSuccess)
            return NotFound(result);

        return Ok(result);
    }
}

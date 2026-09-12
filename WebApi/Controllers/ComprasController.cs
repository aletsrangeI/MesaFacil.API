using System.Security.Claims;
using Common;
using DTO.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using UseCases.Compras;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ComprasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICfdiXmlParserService _cfdiParser;
    private readonly IRecepcionCompraService _recepcionCompraService;

    public ComprasController(
        ApplicationDbContext context,
        ICfdiXmlParserService cfdiParser,
        IRecepcionCompraService recepcionCompraService)
    {
        _context = context;
        _cfdiParser = cfdiParser;
        _recepcionCompraService = recepcionCompraService;
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst("idUsuario") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out var id))
            return id;
        return null;
    }

    /// <summary>
    /// Ingesta inteligente de archivo XML de CFDI 4.0 / 3.3.
    /// Soporta envío de texto XML en JSON o archivo adjunto multipart/form-data.
    /// </summary>
    [HttpPost("ParseXml")]
    public async Task<ActionResult<Response<CfdiParseResultDTO>>> ParseXml([FromBody] CfdiXmlParseRequestDTO? bodyDto)
    {
        var response = new Response<CfdiParseResultDTO>();

        string? xmlContent = bodyDto?.XmlContent;

        // Si no vino en el body JSON, revisar si se envió en multipart form
        if (string.IsNullOrWhiteSpace(xmlContent) && Request.HasFormContentType && Request.Form.Files.Count > 0)
        {
            var file = Request.Form.Files[0];
            using var reader = new StreamReader(file.OpenReadStream());
            xmlContent = await reader.ReadToEndAsync();
        }

        if (string.IsNullOrWhiteSpace(xmlContent))
        {
            response.isSuccess = false;
            response.Message = "Debe proporcionar el contenido del archivo XML (en cuerpo JSON o formulario).";
            return BadRequest(response);
        }

        try
        {
            var result = await _cfdiParser.ParsearCfdiAsync(xmlContent);
            response.Data = result;
            response.isSuccess = true;
            response.Message = result.FacturaYaExiste
                ? $"Atención: Esta factura ya existe en el sistema (UUID: {result.UUID})."
                : "XML CFDI procesado y cotejado exitosamente.";

            return Ok(response);
        }
        catch (FormatException fex)
        {
            response.isSuccess = false;
            response.Message = fex.Message;
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al interpretar el comprobante fiscal CFDI: {ex.Message}";
            return BadRequest(response);
        }
    }

    /// <summary>
    /// Consulta si un UUID del SAT ya fue registrado en el sistema para prevenir duplicidades.
    /// </summary>
    [HttpGet("VerificarUUID/{uuid}")]
    public async Task<ActionResult<Response<bool>>> VerificarUuid(string uuid)
    {
        var response = new Response<bool>();
        if (string.IsNullOrWhiteSpace(uuid))
        {
            response.Data = false;
            response.isSuccess = true;
            return Ok(response);
        }

        var uuidNorm = uuid.Trim().ToUpperInvariant();
        var existe = await _context.ComprasFactura
            .AnyAsync(c => c.UUID == uuidNorm && c.Estado != "Cancelada");

        response.Data = existe;
        response.isSuccess = true;
        response.Message = existe ? "El UUID ya se encuentra registrado." : "El UUID está disponible.";
        return Ok(response);
    }

    /// <summary>
    /// Registra una nueva compra o factura recibida (manual o por XML).
    /// Si AplicarDirecto es true, impacta existencias, Kárdex y recálculo de CPP.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Response<CompraFacturaDTO>>> RegistrarCompra([FromBody] RegistrarCompraDTO dto)
    {
        var userId = GetCurrentUserId();
        var result = await _recepcionCompraService.RegistrarCompraAsync(dto, userId);

        if (!result.isSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetCompraPorId), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Aplica una compra previamente guardada en estado 'Borrador'.
    /// Genera la entrada en almacén, kárdex y recálculo de costos.
    /// </summary>
    [HttpPost("{id:int}/Aplicar")]
    public async Task<ActionResult<Response<CompraFacturaDTO>>> AplicarCompra(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _recepcionCompraService.AplicarCompraAsync(id, userId);

        if (!result.isSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Cancela una factura de compra. Si estaba aplicada, reversa el inventario con un movimiento de cancelación en kárdex.
    /// </summary>
    [HttpPost("{id:int}/Cancelar")]
    public async Task<ActionResult<Response<bool>>> CancelarCompra(int id, [FromBody] CancelarCompraRequest? request)
    {
        var userId = GetCurrentUserId();
        var result = await _recepcionCompraService.CancelarCompraAsync(id, userId, request?.Motivo);

        if (!result.isSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle completo de una factura de compra con sus partidas.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Response<CompraFacturaDTO>>> GetCompraPorId(int id)
    {
        var result = await _recepcionCompraService.ObtenerPorIdAsync(id);
        if (!result.isSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Listado de facturas de compra con filtros avanzados por sucursal, almacén, proveedor, estatus y fechas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Response<List<CompraItemResumenDTO>>>> GetCompras([FromQuery] CompraFiltroDTO filtro)
    {
        var result = await _recepcionCompraService.ObtenerListadoAsync(filtro);
        return Ok(result);
    }

    /// <summary>
    /// Métricas y KPIs de compras del mes (totales, borradores, aplicadas, a crédito).
    /// </summary>
    [HttpGet("Kpis")]
    public async Task<ActionResult<Response<ResumenKpisComprasDTO>>> GetKpis([FromQuery] int? idSucursal = null)
    {
        var result = await _recepcionCompraService.ObtenerKpisAsync(idSucursal);
        return Ok(result);
    }
}

public class CancelarCompraRequest
{
    public string? Motivo { get; set; }
}

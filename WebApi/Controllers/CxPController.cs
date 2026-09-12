using System.Security.Claims;
using Common;
using DTO.CxP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UseCases.CxP;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CxPController : ControllerBase
{
    private readonly ICxPService _cxpService;

    public CxPController(ICxPService cxpService)
    {
        _cxpService = cxpService;
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst("idUsuario") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out var id))
            return id;
        return null;
    }

    /// <summary>
    /// Listado de cuentas por pagar con filtros por sucursal, proveedor, estado, semáforo y fechas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Response<List<CuentaPorPagarItemDTO>>>> GetListado([FromQuery] FiltroCxPDTO filtro)
    {
        var result = await _cxpService.ObtenerListadoAsync(filtro);
        return Ok(result);
    }

    /// <summary>
    /// Consulta el detalle de una cuenta por pagar por su ID, incluyendo el historial cronológico de abonos.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Response<CuentaPorPagarDTO>>> GetById(int id)
    {
        var result = await _cxpService.ObtenerPorIdAsync(id);
        if (!result.isSuccess)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Métricas y KPIs de cuentas por pagar (Total por pagar, vencido, vence esta semana, pagado en el mes).
    /// </summary>
    [HttpGet("Kpis")]
    public async Task<ActionResult<Response<ResumenKpisCxPDTO>>> GetKpis([FromQuery] int? idSucursal)
    {
        var result = await _cxpService.ObtenerKpisAsync(idSucursal);
        return Ok(result);
    }

    /// <summary>
    /// Registra un abono o liquidación total a una cuenta por pagar.
    /// Si PagarDesdeCajaChica = true, genera un MovimientoCaja de tipo Egreso en el turno activo.
    /// </summary>
    [HttpPost("Abonar")]
    public async Task<ActionResult<Response<PagoCuentaPorPagarDTO>>> RegistrarAbono([FromBody] RegistrarPagoCxPDTO dto)
    {
        var idUsuario = GetCurrentUserId();
        var result = await _cxpService.RegistrarAbonoAsync(dto, idUsuario);
        if (!result.isSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Cancela una cuenta por pagar si no cuenta con abonos registrados.
    /// </summary>
    [HttpPost("{id}/Cancelar")]
    public async Task<ActionResult<Response<bool>>> Cancelar(int id, [FromQuery] string? motivo)
    {
        var idUsuario = GetCurrentUserId();
        var result = await _cxpService.CancelarCuentaPorPagarAsync(id, idUsuario, motivo);
        if (!result.isSuccess)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Reporte ejecutivo de Antigüedad de Saldos desglosado en 5 buckets:
    /// Al corriente, 1-15 días, 16-30 días, 31-60 días y >60 días, agrupado por proveedor.
    /// </summary>
    [HttpGet("AntiguedadSaldos")]
    public async Task<ActionResult<Response<ReporteAntiguedadSaldosDTO>>> GetAntiguedadSaldos([FromQuery] int? idSucursal)
    {
        var result = await _cxpService.ObtenerReporteAntiguedadSaldosAsync(idSucursal);
        return Ok(result);
    }

    /// <summary>
    /// Estado de cuenta integral por proveedor con saldo vivo y desglose cronológico de facturas y pagos.
    /// </summary>
    [HttpGet("EstadoCuenta/{idProveedor}")]
    public async Task<ActionResult<Response<EstadoCuentaProveedorDTO>>> GetEstadoCuentaProveedor(int idProveedor, [FromQuery] int? idSucursal)
    {
        var result = await _cxpService.ObtenerEstadoCuentaProveedorAsync(idProveedor, idSucursal);
        if (!result.isSuccess)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene los turnos de caja actualmente abiertos en la sucursal para debitar pagos en efectivo de caja chica.
    /// </summary>
    [HttpGet("TurnosActivos/{idSucursal}")]
    public async Task<ActionResult<Response<List<TurnoActivoDTO>>>> GetTurnosActivos(int idSucursal)
    {
        var result = await _cxpService.ObtenerTurnosActivosAsync(idSucursal);
        return Ok(result);
    }
}

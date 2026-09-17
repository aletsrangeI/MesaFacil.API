using Common;
using Domain.Entities;
using DTO.MovimientoCaja;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MovimientoCajaController : ControllerBase
{
    private readonly IMovimientoCajaApplication _movimientoCajaApplication;
    private readonly ApplicationDbContext _context;

    public MovimientoCajaController(
        IMovimientoCajaApplication movimientoCajaApplication,
        ApplicationDbContext context)
    {
        _movimientoCajaApplication = movimientoCajaApplication;
        _context = context;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] MovimientoCajaDTO dto)
    {
        var response = _movimientoCajaApplication.Insert(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<MovimientoCajaDTO>>> GetAll()
    {
        var response = _movimientoCajaApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id:guid}")]
    public ActionResult<Response<MovimientoCajaDTO>> GetById(Guid id)
    {
        var response = _movimientoCajaApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] MovimientoCajaDTO dto)
    {
        var response = _movimientoCajaApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id:guid}")]
    public ActionResult<Response<bool>> Delete(Guid id)
    {
        var response = _movimientoCajaApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<MovimientoCajaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _movimientoCajaApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _movimientoCajaApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("insert-async")]
    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] MovimientoCajaDTO dto)
    {
        var response = await _movimientoCajaApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("getall-async")]
    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<MovimientoCajaDTO>>>> GetAllAsync()
    {
        var response = await _movimientoCajaApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("getbyid-async/{id:guid}")]
    [HttpGet("GetByIdAsync/{id:guid}")]
    public async Task<ActionResult<Response<MovimientoCajaDTO>>> GetByIdAsync(Guid id)
    {
        var response = await _movimientoCajaApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("update-async/{id:guid}")]
    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] MovimientoCajaDTO dto, Guid? id = null)
    {
        if (id.HasValue) dto.Id = id.Value;
        var response = await _movimientoCajaApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("delete-async/{id:guid}")]
    [HttpDelete("DeleteAsync/{id:guid}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(Guid id)
    {
        var response = await _movimientoCajaApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("getpaged-async")]
    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<MovimientoCajaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _movimientoCajaApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("count-async")]
    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _movimientoCajaApplication.CountAsync();
        return Ok(response);
    }

    /// <summary>
    /// Historial de movimientos con enriquecimiento de datos de turno y usuario para auditoría
    /// </summary>
    [HttpGet("historial")]
    public async Task<ActionResult<Response<IEnumerable<MovimientoCajaItemDTO>>>> Historial(
        [FromQuery] int? idTurno = null,
        [FromQuery] int? idSucursal = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        var response = new Response<IEnumerable<MovimientoCajaItemDTO>>();

        try
        {
            DateTime? fInicio = fechaInicio.HasValue ? DateTime.SpecifyKind(fechaInicio.Value, DateTimeKind.Utc) : null;
            DateTime? fFin = fechaFin.HasValue ? DateTime.SpecifyKind(fechaFin.Value, DateTimeKind.Utc) : null;

            // 1. Obtener Turno de referencia si se solicitó por idTurno
            Turno? turnoFiltro = null;
            if (idTurno.HasValue && idTurno.Value > 0)
            {
                turnoFiltro = await _context.Turnos
                    .Include(t => t.Sucursal)
                    .Include(t => t.Usuario)
                    .FirstOrDefaultAsync(t => t.Id == idTurno.Value);
            }

            // 2. Movimientos manuales de caja (Entradas / Egresos)
            var movQuery = _context.MovimientosCaja
                .Include(m => m.Turno)
                    .ThenInclude(t => t.Usuario)
                .Include(m => m.Turno)
                    .ThenInclude(t => t.Sucursal)
                .Where(m => m.IsActive);

            if (idTurno.HasValue && idTurno.Value > 0)
            {
                movQuery = movQuery.Where(m => m.IdTurno == idTurno.Value);
            }

            if (idSucursal.HasValue && idSucursal.Value > 0)
            {
                movQuery = movQuery.Where(m => m.Turno.IdSucursal == idSucursal.Value);
            }

            if (fInicio.HasValue)
            {
                movQuery = movQuery.Where(m => m.CreatedAt >= fInicio.Value);
            }

            if (fFin.HasValue)
            {
                movQuery = movQuery.Where(m => m.CreatedAt <= fFin.Value);
            }

            var movList = await movQuery
                .Select(m => new MovimientoCajaItemDTO
                {
                    Id = m.Id,
                    IdTurno = m.IdTurno,
                    IdSucursal = m.Turno != null ? m.Turno.IdSucursal : 0,
                    NombreSucursal = m.Turno != null && m.Turno.Sucursal != null ? m.Turno.Sucursal.Nombre : "",
                    NombreUsuario = m.Turno != null && m.Turno.Usuario != null ? (m.Turno.Usuario.NombreCompleto ?? "") : "",
                    Tipo = m.Tipo,
                    Monto = m.Monto,
                    Concepto = m.Nota != null && m.Nota.StartsWith("[") && m.Nota.Contains("]")
                        ? m.Nota.Substring(1, m.Nota.IndexOf("]") - 1)
                        : m.Tipo,
                    Nota = m.Nota != null && m.Nota.StartsWith("[") && m.Nota.Contains("]")
                        ? m.Nota.Substring(m.Nota.IndexOf("]") + 1).Trim()
                        : m.Nota,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            // 3. Pagos de cuentas/pedidos (Cobros de mesas, mostrador y delivery)
            var pagosQuery = _context.Pagos
                .Include(p => p.Cuenta)
                    .ThenInclude(c => c.Pedido)
                        .ThenInclude(ped => ped.Mesa)
                .Include(p => p.Cuenta)
                    .ThenInclude(c => c.Pedido)
                        .ThenInclude(ped => ped.Sucursal)
                .Include(p => p.Cuenta)
                    .ThenInclude(c => c.Pedido)
                        .ThenInclude(ped => ped.AbiertoPorUsuario)
                .Include(p => p.RecibidoPorUsuario)
                .Include(p => p.MetodoDePago)
                .Where(p => p.IsActive);

            if (idTurno.HasValue && idTurno.Value > 0)
            {
                if (turnoFiltro != null)
                {
                    var aperturaUtc = DateTime.SpecifyKind(turnoFiltro.Apertura, DateTimeKind.Utc);
                    pagosQuery = pagosQuery.Where(p => p.Cuenta.Pedido.IdSucursal == turnoFiltro.IdSucursal && p.PagadoEn >= aperturaUtc);
                    if (turnoFiltro.Cierre.HasValue)
                    {
                        var cierreUtc = DateTime.SpecifyKind(turnoFiltro.Cierre.Value, DateTimeKind.Utc);
                        pagosQuery = pagosQuery.Where(p => p.PagadoEn <= cierreUtc);
                    }
                }
                else
                {
                    pagosQuery = pagosQuery.Where(p => false);
                }
            }
            else
            {
                if (idSucursal.HasValue && idSucursal.Value > 0)
                {
                    pagosQuery = pagosQuery.Where(p => p.Cuenta.Pedido.IdSucursal == idSucursal.Value);
                }

                if (fInicio.HasValue)
                {
                    pagosQuery = pagosQuery.Where(p => p.PagadoEn >= fInicio.Value);
                }

                if (fFin.HasValue)
                {
                    pagosQuery = pagosQuery.Where(p => p.PagadoEn <= fFin.Value);
                }
            }

            var pagos = await pagosQuery.ToListAsync();

            // 4. Pre-cargar turnos relevantes para asociar a los cobros
            List<Turno> turnosRelevantes = new();
            if (pagos.Any())
            {
                if (turnoFiltro != null)
                {
                    turnosRelevantes.Add(turnoFiltro);
                }
                else
                {
                    var turnosQ = _context.Turnos
                        .Include(t => t.Usuario)
                        .Include(t => t.Sucursal)
                        .AsQueryable();

                    if (idSucursal.HasValue && idSucursal.Value > 0)
                    {
                        turnosQ = turnosQ.Where(t => t.IdSucursal == idSucursal.Value);
                    }

                    if (fInicio.HasValue)
                    {
                        var fMargen = fInicio.Value.AddDays(-1);
                        turnosQ = turnosQ.Where(t => t.Cierre == null || t.Cierre >= fMargen);
                    }

                    turnosRelevantes = await turnosQ.ToListAsync();
                }
            }

            var pagosList = pagos.Select(p =>
            {
                var ped = p.Cuenta?.Pedido;
                var pedSucursalId = ped?.IdSucursal ?? idSucursal ?? 0;
                var pFecha = DateTime.SpecifyKind(p.PagadoEn, DateTimeKind.Utc);

                var turnoAsociado = turnosRelevantes.FirstOrDefault(t =>
                    t.IdSucursal == pedSucursalId &&
                    pFecha >= DateTime.SpecifyKind(t.Apertura, DateTimeKind.Utc) &&
                    (!t.Cierre.HasValue || pFecha <= DateTime.SpecifyKind(t.Cierre.Value, DateTimeKind.Utc)));

                int itemTurnoId = turnoAsociado?.Id ?? 0;
                string itemSucursalNombre = ped?.Sucursal?.Nombre ?? turnoAsociado?.Sucursal?.Nombre ?? "";
                string itemUsuario = turnoAsociado?.Usuario?.NombreCompleto
                    ?? p.RecibidoPorUsuario?.NombreCompleto
                    ?? ped?.AbiertoPorUsuario?.NombreCompleto
                    ?? p.CreatedBy
                    ?? "Cajero";

                string mesaDesc;
                if (ped?.Mesa != null)
                {
                    mesaDesc = $"Mesa {ped.Mesa.Codigo}";
                }
                else if (!string.IsNullOrEmpty(ped?.CanalOrigen) && ped.CanalOrigen != "POS")
                {
                    mesaDesc = $"Pedido {ped.CanalOrigen}";
                }
                else
                {
                    mesaDesc = "Mostrador";
                }

                var metodoNombre = p.MetodoDePago?.Descripcion ?? "Pago";
                var concepto = $"Cobro {mesaDesc} ({metodoNombre})";

                var folio = ped != null && ped.FolioDiario > 0 ? $" • Folio #{ped.FolioDiario}" : "";
                var propinaInfo = p.Propina > 0 ? $" • Propina: ${p.Propina:F2}" : "";
                var refInfo = !string.IsNullOrWhiteSpace(p.Referencia) ? $" • Ref: {p.Referencia}" : "";
                var nota = $"Cuenta #{p.IdCuenta}{folio}{propinaInfo}{refInfo}";

                return new MovimientoCajaItemDTO
                {
                    Id = p.Id,
                    IdTurno = itemTurnoId,
                    IdSucursal = pedSucursalId,
                    NombreSucursal = itemSucursalNombre,
                    NombreUsuario = itemUsuario,
                    Tipo = "Ingreso",
                    Monto = p.Monto + p.Propina,
                    Concepto = concepto,
                    Nota = nota,
                    CreatedAt = p.PagadoEn != default ? p.PagadoEn : p.CreatedAt
                };
            }).ToList();

            // 5. Unificar y ordenar cronológicamente descendente
            var list = movList
                .Concat(pagosList)
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            response.Data = list;
            response.isSuccess = true;
            response.Message = "Movimientos encontrados";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }

        return Ok(response);
    }

    #endregion
}

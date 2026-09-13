using Common;
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
            var query = _context.MovimientosCaja
                .Include(m => m.Turno)
                    .ThenInclude(t => t.Usuario)
                .Include(m => m.Turno)
                    .ThenInclude(t => t.Sucursal)
                .Where(m => m.IsActive);

            if (idTurno.HasValue && idTurno.Value > 0)
            {
                query = query.Where(m => m.IdTurno == idTurno.Value);
            }

            if (idSucursal.HasValue && idSucursal.Value > 0)
            {
                query = query.Where(m => m.Turno.IdSucursal == idSucursal.Value);
            }

            if (fechaInicio.HasValue)
            {
                var fInicio = DateTime.SpecifyKind(fechaInicio.Value, DateTimeKind.Utc);
                query = query.Where(m => m.CreatedAt >= fInicio);
            }

            if (fechaFin.HasValue)
            {
                var fFin = DateTime.SpecifyKind(fechaFin.Value, DateTimeKind.Utc);
                query = query.Where(m => m.CreatedAt <= fFin);
            }

            var list = await query
                .OrderByDescending(m => m.CreatedAt)
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

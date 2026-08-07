using Common;
using DTO.TicketCocina;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TicketsCocinaController : ControllerBase
{
    private readonly ITicketCocinaApplication _ticketCocinaApplication;
    private readonly ApplicationDbContext _db;

    public TicketsCocinaController(ITicketCocinaApplication ticketCocinaApplication, ApplicationDbContext db)
    {
        _ticketCocinaApplication = ticketCocinaApplication;
        _db = db;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<int>> Insert([FromBody] TicketCocinaDTO dto)
    {
        var response = _ticketCocinaApplication.Insert(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<TicketCocinaDTO>>> GetAll()
    {
        var response = _ticketCocinaApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<TicketCocinaDTO>> GetById(int id)
    {
        var response = _ticketCocinaApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] TicketCocinaDTO dto)
    {
        var response = _ticketCocinaApplication.Update(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _ticketCocinaApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<TicketCocinaDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _ticketCocinaApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _ticketCocinaApplication.Count();
        return Ok(response);
    }

    [HttpGet("GetKdsBoard")]
    public async Task<IActionResult> GetKdsBoard()
    {
        // Custom query to get all active tickets with their details, product names, and modifiers
        var tickets = await _db.TicketsCocina
            .Where(t => t.IdEstadoTicketCocina == 1 || t.IdEstadoTicketCocina == 2) // Pending or Preparing
            .Select(t => new
            {
                t.Id,
                t.IdPedido,
                t.IdEstacion,
                t.IdEstadoTicketCocina,
                t.CreatedAt,
                Detalles = t.Detalles.Select(d => new
                {
                    d.Id,
                    d.IdEstadoItemKDS,
                    d.DetallePedido.Cantidad,
                    ProductoNombre = d.DetallePedido.Producto.Nombre,
                    Notas = d.DetallePedido.Notas,
                    Modificadores = d.DetallePedido.Modificadores.Select(m => m.Opcion.Nombre).ToList()
                }).ToList()
            })
            .ToListAsync();

        return Ok(new Response<object> { Data = tickets, isSuccess = true });
    }

    [HttpPut("ChangeTicketStatus/{id}/{status}")]
    public async Task<IActionResult> ChangeTicketStatus(int id, int status)
    {
        var ticket = await _db.TicketsCocina.FindAsync(id);
        if (ticket == null) return NotFound();
        ticket.IdEstadoTicketCocina = status;
        await _db.SaveChangesAsync();
        return Ok(new Response<bool> { Data = true, isSuccess = true });
    }

    [HttpPut("ChangeItemStatus/{id}/{status}")]
    public async Task<IActionResult> ChangeItemStatus(int id, int status)
    {
        var item = await _db.TicketDetalles.FindAsync(id);
        if (item == null) return NotFound();
        item.IdEstadoItemKDS = status;
        await _db.SaveChangesAsync();
        return Ok(new Response<bool> { Data = true, isSuccess = true });
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<int>>> InsertAsync([FromBody] TicketCocinaDTO dto)
    {
        var response = await _ticketCocinaApplication.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<TicketCocinaDTO>>>> GetAllAsync()
    {
        var response = await _ticketCocinaApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<TicketCocinaDTO>>> GetByIdAsync(int id)
    {
        var response = await _ticketCocinaApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] TicketCocinaDTO dto)
    {
        var response = await _ticketCocinaApplication.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _ticketCocinaApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<TicketCocinaDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _ticketCocinaApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _ticketCocinaApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}

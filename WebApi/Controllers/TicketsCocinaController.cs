using Common;
using DTO.TicketCocina;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TicketsCocinaController : ControllerBase
{
    private readonly ITicketCocinaApplication _ticketCocinaApplication;
    private readonly ApplicationDbContext _db;
    private readonly IHubContext<KdsHub> _kdsHub;

    public TicketsCocinaController(ITicketCocinaApplication ticketCocinaApplication, ApplicationDbContext db, IHubContext<KdsHub> kdsHub)
    {
        _ticketCocinaApplication = ticketCocinaApplication;
        _db = db;
        _kdsHub = kdsHub;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<Guid>> Insert([FromBody] TicketCocinaDTO dto)
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

    [HttpGet("GetById/{id:guid}")]
    public ActionResult<Response<TicketCocinaDTO>> GetById(Guid id)
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

    [HttpDelete("Delete/{id:guid}")]
    public ActionResult<Response<bool>> Delete(Guid id)
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
    public async Task<IActionResult> GetKdsBoard([FromQuery] int? idEstacion = null)
    {
        // Custom query to get all active tickets with their details, product names, and modifiers
        var query = _db.TicketsCocina
            .Where(t => t.IdEstadoTicketCocina == 1 || t.IdEstadoTicketCocina == 2); // Pending or Preparing

        if (idEstacion.HasValue && idEstacion.Value > 0)
        {
            query = query.Where(t => t.IdEstacion == idEstacion.Value);
        }

        var tickets = await query
            .Select(t => new
            {
                t.Id,
                t.IdPedido,
                MesaNombre = t.Pedido.Mesa != null ? t.Pedido.Mesa.Codigo : null,
                AreaNombre = t.Pedido.Mesa != null && t.Pedido.Mesa.Area != null ? t.Pedido.Mesa.Area.Nombre : null,
                t.IdEstacion,
                EstacionNombre = t.Estacion != null ? t.Estacion.Nombre : null,
                t.IdEstadoTicketCocina,
                t.CreatedAt,
                Detalles = t.Detalles.Select(d => new
                {
                    d.Id,
                    d.IdEstadoItemKDS,
                    IsCancelado = d.DetallePedido != null && d.DetallePedido.IdEstadoPedidoDetalle == 5,
                    d.DetallePedido.Cantidad,
                    ProductoNombre = d.DetallePedido.Producto.Nombre,
                    Notas = d.DetallePedido.Notas,
                    Modificadores = d.DetallePedido.Modificadores.Select(m => m.Opcion.Nombre).ToList()
                }).ToList()
            })
            .ToListAsync();

        return Ok(new Response<object> { Data = tickets, isSuccess = true });
    }

    [HttpGet("GetKdsHistory")]
    public async Task<IActionResult> GetKdsHistory([FromQuery] int? idEstacion = null)
    {
        var query = _db.TicketsCocina
            .Where(t => t.IdEstadoTicketCocina == 3); // Completed / Despachado

        if (idEstacion.HasValue && idEstacion.Value > 0)
        {
            query = query.Where(t => t.IdEstacion == idEstacion.Value);
        }

        var tickets = await query
            .OrderByDescending(t => t.CompletadoEn ?? t.UpdatedAt)
            .Take(30)
            .Select(t => new
            {
                t.Id,
                t.IdPedido,
                MesaNombre = t.Pedido.Mesa != null ? t.Pedido.Mesa.Codigo : null,
                AreaNombre = t.Pedido.Mesa != null && t.Pedido.Mesa.Area != null ? t.Pedido.Mesa.Area.Nombre : null,
                t.IdEstacion,
                EstacionNombre = t.Estacion != null ? t.Estacion.Nombre : null,
                t.IdEstadoTicketCocina,
                t.CreatedAt,
                t.CompletadoEn,
                t.FechaRecuperacion,
                t.UsuarioRecuperacion,
                Detalles = t.Detalles.Select(d => new
                {
                    d.Id,
                    d.IdEstadoItemKDS,
                    IsCancelado = d.DetallePedido != null && d.DetallePedido.IdEstadoPedidoDetalle == 5,
                    d.DetallePedido.Cantidad,
                    ProductoNombre = d.DetallePedido.Producto.Nombre,
                    Notas = d.DetallePedido.Notas,
                    Modificadores = d.DetallePedido.Modificadores.Select(m => m.Opcion.Nombre).ToList()
                }).ToList()
            })
            .ToListAsync();

        return Ok(new Response<object> { Data = tickets, isSuccess = true });
    }

    [HttpPut("RecuperarTicket/{id:guid}")]
    public async Task<IActionResult> RecuperarTicket(Guid id)
    {
        var ticket = await _db.TicketsCocina.FindAsync(id);
        if (ticket == null) return NotFound();

        // Transición de Despachado (3) a En Preparación (2)
        ticket.IdEstadoTicketCocina = 2;
        ticket.FechaRecuperacion = DateTime.UtcNow;
        ticket.UsuarioRecuperacion = User.Identity?.Name ?? "Supervisor";
        await _db.SaveChangesAsync();

        // Notificar por SignalR a la estación y a expo
        await _kdsHub.Clients.Group($"estacion-{ticket.IdEstacion}").SendAsync("ReceiveNewTicket", ticket.Id);
        await _kdsHub.Clients.Group("expo").SendAsync("ReceiveNewTicket", ticket.Id);

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Ticket recuperado exitosamente." });
    }

    [HttpPut("UpdateEstacionRushConfig/{idEstacion}")]
    public async Task<IActionResult> UpdateEstacionRushConfig(int idEstacion, [FromQuery] int minutosAmbar = 5, [FromQuery] int minutosRojo = 10)
    {
        var estacion = await _db.EstacionesCocina.FindAsync(idEstacion);
        if (estacion == null) return NotFound();

        estacion.MinutosAmbar = minutosAmbar;
        estacion.MinutosRojo = minutosRojo;
        await _db.SaveChangesAsync();

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Configuración RUSH actualizada." });
    }

    [HttpPut("ChangeTicketStatus/{id:guid}/{status}")]
    public async Task<IActionResult> ChangeTicketStatus(Guid id, int status)
    {
        var ticket = await _db.TicketsCocina.FindAsync(id);
        if (ticket == null) return NotFound();
        ticket.IdEstadoTicketCocina = status;
        if (status == 3)
        {
            ticket.CompletadoEn = DateTime.UtcNow;

            // Verificar si todos los tickets del pedido están completados
            var ticketsPedido = await _db.TicketsCocina
                .Where(t => t.IdPedido == ticket.IdPedido && t.Id != ticket.Id)
                .ToListAsync();

            if (ticketsPedido.All(t => t.IdEstadoTicketCocina == 3))
            {
                var pedido = await _db.Pedidos.FindAsync(ticket.IdPedido);
                if (pedido != null && pedido.IdEstadoPedido < 3) // Menor que "Listo"
                {
                    pedido.IdEstadoPedido = 3; // Listo
                    pedido.UpdatedAt = DateTime.UtcNow;

                    _db.EventosPedido.Add(new Domain.Entities.EventoPedido
                    {
                        IdPedido = pedido.Id,
                        TipoEvento = "PedidoListo",
                        Payload = System.Text.Json.JsonSerializer.Serialize(new { IdPedido = pedido.Id, ListoEn = DateTime.UtcNow }),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = User.Identity?.Name ?? "KDS"
                    });
                }
            }
        }
        await _db.SaveChangesAsync();

        await _kdsHub.Clients.Group($"estacion-{ticket.IdEstacion}").SendAsync("ReceiveNewTicket", ticket.Id);
        await _kdsHub.Clients.Group("expo").SendAsync("ReceiveNewTicket", ticket.Id);
        if (status == 3)
        {
            await _kdsHub.Clients.All.SendAsync("OrderReadyForDispatch", ticket.IdPedido);
        }

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
    public async Task<ActionResult<Response<Guid>>> InsertAsync([FromBody] TicketCocinaDTO dto)
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

    [HttpGet("GetByIdAsync/{id:guid}")]
    public async Task<ActionResult<Response<TicketCocinaDTO>>> GetByIdAsync(Guid id)
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

    [HttpDelete("DeleteAsync/{id:guid}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(Guid id)
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

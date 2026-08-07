using Common;
using DTO.Pedido;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Hubs;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly IPedidoApplication _pedidoApplication;
    private readonly ITicketCocinaApplication _ticketCocinaApplication;
    private readonly ITicketDetalleApplication _ticketDetalleApplication;
    private readonly IHubContext<KdsHub> _kdsHub;

    public PedidosController(
        IPedidoApplication pedidoApplication, 
        ITicketCocinaApplication ticketCocinaApplication,
        ITicketDetalleApplication ticketDetalleApplication,
        IHubContext<KdsHub> kdsHub)
    {
        _pedidoApplication = pedidoApplication;
        _ticketCocinaApplication = ticketCocinaApplication;
        _ticketDetalleApplication = ticketDetalleApplication;
        _kdsHub = kdsHub;
    }

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] PedidoDTO pedido)
    {
        var response = _pedidoApplication.Insert(pedido);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<PedidoDTO>>> GetAll()
    {
        var response = _pedidoApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<PedidoDTO>> GetById(int id)
    {
        var response = _pedidoApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] PedidoDTO pedido)
    {
        var response = _pedidoApplication.Update(pedido);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _pedidoApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<PedidoDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _pedidoApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _pedidoApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] PedidoDTO pedido)
    {
        var response = await _pedidoApplication.InsertAsync(pedido);
        return Ok(response);
    }

    [HttpPost("InsertConDetallesAsync")]
    public async Task<ActionResult<Response<int>>> InsertConDetallesAsync([FromBody] CrearPedidoRequestDTO pedido)
    {
        var response = await _pedidoApplication.InsertConDetallesAsync(pedido);
        
        // KDS: Create ticket if successful
        if (response.isSuccess && response.Data > 0)
        {
            var ticketDto = new DTO.TicketCocina.TicketCocinaDTO
            {
                IdPedido = response.Data,
                IdEstacion = 1, // Require kitchen station logic later
                IdEstadoTicketCocina = 1, // Pending
                Activo = true
            };
            var ticketRes = await _ticketCocinaApplication.InsertAsync(ticketDto);
            
            if (ticketRes.isSuccess && ticketRes.Data > 0)
            {
                // Fetch the inserted Pedido details directly from DB to get the generated IDs
                var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>();
                var insertedDetalles = await dbContext.PedidoDetalles.Where(d => d.IdPedido == response.Data).ToListAsync();

                foreach (var detalle in insertedDetalles)
                {
                    var detalleDto = new DTO.TicketDetalle.TicketDetalleDTO
                    {
                        IdTicket = ticketRes.Data,
                        IdDetalle = detalle.Id, 
                        IdEstadoItemKDS = 1, // Pending
                        Activo = true
                    };
                    await _ticketDetalleApplication.InsertAsync(detalleDto);
                }
                
                // NOTIFY KDS VIA SIGNALR
                await _kdsHub.Clients.All.SendAsync("ReceiveNewTicket", ticketRes.Data);
            }
        }

        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<PedidoDTO>>>> GetAllAsync()
    {
        var response = await _pedidoApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<PedidoDTO>>> GetByIdAsync(int id)
    {
        var response = await _pedidoApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] PedidoDTO pedido)
    {
        var response = await _pedidoApplication.UpdateAsync(pedido);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _pedidoApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<PedidoDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _pedidoApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _pedidoApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}

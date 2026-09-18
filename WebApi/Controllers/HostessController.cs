using Common;
using Domain.Entities;
using DTO.Hostess;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HostessController : ControllerBase
{
    private readonly IHostessService _hostessService;
    private readonly IHubContext<MesasHub>? _mesasHub;

    public HostessController(IHostessService hostessService, IHubContext<MesasHub>? mesasHub = null)
    {
        _hostessService = hostessService;
        _mesasHub = mesasHub;
    }

    #region Waitlist (Fila de Espera Digital)

    [HttpGet("waitlist")]
    public async Task<ActionResult<Response<IEnumerable<FilaEsperaItemDTO>>>> GetWaitlist([FromQuery] int idSucursal)
    {
        var response = new Response<IEnumerable<FilaEsperaItemDTO>>();
        try
        {
            var list = await _hostessService.GetWaitlistAsync(idSucursal);
            response.Data = list;
            response.isSuccess = true;
            response.Message = "Fila de espera consultada con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al consultar fila de espera: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPost("waitlist")]
    public async Task<ActionResult<Response<FilaEsperaItemDTO>>> RegistrarEnWaitlist([FromBody] RegistrarWaitlistDTO dto)
    {
        var response = new Response<FilaEsperaItemDTO>();
        try
        {
            if (string.IsNullOrWhiteSpace(dto.NombreCliente) || string.IsNullOrWhiteSpace(dto.TelefonoCliente))
            {
                response.isSuccess = false;
                response.Message = "El nombre y teléfono del cliente son requeridos.";
                return BadRequest(response);
            }

            var item = await _hostessService.RegistrarEnWaitlistAsync(dto);
            response.Data = item;
            response.isSuccess = true;
            response.Message = "Cliente registrado en fila de espera con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al registrar en fila de espera: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPut("waitlist/{id}/notificar")]
    public async Task<ActionResult<Response<FilaEsperaItemDTO>>> NotificarWaitlist(int id)
    {
        var response = new Response<FilaEsperaItemDTO>();
        try
        {
            var item = await _hostessService.NotificarWaitlistAsync(id);
            response.Data = item;
            response.isSuccess = true;
            response.Message = "Comensal marcado como notificado vía WhatsApp.";
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            return NotFound(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al notificar comensal: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPut("waitlist/{id}/sentar")]
    public async Task<ActionResult<Response<bool>>> SentarWaitlist(int id, [FromBody] SentarWaitlistDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            if (dto.IdMesa <= 0)
            {
                response.isSuccess = false;
                response.Message = "Debe especificar una mesa válida para sentar al comensal.";
                return BadRequest(response);
            }

            var ok = await _hostessService.SentarWaitlistAsync(id, dto);
            if (ok && _mesasHub != null)
            {
                // Notificar en tiempo real a todas las pantallas (POS, KDS, Hostess)
                await _mesasHub.Clients.All.SendAsync("MesaEstadoActualizado", new
                {
                    idMesa = dto.IdMesa,
                    idEstadoMesa = EstadosMesaConst.Ocupada,
                    timestamp = DateTime.UtcNow
                });
            }

            response.Data = ok;
            response.isSuccess = true;
            response.Message = "Comensal sentado con éxito. Mesa marcada como ocupada.";
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            return NotFound(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al sentar comensal: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPut("waitlist/{id}/cancelar")]
    public async Task<ActionResult<Response<bool>>> CancelarWaitlist(int id)
    {
        var response = new Response<bool>();
        try
        {
            var ok = await _hostessService.CancelarWaitlistAsync(id);
            response.Data = ok;
            response.isSuccess = ok;
            response.Message = ok ? "Turno de espera cancelado con éxito." : "Registro no encontrado.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al cancelar turno: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Reservaciones

    [HttpGet("reservas")]
    public async Task<ActionResult<Response<IEnumerable<ReservaMesaDTO>>>> GetReservas(
        [FromQuery] int idSucursal,
        [FromQuery] DateTime? fecha = null)
    {
        var response = new Response<IEnumerable<ReservaMesaDTO>>();
        try
        {
            var targetFecha = fecha ?? DateTime.UtcNow;
            var list = await _hostessService.GetReservasAsync(idSucursal, targetFecha);
            response.Data = list;
            response.isSuccess = true;
            response.Message = "Reservaciones consultadas con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al consultar reservaciones: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPost("reservas")]
    public async Task<ActionResult<Response<ReservaMesaDTO>>> CrearReserva([FromBody] CrearReservaDTO dto)
    {
        var response = new Response<ReservaMesaDTO>();
        try
        {
            if (string.IsNullOrWhiteSpace(dto.NombreCliente) || string.IsNullOrWhiteSpace(dto.TelefonoCliente))
            {
                response.isSuccess = false;
                response.Message = "El nombre y teléfono del cliente son requeridos.";
                return BadRequest(response);
            }

            var reserva = await _hostessService.CrearReservaAsync(dto);

            if (dto.IdMesa.HasValue && _mesasHub != null && (dto.FechaHoraReserva - DateTime.UtcNow).TotalMinutes <= 60)
            {
                await _mesasHub.Clients.All.SendAsync("MesaEstadoActualizado", new
                {
                    idMesa = dto.IdMesa.Value,
                    idEstadoMesa = EstadosMesaConst.Reservada,
                    timestamp = DateTime.UtcNow
                });
            }

            response.Data = reserva;
            response.isSuccess = true;
            response.Message = "Reservación creada con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al registrar reservación: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPut("reservas/{id}/confirmar-llegada")]
    public async Task<ActionResult<Response<bool>>> ConfirmarLlegadaReserva(int id, [FromBody] ConfirmarLlegadaReservaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var ok = await _hostessService.ConfirmarLlegadaReservaAsync(id, dto);

            if (ok && dto.IdMesa.HasValue && _mesasHub != null)
            {
                await _mesasHub.Clients.All.SendAsync("MesaEstadoActualizado", new
                {
                    idMesa = dto.IdMesa.Value,
                    idEstadoMesa = EstadosMesaConst.Ocupada,
                    timestamp = DateTime.UtcNow
                });
            }

            response.Data = ok;
            response.isSuccess = true;
            response.Message = "Llegada de reservación confirmada y mesa asignada.";
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            response.isSuccess = false;
            response.Message = ex.Message;
            return NotFound(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al confirmar llegada: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPut("reservas/{id}/cancelar")]
    public async Task<ActionResult<Response<bool>>> CancelarReserva(int id, [FromQuery] string? motivo = null)
    {
        var response = new Response<bool>();
        try
        {
            var ok = await _hostessService.CancelarReservaAsync(id, motivo);
            response.Data = ok;
            response.isSuccess = ok;
            response.Message = ok ? "Reservación cancelada." : "Reservación no encontrada.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al cancelar reservación: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Dashboard Resumen

    [HttpGet("dashboard-summary")]
    public async Task<ActionResult<Response<HostessDashboardSummaryDTO>>> GetDashboardSummary([FromQuery] int idSucursal)
    {
        var response = new Response<HostessDashboardSummaryDTO>();
        try
        {
            var summary = await _hostessService.GetSummaryAsync(idSucursal);
            response.Data = summary;
            response.isSuccess = true;
            response.Message = "Resumen de Hostess obtenido con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al obtener resumen de Hostess: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion
}

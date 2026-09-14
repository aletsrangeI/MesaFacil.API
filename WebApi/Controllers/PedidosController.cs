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

    [HttpGet("GetById/{id:guid}")]
    public ActionResult<Response<PedidoDTO>> GetById(Guid id)
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

    [HttpDelete("Delete/{id:guid}")]
    public ActionResult<Response<bool>> Delete(Guid id)
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
    public async Task<ActionResult<Response<Guid>>> InsertConDetallesAsync([FromBody] CrearPedidoRequestDTO pedido)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>();
        if (dbContext == null)
        {
            return StatusCode(500, new Response<int> { isSuccess = false, Message = "Contexto de base de datos no disponible." });
        }

        // ITEM 6: Idempotency Check (Fast-path if already executed)
        if (!string.IsNullOrEmpty(pedido.IdempotencyKey))
        {
            var existingPedido = await dbContext.Pedidos
                .Where(p => p.IdempotencyKey == pedido.IdempotencyKey)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            if (existingPedido != Guid.Empty)
            {
                return Ok(new Response<Guid> { isSuccess = true, Data = existingPedido, Message = "Pedido procesado previamente." });
            }
        }

        // Lista de notificaciones SignalR a despachar solo tras el commit exitoso
        var pendingKdsNotifications = new List<(int IdEstacion, Guid IdTicket)>();
        Response<Guid> response;

        await using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                // ITEM 7: Atomic Table Availability Validation
                if (pedido.IdMesa.HasValue)
                {
                    var mesa = await dbContext.Mesas.FirstOrDefaultAsync(m => m.Id == pedido.IdMesa.Value);
                    if (mesa != null)
                    {
                        var estadoDisponible = await dbContext.CatEstadosMesa
                            .Where(e => EF.Functions.ILike(e.Descripcion, "%disponible%"))
                            .Select(e => e.Id)
                            .FirstOrDefaultAsync();
                        
                        int targetEstado = estadoDisponible > 0 ? estadoDisponible : 1;

                        if (mesa.IdEstadoMesa != targetEstado)
                        {
                            await transaction.RollbackAsync();
                            return Conflict(new { isSuccess = false, message = $"La mesa {mesa.Codigo} ya está ocupada por otro pedido. Por favor selecciona otra mesa." });
                        }

                        // Bloquear atómicamente la mesa marcándola como Ocupada
                        var estadoOcupada = await dbContext.CatEstadosMesa
                            .Where(e => EF.Functions.ILike(e.Descripcion, "%ocupada%"))
                            .Select(e => e.Id)
                            .FirstOrDefaultAsync();

                        mesa.IdEstadoMesa = estadoOcupada > 0 ? estadoOcupada : 2;
                        mesa.UpdatedAt = DateTime.UtcNow;
                    }
                }

                response = await _pedidoApplication.InsertConDetallesAsync(pedido);

                if (!response.isSuccess || response.Data == Guid.Empty)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(response);
                }

                // Auditoría de EventoPedido
                var evento = new Domain.Entities.EventoPedido
                {
                    IdPedido = response.Data,
                    IdUsuario = ObtenerIdUsuarioActual(),
                    TipoEvento = "PedidoCreado",
                    Payload = System.Text.Json.JsonSerializer.Serialize(new { 
                        IdMesa = pedido.IdMesa, 
                        IdTipoPedido = pedido.IdTipoPedido,
                        TotalItems = pedido.Detalles.Count 
                    })
                };
                dbContext.EventosPedido.Add(evento);
                await dbContext.SaveChangesAsync();

                // Creación de tickets de cocina agrupados por estación física
                var insertedDetalles = await dbContext.PedidoDetalles
                    .Include(d => d.Producto)
                        .ThenInclude(p => p.EstacionCocina)
                    .Where(d => d.IdPedido == response.Data)
                    .ToListAsync();

                var estacionesSucursal = await dbContext.EstacionesCocina
                    .Where(e => e.IdSucursal == pedido.IdSucursal)
                    .ToListAsync();
                var estacionDefaultId = estacionesSucursal.FirstOrDefault()?.Id ?? 1;

                int ResolverEstacionFisica(Domain.Entities.Producto? p)
                {
                    if (p == null || !p.IdEstacionCocina.HasValue) return estacionDefaultId;
                    var matchId = estacionesSucursal.FirstOrDefault(e => e.Id == p.IdEstacionCocina.Value);
                    if (matchId != null) return matchId.Id;
                    if (p.EstacionCocina?.Descripcion != null)
                    {
                        var matchNombre = estacionesSucursal.FirstOrDefault(e =>
                            e.Nombre != null && (
                                e.Nombre.Contains(p.EstacionCocina.Descripcion, StringComparison.OrdinalIgnoreCase) ||
                                p.EstacionCocina.Descripcion.Contains(e.Nombre, StringComparison.OrdinalIgnoreCase)
                            ));
                        if (matchNombre != null) return matchNombre.Id;
                    }
                    return estacionDefaultId;
                }

                var stationGroups = insertedDetalles
                    .GroupBy(d => ResolverEstacionFisica(d.Producto))
                    .ToList();

                foreach (var group in stationGroups)
                {
                    var ticketDto = new DTO.TicketCocina.TicketCocinaDTO
                    {
                        IdPedido = response.Data,
                        IdEstacion = group.Key,
                        IdEstadoTicketCocina = 1, // Pending
                        Activo = true
                    };
                    var ticketRes = await _ticketCocinaApplication.InsertAsync(ticketDto);
                    
                    if (ticketRes.isSuccess && ticketRes.Data != Guid.Empty)
                    {
                        foreach (var detalle in group)
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

                        pendingKdsNotifications.Add((group.Key, ticketRes.Data));
                    }
                }

                // Confirmar transacción de manera atómica
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new Response<Guid> { isSuccess = false, Message = $"Error al procesar el pedido: {ex.Message}" });
            }
        }

        // DESPACHO SEGURO DE SEÑAL KDS (Solo después del commit)
        foreach (var notif in pendingKdsNotifications)
        {
            await _kdsHub.Clients.Group($"estacion-{notif.IdEstacion}").SendAsync("ReceiveNewTicket", notif.IdTicket);
            await _kdsHub.Clients.Group("expo").SendAsync("ReceiveNewTicket", notif.IdTicket);
        }

        return Ok(response);
    }

    private int? ObtenerIdUsuarioActual()
    {
        var claim = User.FindFirst("idUsuario") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<PedidoDTO>>>> GetAllAsync()
    {
        var response = await _pedidoApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id:guid}")]
    public async Task<ActionResult<Response<PedidoDTO>>> GetByIdAsync(Guid id)
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

    [HttpDelete("DeleteAsync/{id:guid}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(Guid id)
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

    // =====================================================================
    // SPRINT: COMANDA ABIERTA
    // =====================================================================

    [HttpGet("GetPedidoActivoByMesa/{idMesa}")]
    public async Task<ActionResult<Response<object>>> GetPedidoActivoByMesa(int idMesa)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;

        // Obtener IDs de estados activos (no cerrado, no cancelado)
        var estadosInactivos = await dbContext.CatEstadosPedido
            .Where(e => EF.Functions.ILike(e.Descripcion, "%cerrado%")
                     || EF.Functions.ILike(e.Descripcion, "%cancelado%"))
            .Select(e => e.Id)
            .ToListAsync();

        var pedido = await dbContext.Pedidos
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Modificadores)
            .Where(p => p.IdMesa == idMesa &&
                        (estadosInactivos.Any()
                            ? !estadosInactivos.Contains(p.IdEstadoPedido)
                            : p.IdEstadoPedido != 3 && p.IdEstadoPedido != 4))
            .Select(p => new
            {
                p.Id,
                p.IdMesa,
                p.IdEstadoPedido,
                p.AbiertoEn,
                p.Personas,
                p.Notas,
                p.IdTipoPedido,
                Detalles = p.Detalles.Select(d => new
                {
                    d.Id,
                    d.IdProducto,
                    d.IdVariante,
                    d.ProductoNombre,
                    d.VarianteNombre,
                    d.Cantidad,
                    d.PrecioUnitario,
                    d.TasaImpuesto,
                    d.MontoImpuesto,
                    d.Notas,
                    d.IdEstadoPedidoDetalle,
                    d.Cancelado,
                    Modificadores = d.Modificadores.Select(m => new
                    {
                        m.IdOpcion,
                        m.OpcionNombre,
                        m.PrecioExtra
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (pedido == null)
            return Ok(new Response<object> { Data = null, isSuccess = true, Message = "Sin pedido activo" });

        return Ok(new Response<object> { Data = pedido, isSuccess = true });
    }

    [HttpPost("AgregarDetalles/{idPedido:guid}")]
    public async Task<ActionResult<Response<bool>>> AgregarDetalles(Guid idPedido, [FromBody] List<CrearPedidoDetalleDTO> detalles)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;

        // Verificar que el pedido existe y está activo
        var estadosInactivos = await dbContext.CatEstadosPedido
            .Where(e => EF.Functions.ILike(e.Descripcion, "%cerrado%")
                     || EF.Functions.ILike(e.Descripcion, "%cancelado%"))
            .Select(e => e.Id)
            .ToListAsync();

        var pedido = await dbContext.Pedidos.FirstOrDefaultAsync(p =>
            p.Id == idPedido &&
            (estadosInactivos.Any()
                ? !estadosInactivos.Contains(p.IdEstadoPedido)
                : p.IdEstadoPedido != 3 && p.IdEstadoPedido != 4));

        if (pedido == null)
            return NotFound(new Response<bool> { isSuccess = false, Message = "Pedido no encontrado o no está activo." });

        var nuevosDetalles = new List<Domain.Entities.PedidoDetalle>();

        foreach (var dto in detalles)
        {
            var detalle = new Domain.Entities.PedidoDetalle
            {
                IdPedido        = idPedido,
                IdProducto      = dto.IdProducto,
                IdVariante      = dto.IdVariante,
                ProductoNombre  = dto.ProductoNombre,
                VarianteNombre  = dto.VarianteNombre,
                Cantidad        = dto.Cantidad,
                PrecioUnitario  = dto.PrecioUnitario,
                IdImpuesto      = dto.IdImpuesto,
                TasaImpuesto    = dto.TasaImpuesto,
                MontoImpuesto   = dto.MontoImpuesto,
                Notas           = dto.Notas,
                IdEstadoPedidoDetalle = dto.IdEstadoPedidoDetalle
            };

            // Modificadores
            foreach (var idOpcion in dto.OpcionesModificador)
            {
                var opcion = await dbContext.OpcionesModificador.FindAsync(idOpcion);
                if (opcion != null)
                {
                    detalle.Modificadores.Add(new Domain.Entities.PedidoModificador
                    {
                        IdOpcion    = idOpcion,
                        OpcionNombre = opcion.Nombre ?? string.Empty,
                        PrecioExtra = opcion.PrecioExtra
                    });
                }
            }

            dbContext.PedidoDetalles.Add(detalle);
            nuevosDetalles.Add(detalle);
        }

        await dbContext.SaveChangesAsync();

        // Recargar los detalles con el producto para agrupar por estación
        var idsNuevos = nuevosDetalles.Select(d => d.Id).ToList();
        var detallesConProducto = await dbContext.PedidoDetalles
            .Include(d => d.Producto)
                .ThenInclude(p => p.EstacionCocina)
            .Where(d => idsNuevos.Contains(d.Id))
            .ToListAsync();

        var estacionesSucursal = await dbContext.EstacionesCocina
            .Where(e => e.IdSucursal == pedido.IdSucursal)
            .ToListAsync();
        var estacionDefaultId = estacionesSucursal.FirstOrDefault()?.Id ?? 1;

        int ResolverEstacionFisica(Domain.Entities.Producto? p)
        {
            if (p == null || !p.IdEstacionCocina.HasValue) return estacionDefaultId;
            var matchId = estacionesSucursal.FirstOrDefault(e => e.Id == p.IdEstacionCocina.Value);
            if (matchId != null) return matchId.Id;
            if (p.EstacionCocina?.Descripcion != null)
            {
                var matchNombre = estacionesSucursal.FirstOrDefault(e =>
                    e.Nombre != null && (
                        e.Nombre.Contains(p.EstacionCocina.Descripcion, StringComparison.OrdinalIgnoreCase) ||
                        p.EstacionCocina.Descripcion.Contains(e.Nombre, StringComparison.OrdinalIgnoreCase)
                    ));
                if (matchNombre != null) return matchNombre.Id;
            }
            return estacionDefaultId;
        }

        var gruposEstacion = detallesConProducto
            .GroupBy(d => ResolverEstacionFisica(d.Producto))
            .ToList();

        foreach (var grupo in gruposEstacion)
        {
            int idEstacion = grupo.Key;

            // Buscar ticket activo para este pedido y estación (estado 1 = pendiente, 2 = en progreso)
            var ticketExistente = await dbContext.TicketsCocina
                .FirstOrDefaultAsync(t =>
                    t.IdPedido == idPedido &&
                    t.IdEstacion == idEstacion &&
                    (t.IdEstadoTicketCocina == 1 || t.IdEstadoTicketCocina == 2));

            Guid ticketId;

            if (ticketExistente != null)
            {
                ticketId = ticketExistente.Id;
                foreach (var det in grupo)
                {
                    dbContext.TicketDetalles.Add(new Domain.Entities.TicketDetalle
                    {
                        IdTicket        = ticketId,
                        IdDetalle       = det.Id,
                        IdEstadoItemKDS = 1 // Pendiente
                    });
                }
            }
            else
            {
                var nuevoTicket = new Domain.Entities.TicketCocina
                {
                    IdPedido           = idPedido,
                    IdEstacion         = idEstacion,
                    IdEstadoTicketCocina = 1 // Pendiente
                };
                dbContext.TicketsCocina.Add(nuevoTicket);
                await dbContext.SaveChangesAsync();
                ticketId = nuevoTicket.Id;

                foreach (var det in grupo)
                {
                    dbContext.TicketDetalles.Add(new Domain.Entities.TicketDetalle
                    {
                        IdTicket        = ticketId,
                        IdDetalle       = det.Id,
                        IdEstadoItemKDS = 1 // Pendiente
                    });
                }
            }

            await dbContext.SaveChangesAsync();

            // Notificar KDS vía SignalR
            await _kdsHub.Clients.Group($"estacion-{idEstacion}").SendAsync("ReceiveNewTicket", ticketId);
            await _kdsHub.Clients.Group("expo").SendAsync("ReceiveNewTicket", ticketId);
        }

        // Registrar evento
        dbContext.EventosPedido.Add(new Domain.Entities.EventoPedido
        {
            IdPedido   = idPedido,
            IdUsuario  = ObtenerIdUsuarioActual(),
            TipoEvento = "DetallesAgregados",
            Payload    = System.Text.Json.JsonSerializer.Serialize(new { TotalItems = detalles.Count })
        });
        await dbContext.SaveChangesAsync();

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Ítems agregados al pedido." });
    }

    [HttpPut("CancelarDetalle/{idDetalle:guid}")]
    public async Task<ActionResult<Response<bool>>> CancelarDetalle(Guid idDetalle, [FromQuery] string? motivo = null)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;

        // Buscar el detalle e incluir el pedido padre
        var detalle = await dbContext.PedidoDetalles
            .Include(d => d.Pedido)
            .FirstOrDefaultAsync(d => d.Id == idDetalle);

        if (detalle == null)
            return NotFound(new Response<bool> { isSuccess = false, Message = "Detalle no encontrado." });

        // Marcar como cancelado
        detalle.Cancelado           = true;
        detalle.MotivoCancelacion   = motivo;
        detalle.CanceladoEn         = DateTime.UtcNow;
        detalle.CanceladoPor        = ObtenerIdUsuarioActual();

        // Obtener estado "cancelado" del catálogo de estados de detalle
        var estadoCanceladoId = await dbContext.CatEstadosPedidoDetalle
            .Where(e => EF.Functions.ILike(e.Descripcion, "%cancelado%"))
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        detalle.IdEstadoPedidoDetalle = estadoCanceladoId > 0 ? estadoCanceladoId : 5;

        // Cancelar el ítem en KDS si existe y no está ya completado (IdEstadoItemKDS != 3)
        var ticketDetalle = await dbContext.TicketDetalles
            .Include(td => td.Ticket)
            .FirstOrDefaultAsync(td => td.IdDetalle == idDetalle && td.IdEstadoItemKDS != 3);

        Guid? ticketIdKds = null;
        int? idEstacionKds = null;

        if (ticketDetalle != null)
        {
            ticketIdKds    = ticketDetalle.IdTicket;
            idEstacionKds  = ticketDetalle.Ticket?.IdEstacion;
            ticketDetalle.IdEstadoItemKDS = 5; // Cancelado en KDS
        }

        await dbContext.SaveChangesAsync();

        // Notificar KDS vía SignalR si había ticket activo
        if (ticketIdKds.HasValue)
        {
            if (idEstacionKds.HasValue)
                await _kdsHub.Clients.Group($"estacion-{idEstacionKds.Value}").SendAsync("ReceiveNewTicket", ticketIdKds.Value);
            await _kdsHub.Clients.Group("expo").SendAsync("ReceiveNewTicket", ticketIdKds.Value);
        }

        // Registrar evento
        dbContext.EventosPedido.Add(new Domain.Entities.EventoPedido
        {
            IdPedido   = detalle.IdPedido,
            IdUsuario  = ObtenerIdUsuarioActual(),
            TipoEvento = "ItemCancelado",
            Payload    = System.Text.Json.JsonSerializer.Serialize(new { IdDetalle = idDetalle, Motivo = motivo })
        });
        await dbContext.SaveChangesAsync();

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Ítem cancelado." });
    }

    [HttpGet("{idPedido:guid}/Eventos")]
    public async Task<ActionResult<Response<IEnumerable<DTO.EventoPedido.EventoPedidoHistorialDTO>>>> GetEventosByPedido(Guid idPedido)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;

        var eventos = await dbContext.EventosPedido
            .Include(e => e.Usuario)
            .Where(e => e.IdPedido == idPedido)
            .OrderBy(e => e.CreatedAt)
            .Select(e => new DTO.EventoPedido.EventoPedidoHistorialDTO
            {
                Id = e.Id,
                IdPedido = e.IdPedido,
                IdUsuario = e.IdUsuario,
                UsuarioNombre = e.Usuario != null ? (e.Usuario.NombreCompleto ?? e.Usuario.Correo) : null,
                TipoEvento = e.TipoEvento,
                Payload = e.Payload,
                FechaUtc = e.CreatedAt
            })
            .ToListAsync();

        return Ok(new Response<IEnumerable<DTO.EventoPedido.EventoPedidoHistorialDTO>>
        {
            Data = eventos,
            isSuccess = true,
            Message = "Historial de eventos obtenido correctamente."
        });
    }

    // =====================================================================
    // SPEC 012: DELIVERY Y DESPACHO EN MOSTRADOR
    // =====================================================================

    [HttpGet("DeliveryQueue")]
    public async Task<ActionResult<Response<IEnumerable<DTO.Delivery.DeliveryQueueItemDTO>>>> GetDeliveryQueue([FromQuery] int? idSucursal = null)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;

        // Pedidos sin mesa o que sean explícitamente "Para Llevar" o "Delivery"
        var query = dbContext.Pedidos
            .Include(p => p.TipoPedido)
            .Include(p => p.EstadoPedido)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Variante)
            .Include(p => p.Detalles)
                .ThenInclude(d => d.Modificadores)
                    .ThenInclude(m => m.Opcion)
            .Include(p => p.Cuentas)
            .Where(p => !p.TipoPedido.IsComedor || p.IdMesa == null);

        if (idSucursal.HasValue && idSucursal.Value > 0)
        {
            query = query.Where(p => p.IdSucursal == idSucursal.Value);
        }

        // Filtramos pedidos de las últimas 16 horas o que no estén completados
        var limiteRecientes = DateTime.UtcNow.AddHours(-16);
        query = query.Where(p => p.AbiertoEn >= limiteRecientes || p.IdEstadoPedido < 5);

        var pedidos = await query
            .OrderByDescending(p => p.AbiertoEn)
            .Take(100)
            .ToListAsync();

        var ahora = DateTime.UtcNow;

        var result = pedidos.Select(p =>
        {
            var cuentas = p.Cuentas.Where(c => c.IsActive).ToList();
            var totalPedido = cuentas.Any() ? cuentas.Sum(c => c.Total) : p.Detalles.Where(d => d.IsActive && !d.Cancelado).Sum(d => d.Cantidad * d.PrecioUnitario);
            var estaPagado = cuentas.Any() && cuentas.All(c => c.IdEstadoCuenta == 1); // 1: Pagada

            DateTime fechaReferencia = p.EntregadoEn ?? p.DespachadoEn ?? p.AbiertoEn;
            int minutos = (int)(ahora - fechaReferencia).TotalMinutes;

            return new DTO.Delivery.DeliveryQueueItemDTO
            {
                IdPedido = p.Id,
                Folio = $"#{p.Id}",
                TipoPedido = p.TipoPedido?.Descripcion ?? (p.IdMesa == null ? "Para Llevar" : "Comedor"),
                IsComedor = p.TipoPedido?.IsComedor ?? false,
                CanalOrigen = p.CanalOrigen ?? "POS",
                IdExterno = p.IdExterno,
                ClienteNombre = p.NombreClienteDelivery ?? p.Notas,
                ClienteTelefono = p.TelefonoDelivery,
                DireccionEntrega = p.DireccionEntrega,
                NombreRepartidor = p.NombreRepartidor,
                TelefonoRepartidor = p.TelefonoRepartidor,
                IdEstadoPedido = p.IdEstadoPedido,
                EstadoNombre = p.EstadoPedido?.Descripcion ?? "Desconocido",
                Total = totalPedido,
                EstaPagado = estaPagado,
                AbiertoEn = p.AbiertoEn,
                ListoEn = p.UpdatedAt,
                DespachadoEn = p.DespachadoEn,
                EntregadoEn = p.EntregadoEn,
                MinutosEnEstado = minutos,
                Items = p.Detalles.Where(d => d.IsActive && !d.Cancelado).Select(d => new DTO.Delivery.DeliveryItemDetalleDTO
                {
                    IdDetalle = d.Id,
                    ProductoNombre = d.Producto?.Nombre ?? "Producto",
                    VarianteNombre = d.Variante?.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Notas = d.Notas,
                    Modificadores = d.Modificadores.Where(m => m.IsActive && m.Opcion != null).Select(m => m.Opcion.Nombre).ToList()
                }).ToList()
            };
        }).ToList();

        return Ok(new Response<IEnumerable<DTO.Delivery.DeliveryQueueItemDTO>>
        {
            Data = result,
            isSuccess = true,
            Message = "Cola de delivery y despacho obtenida correctamente."
        });
    }

    [HttpGet("DeliveryHistorial")]
    public async Task<ActionResult<Response<DTO.Delivery.ResumenHistorialDeliveryDTO>>> GetDeliveryHistorial(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] int? idSucursal,
        [FromQuery] string? canal,
        [FromQuery] int? idEstado)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;

        var fInicio = fechaInicio ?? DateTime.UtcNow.Date;
        var fFin = fechaFin ?? DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);

        var query = dbContext.Pedidos
            .AsNoTracking()
            .Include(p => p.TipoPedido)
            .Include(p => p.EstadoPedido)
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .Include(p => p.Detalles).ThenInclude(d => d.Variante)
            .Include(p => p.Detalles).ThenInclude(d => d.Modificadores).ThenInclude(m => m.Opcion)
            .Include(p => p.Cuentas)
            .Where(p => p.IsActive
                && p.AbiertoEn >= fInicio
                && p.AbiertoEn <= fFin
                && (p.IdMesa == null || (p.TipoPedido != null && !p.TipoPedido.IsComedor)));

        if (idSucursal.HasValue && idSucursal.Value > 0)
        {
            query = query.Where(p => p.IdSucursal == idSucursal.Value);
        }

        if (!string.IsNullOrEmpty(canal) && canal != "Todos")
        {
            if (canal.Equals("Para Llevar", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p => p.TipoPedido != null && EF.Functions.ILike(p.TipoPedido.Descripcion, "%llevar%"));
            }
            else
            {
                query = query.Where(p => p.CanalOrigen != null && EF.Functions.ILike(p.CanalOrigen, canal));
            }
        }

        if (idEstado.HasValue && idEstado.Value > 0)
        {
            query = query.Where(p => p.IdEstadoPedido == idEstado.Value);
        }

        var pedidosList = await query
            .OrderByDescending(p => p.AbiertoEn)
            .ToListAsync();

        var pedidoIds = pedidosList.Select(p => p.Id).ToList();

        // Cargar eventos de auditoría para estos pedidos
        var eventos = await dbContext.EventosPedido
            .AsNoTracking()
            .Where(e => e.IsActive && pedidoIds.Contains(e.IdPedido))
            .OrderBy(e => e.CreatedAt)
            .ToListAsync();

        var eventosGroup = eventos.GroupBy(e => e.IdPedido).ToDictionary(g => g.Key, g => g.ToList());

        var itemsDto = pedidosList.Select(p =>
        {
            var cuentas = p.Cuentas.Where(c => c.IsActive).ToList();
            bool estaPagado = cuentas.Any() && cuentas.All(c => c.IdEstadoCuenta == 1);
            decimal totalPedido = cuentas.Sum(c => c.Total);
            if (totalPedido <= 0)
            {
                totalPedido = p.Detalles.Where(d => d.IsActive && !d.Cancelado).Sum(d => d.Cantidad * d.PrecioUnitario);
            }

            int? minutosTotales = null;
            if (p.EntregadoEn.HasValue)
            {
                minutosTotales = (int)(p.EntregadoEn.Value - p.AbiertoEn).TotalMinutes;
            }
            else if (p.CerradoEn.HasValue)
            {
                minutosTotales = (int)(p.CerradoEn.Value - p.AbiertoEn).TotalMinutes;
            }

            // Buscar motivo de cancelación si fue cancelado / rebotado
            string? motivoCancelacion = null;
            if (eventosGroup.TryGetValue(p.Id, out var evList))
            {
                var evtRebote = evList.LastOrDefault(e => e.TipoEvento == "PedidoRebotado" || e.TipoEvento == "PedidoCancelado");
                if (evtRebote != null && !string.IsNullOrEmpty(evtRebote.Payload))
                {
                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(evtRebote.Payload);
                        if (doc.RootElement.TryGetProperty("Motivo", out var motProp))
                        {
                            motivoCancelacion = motProp.GetString();
                        }
                    }
                    catch { }
                }
            }

            var eventosDto = (eventosGroup.TryGetValue(p.Id, out var evs) ? evs : new List<Domain.Entities.EventoPedido>())
                .Select(e => new DTO.Delivery.DeliveryEventoAuditoriaDTO
                {
                    Id = e.Id,
                    TipoEvento = e.TipoEvento,
                    CreatedAt = e.CreatedAt,
                    CreatedBy = e.CreatedBy,
                    Payload = e.Payload,
                    Descripcion = e.TipoEvento switch
                    {
                        "PedidoCreado" => "Pedido creado e ingresado a preparación",
                        "PedidoListo" => "Pedido completado en cocina y listo para despacho",
                        "PedidoDespachado" => "Pedido entregado a repartidor y en camino",
                        "PedidoEntregado" => "Pedido entregado exitosamente",
                        "PedidoRebotado" => $"Pedido rebotado/cancelado ({motivoCancelacion ?? "Sin motivo"})",
                        _ => e.TipoEvento
                    }
                }).ToList();

            return new DTO.Delivery.DeliveryHistorialItemDTO
            {
                IdPedido = p.Id,
                Folio = $"#{p.Id}",
                TipoPedido = p.TipoPedido?.Descripcion ?? (p.IdMesa == null ? "Para Llevar" : "Comedor"),
                CanalOrigen = p.CanalOrigen ?? "POS",
                IdExterno = p.IdExterno,
                ClienteNombre = p.NombreClienteDelivery ?? p.Notas,
                ClienteTelefono = p.TelefonoDelivery,
                DireccionEntrega = p.DireccionEntrega,
                NombreRepartidor = p.NombreRepartidor,
                TelefonoRepartidor = p.TelefonoRepartidor,
                IdEstadoPedido = p.IdEstadoPedido,
                EstadoNombre = p.EstadoPedido?.Descripcion ?? "Desconocido",
                Total = totalPedido,
                EstaPagado = estaPagado,
                AbiertoEn = p.AbiertoEn,
                ListoEn = p.UpdatedAt,
                DespachadoEn = p.DespachadoEn,
                EntregadoEn = p.EntregadoEn,
                CerradoEn = p.CerradoEn,
                MinutosTotales = minutosTotales,
                MotivoCancelacion = motivoCancelacion,
                Items = p.Detalles.Where(d => d.IsActive && !d.Cancelado).Select(d => new DTO.Delivery.DeliveryItemDetalleDTO
                {
                    IdDetalle = d.Id,
                    ProductoNombre = d.Producto?.Nombre ?? "Producto",
                    VarianteNombre = d.Variante?.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Notas = d.Notas,
                    Modificadores = d.Modificadores.Where(m => m.IsActive && m.Opcion != null).Select(m => m.Opcion.Nombre).ToList()
                }).ToList(),
                Eventos = eventosDto
            };
        }).ToList();

        // Agregación de Métricas
        int totalPedidos = itemsDto.Count;
        decimal totalVentas = itemsDto.Where(p => p.IdEstadoPedido != 6).Sum(p => p.Total);
        int totalEntregados = itemsDto.Count(p => p.IdEstadoPedido >= 5);
        int totalRebotados = itemsDto.Count(p => p.IdEstadoPedido == 6 || !string.IsNullOrEmpty(p.MotivoCancelacion));
        int totalEnCamino = itemsDto.Count(p => p.IdEstadoPedido == 4);

        double tasaExito = totalPedidos > 0 ? Math.Round((double)totalEntregados / totalPedidos * 100, 1) : 0;

        var pedidosConTiempo = itemsDto.Where(p => p.MinutosTotales.HasValue && p.MinutosTotales.Value > 0).ToList();
        double tiempoPromedio = pedidosConTiempo.Any() ? Math.Round(pedidosConTiempo.Average(p => p.MinutosTotales!.Value), 1) : 0;

        // Desglose por canal
        var canalesGroup = itemsDto
            .GroupBy(p => p.CanalOrigen)
            .Select(g => new DTO.Delivery.CanalVentaResumenDTO
            {
                Canal = g.Key,
                CantidadPedidos = g.Count(),
                TotalVentas = g.Where(p => p.IdEstadoPedido != 6).Sum(p => p.Total),
                Porcentaje = totalVentas > 0 ? Math.Round((double)(g.Where(p => p.IdEstadoPedido != 6).Sum(p => p.Total) / totalVentas) * 100, 1) : 0
            })
            .OrderByDescending(c => c.TotalVentas)
            .ToList();

        var resumen = new DTO.Delivery.ResumenHistorialDeliveryDTO
        {
            FechaInicio = fInicio,
            FechaFin = fFin,
            TotalPedidos = totalPedidos,
            TotalVentas = totalVentas,
            TotalEntregados = totalEntregados,
            TotalRebotados = totalRebotados,
            TotalEnCamino = totalEnCamino,
            TasaExitoPorcentaje = tasaExito,
            TiempoPromedioEntregaMinutos = tiempoPromedio,
            VentasPorCanal = canalesGroup,
            Pedidos = itemsDto
        };

        return Ok(new Response<DTO.Delivery.ResumenHistorialDeliveryDTO>
        {
            Data = resumen,
            isSuccess = true,
            Message = "Historial de delivery y despachos obtenido exitosamente."
        });
    }

    [HttpPut("{id:guid}/MarcarListo")]
    public async Task<ActionResult<Response<bool>>> MarcarListo(Guid id)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;
        var pedido = await dbContext.Pedidos.FindAsync(id);
        if (pedido == null)
            return NotFound(new Response<bool> { isSuccess = false, Message = "Pedido no encontrado." });

        pedido.IdEstadoPedido = 3; // Listo
        pedido.UpdatedAt = DateTime.UtcNow;

        // Sincronizar tickets de cocina abiertos si los hubiera
        var tickets = await dbContext.TicketsCocina
            .Where(t => t.IdPedido == id && t.IdEstadoTicketCocina != 3)
            .ToListAsync();

        foreach (var t in tickets)
        {
            t.IdEstadoTicketCocina = 3;
            t.CompletadoEn = DateTime.UtcNow;
        }

        dbContext.EventosPedido.Add(new Domain.Entities.EventoPedido
        {
            IdPedido = pedido.Id,
            IdUsuario = ObtenerIdUsuarioActual(),
            TipoEvento = "PedidoListo",
            Payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                IdPedido = pedido.Id,
                ListoEn = DateTime.UtcNow,
                Manual = true
            }),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name ?? "Mostrador"
        });

        await dbContext.SaveChangesAsync();

        await _kdsHub.Clients.All.SendAsync("OrderReadyForDispatch", pedido.Id);
        await _kdsHub.Clients.Group("expo").SendAsync("ReceiveNewTicket", Guid.Empty);

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Pedido marcado como listo para despacho." });
    }

    [HttpPut("{id:guid}/Despachar")]
    public async Task<ActionResult<Response<bool>>> DespacharPedido(Guid id, [FromBody] DTO.Delivery.DespacharPedidoRequestDTO dto)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;
        var pedido = await dbContext.Pedidos.FindAsync(id);
        if (pedido == null)
            return NotFound(new Response<bool> { isSuccess = false, Message = "Pedido no encontrado." });

        var estadoEnCamino = await dbContext.CatEstadosPedido
            .Where(e => EF.Functions.ILike(e.Descripcion, "%camino%"))
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        pedido.IdEstadoPedido = estadoEnCamino > 0 ? estadoEnCamino : 4;
        pedido.NombreRepartidor = dto.NombreRepartidor ?? pedido.NombreRepartidor;
        pedido.TelefonoRepartidor = dto.TelefonoRepartidor ?? pedido.TelefonoRepartidor;
        if (!string.IsNullOrEmpty(dto.IdExterno))
        {
            pedido.IdExterno = dto.IdExterno;
        }
        pedido.DespachadoEn = DateTime.UtcNow;
        pedido.UpdatedAt = DateTime.UtcNow;

        dbContext.EventosPedido.Add(new Domain.Entities.EventoPedido
        {
            IdPedido = pedido.Id,
            IdUsuario = ObtenerIdUsuarioActual(),
            TipoEvento = "PedidoDespachado",
            Payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                IdPedido = pedido.Id,
                Repartidor = pedido.NombreRepartidor,
                DespachadoEn = pedido.DespachadoEn
            }),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name ?? "Delivery"
        });

        await dbContext.SaveChangesAsync();

        await _kdsHub.Clients.All.SendAsync("OrderDispatched", pedido.Id);

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Pedido marcado como despachado en camino." });
    }

    [HttpPut("{id:guid}/Entregar")]
    public async Task<ActionResult<Response<bool>>> EntregarPedido(Guid id, [FromBody] DTO.Delivery.EntregarPedidoRequestDTO? dto = null)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;
        var pedido = await dbContext.Pedidos
            .Include(p => p.Cuentas).ThenInclude(c => c.Pagos)
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound(new Response<bool> { isSuccess = false, Message = "Pedido no encontrado." });

        var estadoCerrado = await dbContext.CatEstadosPedido
            .Where(e => EF.Functions.ILike(e.Descripcion, "%cerrado%"))
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        pedido.EntregadoEn = DateTime.UtcNow;
        pedido.UpdatedAt = DateTime.UtcNow;

        // Auto-crear y liquidar cuenta/pago si aún no existe
        var cuentas = pedido.Cuentas.Where(c => c.IsActive).ToList();
        bool yaPagado = cuentas.Any(c => c.IdEstadoCuenta == 1 && c.Pagos.Any(pg => pg.IsActive));

        if (!yaPagado)
        {
            decimal totalPedido = pedido.Detalles.Where(d => d.IsActive && !d.Cancelado).Sum(d => d.Cantidad * d.PrecioUnitario);
            if (totalPedido > 0)
            {
                int metodoId = dto?.IdMetodoDePago ?? 1; // Default Efectivo si no se indica

                // Si es plataforma externa, mapear a su método de pago correspondiente
                if (!string.IsNullOrEmpty(pedido.CanalOrigen))
                {
                    if (pedido.CanalOrigen.Contains("uber", StringComparison.OrdinalIgnoreCase)) metodoId = 3;
                    else if (pedido.CanalOrigen.Contains("rappi", StringComparison.OrdinalIgnoreCase)) metodoId = 4;
                    else if (pedido.CanalOrigen.Contains("didi", StringComparison.OrdinalIgnoreCase)) metodoId = 5;
                }

                var cuenta = cuentas.FirstOrDefault(c => c.IdEstadoCuenta == 2) ?? new Domain.Entities.Cuenta
                {
                    IdPedido = pedido.Id,
                    Subtotal = totalPedido,
                    Total = totalPedido,
                    IdEstadoCuenta = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "Delivery"
                };

                cuenta.IdEstadoCuenta = 1; // Pagada

                if (cuenta.Id == 0)
                {
                    dbContext.Cuentas.Add(cuenta);
                    await dbContext.SaveChangesAsync();
                }

                var pago = new Domain.Entities.Pago
                {
                    IdCuenta = cuenta.Id,
                    Monto = totalPedido,
                    Propina = 0,
                    IdMetodoDePago = metodoId,
                    Referencia = pedido.IdExterno ?? pedido.CanalOrigen,
                    PagadoEn = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "Delivery",
                    Moneda = "MXN"
                };

                dbContext.Pagos.Add(pago);
            }
        }

        pedido.IdEstadoPedido = estadoCerrado > 0 ? estadoCerrado : 5;
        pedido.CerradoEn = DateTime.UtcNow;

        dbContext.EventosPedido.Add(new Domain.Entities.EventoPedido
        {
            IdPedido = pedido.Id,
            IdUsuario = ObtenerIdUsuarioActual(),
            TipoEvento = "PedidoEntregado",
            Payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                IdPedido = pedido.Id,
                EntregadoEn = pedido.EntregadoEn,
                EstaLiquidado = true
            }),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name ?? "Delivery"
        });

        await dbContext.SaveChangesAsync();

        await _kdsHub.Clients.All.SendAsync("OrderDelivered", pedido.Id);

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Pedido entregado y liquidado en caja exitosamente." });
    }

    [HttpPut("{id:guid}/Rebotar")]
    public async Task<ActionResult<Response<bool>>> RebotarPedido(Guid id, [FromBody] DTO.Delivery.RebotarPedidoRequestDTO dto)
    {
        var dbContext = HttpContext.RequestServices.GetService<Persistence.Context.ApplicationDbContext>()!;
        var pedido = await dbContext.Pedidos.Include(p => p.Cuentas).FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null)
            return NotFound(new Response<bool> { isSuccess = false, Message = "Pedido no encontrado." });

        var estadoCancelado = await dbContext.CatEstadosPedido
            .Where(e => EF.Functions.ILike(e.Descripcion, "%cancelado%"))
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        pedido.IdEstadoPedido = estadoCancelado > 0 ? estadoCancelado : 6;
        pedido.CerradoEn = DateTime.UtcNow;
        pedido.UpdatedAt = DateTime.UtcNow;

        // Cancelar cuentas si no fueron cobradas
        foreach (var c in pedido.Cuentas.Where(c => c.IsActive && c.IdEstadoCuenta != 1))
        {
            c.IdEstadoCuenta = 3; // Cancelada
        }

        dbContext.EventosPedido.Add(new Domain.Entities.EventoPedido
        {
            IdPedido = pedido.Id,
            IdUsuario = ObtenerIdUsuarioActual(),
            TipoEvento = "PedidoRebotado",
            Payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                IdPedido = pedido.Id,
                Motivo = dto.Motivo,
                RebotadoEn = DateTime.UtcNow
            }),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.Identity?.Name ?? "Delivery"
        });

        await dbContext.SaveChangesAsync();

        await _kdsHub.Clients.All.SendAsync("OrderRejected", pedido.Id);

        return Ok(new Response<bool> { Data = true, isSuccess = true, Message = "Pedido cancelado/rebotado correctamente." });
    }

    #endregion
}

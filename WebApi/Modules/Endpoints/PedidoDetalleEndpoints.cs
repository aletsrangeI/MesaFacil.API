using Common;
using DTO.PedidoDetalle;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using WebApi.Hubs;

namespace MesaFacil.API.Modules.Endpoints;

public static class PedidoDetalleEndpoints
{
    public static IEndpointRouteBuilder MapPedidoDetalleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pedidodetalle")
            .WithTags("PedidoDetalle")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (PedidoDetalleDTO dto, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Insert");

        group.MapPost("/insert-async",
                async (PedidoDetalleDTO dto, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:guid}",
                (Guid id, PedidoDetalleDTO dto, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Update");

        group.MapPut("/update-async/{id:guid}",
                async (Guid id, PedidoDetalleDTO dto, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================
        // Spec 024, criterio de aceptación #1 (Restricción Infranqueable): si el PedidoDetalle ya
        // fue enviado a cocina (existe un TicketDetalle asociado), el borrado exige el token de
        // autorización efímero (60s) emitido por POST /api/seguridad/autorizar-supervisor-pin en
        // el header "X-Authorization-Token", con claims accionProtegida="CancelarPlatilloCocina"
        // e idPedidoDetalle == {id}. Si NO fue enviado a cocina, el borrado sigue funcionando
        // exactamente igual que antes (criterio #2: cero impacto en operación normal).

        group.MapDelete("/delete/{id:guid}",
                (Guid id, string? motivo, IPedidoDetalleApplication svc, ApplicationDbContext db,
                 ISupervisorPinSecurityService pinService, IHubContext<KdsHub> kdsHub,
                 HttpRequest request, CancellationToken ct) =>
                    EjecutarBorradoProtegidoAsync(id, motivo, svc, db, pinService, kdsHub, request, ct))
            .WithName("PedidoDetalle_Delete");

        group.MapDelete("/delete-async/{id:guid}",
                (Guid id, string? motivo, IPedidoDetalleApplication svc, ApplicationDbContext db,
                 ISupervisorPinSecurityService pinService, IHubContext<KdsHub> kdsHub,
                 HttpRequest request, CancellationToken ct) =>
                    EjecutarBorradoProtegidoAsync(id, motivo, svc, db, pinService, kdsHub, request, ct))
            .WithName("PedidoDetalle_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoDetalleDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_GetAll");

        group.MapGet("/getall-async",
                async (IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoDetalleDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:guid}",
                Results<Ok<Response<PedidoDetalleDTO>>, NotFound<Response<PedidoDetalleDTO>>>
                    (Guid id, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoDetalleDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoDetalleDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "PedidoDetalle con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_GetById");

        group.MapGet("/getbyid-async/{id:guid}",
                async Task<Results<Ok<Response<PedidoDetalleDTO>>, NotFound<Response<PedidoDetalleDTO>>>>
                    (Guid id, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoDetalleDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoDetalleDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "PedidoDetalle con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoDetalleDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoDetalleDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_GetPaged_Async");

        group.MapGet("/count",
                (IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Count");

        group.MapGet("/count-async",
                async (IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Count_Async");

        return endpoints;
    }

    private const string AccionProtegidaCancelarPlatilloCocina = "CancelarPlatilloCocina";

    /// <summary>
    /// Spec 024: guardia infranqueable del borrado de PedidoDetalle. Ver comentario sobre la
    /// restricción en el registro de la ruta DELETE más arriba.
    /// </summary>
    private static async Task<IResult> EjecutarBorradoProtegidoAsync(
        Guid id,
        string? motivo,
        IPedidoDetalleApplication svc,
        ApplicationDbContext db,
        ISupervisorPinSecurityService pinService,
        IHubContext<KdsHub> kdsHub,
        HttpRequest request,
        CancellationToken ct)
    {
        var ticketDetalle = await db.TicketDetalles
            .Include(td => td.Ticket)
            .FirstOrDefaultAsync(td => td.IdDetalle == id, ct);

        var yaEnviadoACocina = ticketDetalle is not null;
        int? idUsuarioSupervisor = null;

        if (yaEnviadoACocina)
        {
            var token = request.Headers["X-Authorization-Token"].FirstOrDefault();
            var autorizado = pinService.ValidarTokenAutorizacion(token, AccionProtegidaCancelarPlatilloCocina, id, out idUsuarioSupervisor);

            if (!autorizado)
            {
                return Results.Json(
                    new Response<bool>
                    {
                        Data = false,
                        isSuccess = false,
                        Message = "Este platillo ya fue enviado a cocina. Se requiere autorización de un supervisor (PIN) para cancelarlo.",
                        Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                    },
                    statusCode: StatusCodes.Status403Forbidden);
            }
        }

        // Snapshot previo al borrado físico (el repositorio elimina la fila; necesitamos estos
        // datos para la auditoría inmutable en EventoPedido y para notificar al KDS correcto).
        var detalle = await db.PedidoDetalles.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);

        Response<bool> result = await svc.DeleteAsync(id);

        if (result.Data && yaEnviadoACocina && detalle is not null)
        {
            db.EventosPedido.Add(new Domain.Entities.EventoPedido
            {
                IdPedido = detalle.IdPedido,
                IdUsuarioSupervisor = idUsuarioSupervisor,
                TipoEvento = "PlatilloCancelado",
                MontoCancelado = detalle.PrecioUnitario * detalle.Cantidad,
                Payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    IdPedidoDetalle = id,
                    detalle.ProductoNombre,
                    Motivo = motivo
                }),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SupervisorPinSecurityService"
            });
            await db.SaveChangesAsync(ct);

            // Mismo mecanismo de notificación SignalR que ya usa TicketsCocinaController para que
            // el KDS refleje de inmediato el platillo cancelado (criterio de aceptación #3).
            if (ticketDetalle?.Ticket is not null)
            {
                await kdsHub.Clients.Group($"estacion-{ticketDetalle.Ticket.IdEstacion}").SendAsync("ReceiveNewTicket", ticketDetalle.Ticket.Id, ct);
                await kdsHub.Clients.Group("expo").SendAsync("ReceiveNewTicket", ticketDetalle.Ticket.Id, ct);
            }
        }

        return Results.Ok(result);
    }
}

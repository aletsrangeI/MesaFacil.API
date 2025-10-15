using Common;
using DTO.EventoPedido;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class EventoPedidoEndpoints
{
    public static IEndpointRouteBuilder MapEventoPedidoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/eventopedido")
            .WithTags("EventoPedido")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (EventoPedidoDTO dto, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Insert");

        group.MapPost("/insert-async",
                async (EventoPedidoDTO dto, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, EventoPedidoDTO dto, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, EventoPedidoDTO dto, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<EventoPedidoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_GetAll");

        group.MapGet("/getall-async",
                async (IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<EventoPedidoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<EventoPedidoDTO>>, NotFound<Response<EventoPedidoDTO>>>
                    (int id, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<EventoPedidoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<EventoPedidoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "EventoPedido con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<EventoPedidoDTO>>, NotFound<Response<EventoPedidoDTO>>>>
                    (int id, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<EventoPedidoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<EventoPedidoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "EventoPedido con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<EventoPedidoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<EventoPedidoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_GetPaged_Async");

        group.MapGet("/count",
                (IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Count");

        group.MapGet("/count-async",
                async (IEventoPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("EventoPedido_Count_Async");

        return endpoints;
    }
}

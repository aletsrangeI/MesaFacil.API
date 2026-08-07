using Common;
using DTO.Pedido;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class PedidoEndpoints
{
    public static IEndpointRouteBuilder MapPedidoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pedido")
            .WithTags("Pedido")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (PedidoDTO dto, IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Insert");

        group.MapPost("/insert-async",
                async (PedidoDTO dto, IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Insert_Async");

        group.MapPost("/insert-con-detalles",
                async (CrearPedidoRequestDTO dto, IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.InsertConDetallesAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Insert_Con_Detalles");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, PedidoDTO dto, IPedidoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, PedidoDTO dto, IPedidoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_GetAll");

        group.MapGet("/getall-async",
                async (IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<PedidoDTO>>, NotFound<Response<PedidoDTO>>>
                    (int id, IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Pedido con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<PedidoDTO>>, NotFound<Response<PedidoDTO>>>>
                    (int id, IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Pedido con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IPedidoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IPedidoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_GetPaged_Async");

        group.MapGet("/count",
                (IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Count");

        group.MapGet("/count-async",
                async (IPedidoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Pedido_Count_Async");

        return endpoints;
    }
}

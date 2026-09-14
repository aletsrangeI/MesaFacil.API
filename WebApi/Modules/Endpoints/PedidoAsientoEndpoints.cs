using Common;
using DTO.PedidoAsiento;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class PedidoAsientoEndpoints
{
    public static IEndpointRouteBuilder MapPedidoAsientoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pedidoasiento")
            .WithTags("PedidoAsiento")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (PedidoAsientoDTO dto, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Insert");

        group.MapPost("/insert-async",
                async (PedidoAsientoDTO dto, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:guid}",
                (Guid id, PedidoAsientoDTO dto, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Update");

        group.MapPut("/update-async/{id:guid}",
                async (Guid id, PedidoAsientoDTO dto, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:guid}",
                (Guid id, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Delete");

        group.MapDelete("/delete-async/{id:guid}",
                async (Guid id, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoAsientoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_GetAll");

        group.MapGet("/getall-async",
                async (IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoAsientoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:guid}",
                Results<Ok<Response<PedidoAsientoDTO>>, NotFound<Response<PedidoAsientoDTO>>>
                    (Guid id, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoAsientoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoAsientoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "PedidoAsiento con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_GetById");

        group.MapGet("/getbyid-async/{id:guid}",
                async Task<Results<Ok<Response<PedidoAsientoDTO>>, NotFound<Response<PedidoAsientoDTO>>>>
                    (Guid id, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoAsientoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoAsientoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "PedidoAsiento con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoAsientoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoAsientoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_GetPaged_Async");

        group.MapGet("/count",
                (IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Count");

        group.MapGet("/count-async",
                async (IPedidoAsientoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoAsiento_Count_Async");

        return endpoints;
    }
}

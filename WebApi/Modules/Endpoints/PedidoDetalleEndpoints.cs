using Common;
using DTO.PedidoDetalle;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

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

        group.MapPut("/update/{id:int}",
                (int id, PedidoDetalleDTO dto, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, PedidoDetalleDTO dto, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoDetalle_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IPedidoDetalleApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
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

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<PedidoDetalleDTO>>, NotFound<Response<PedidoDetalleDTO>>>
                    (int id, IPedidoDetalleApplication svc, CancellationToken ct) =>
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

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<PedidoDetalleDTO>>, NotFound<Response<PedidoDetalleDTO>>>>
                    (int id, IPedidoDetalleApplication svc, CancellationToken ct) =>
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
}

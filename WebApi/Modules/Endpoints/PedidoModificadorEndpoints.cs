using Common;
using DTO.PedidoModificador;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class PedidoModificadorEndpoints
{
    public static IEndpointRouteBuilder MapPedidoModificadorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pedidomodificador")
            .WithTags("PedidoModificador")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (PedidoModificadorDTO dto, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Insert");

        group.MapPost("/insert-async",
                async (PedidoModificadorDTO dto, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, PedidoModificadorDTO dto, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, PedidoModificadorDTO dto, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoModificadorDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_GetAll");

        group.MapGet("/getall-async",
                async (IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PedidoModificadorDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<PedidoModificadorDTO>>, NotFound<Response<PedidoModificadorDTO>>>
                    (int id, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoModificadorDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoModificadorDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "PedidoModificador con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<PedidoModificadorDTO>>, NotFound<Response<PedidoModificadorDTO>>>>
                    (int id, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<PedidoModificadorDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PedidoModificadorDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "PedidoModificador con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoModificadorDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PedidoModificadorDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_GetPaged_Async");

        group.MapGet("/count",
                (IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Count");

        group.MapGet("/count-async",
                async (IPedidoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("PedidoModificador_Count_Async");

        return endpoints;
    }
}

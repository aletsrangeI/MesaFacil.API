using Common;
using DTO.OpcionModificador;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class OpcionModificadorEndpoints
{
    public static IEndpointRouteBuilder MapOpcionModificadorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/opcionmodificador")
            .WithTags("OpcionModificador")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (OpcionModificadorDTO dto, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Insert");

        group.MapPost("/insert-async",
                async (OpcionModificadorDTO dto, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, OpcionModificadorDTO dto, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, OpcionModificadorDTO dto, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<OpcionModificadorDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_GetAll");

        group.MapGet("/getall-async",
                async (IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<OpcionModificadorDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<OpcionModificadorDTO>>, NotFound<Response<OpcionModificadorDTO>>>
                    (int id, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<OpcionModificadorDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<OpcionModificadorDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "OpcionModificador con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<OpcionModificadorDTO>>, NotFound<Response<OpcionModificadorDTO>>>>
                    (int id, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<OpcionModificadorDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<OpcionModificadorDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "OpcionModificador con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<OpcionModificadorDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<OpcionModificadorDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_GetPaged_Async");

        group.MapGet("/count",
                (IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Count");

        group.MapGet("/count-async",
                async (IOpcionModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("OpcionModificador_Count_Async");

        return endpoints;
    }
}

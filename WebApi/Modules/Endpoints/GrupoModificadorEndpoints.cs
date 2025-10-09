using Common;
using DTO.GrupoModificador;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class GrupoModificadorEndpoints
{
    public static IEndpointRouteBuilder MapGrupoModificadorEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/grupomodificador")
            .WithTags("GrupoModificador")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (GrupoModificadorDTO dto, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Insert");

        group.MapPost("/insert-async",
                async (GrupoModificadorDTO dto, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, GrupoModificadorDTO dto, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, GrupoModificadorDTO dto, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<GrupoModificadorDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_GetAll");

        group.MapGet("/getall-async",
                async (IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<GrupoModificadorDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<GrupoModificadorDTO>>, NotFound<Response<GrupoModificadorDTO>>>
                    (int id, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<GrupoModificadorDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<GrupoModificadorDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "GrupoModificador con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<GrupoModificadorDTO>>, NotFound<Response<GrupoModificadorDTO>>>>
                    (int id, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<GrupoModificadorDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<GrupoModificadorDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "GrupoModificador con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<GrupoModificadorDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<GrupoModificadorDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_GetPaged_Async");

        group.MapGet("/count",
                (IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Count");

        group.MapGet("/count-async",
                async (IGrupoModificadorApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("GrupoModificador_Count_Async");

        return endpoints;
    }
}

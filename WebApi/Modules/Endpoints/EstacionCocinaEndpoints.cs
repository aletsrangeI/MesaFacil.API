using Common;
using DTO.EstacionCocina;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class EstacionCocinaEndpoints
{
    public static IEndpointRouteBuilder MapEstacionCocinaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/estacioncocina")
            .WithTags("EstacionCocina")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (EstacionCocinaDTO dto, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Insert");

        group.MapPost("/insert-async",
                async (EstacionCocinaDTO dto, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, EstacionCocinaDTO dto, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, EstacionCocinaDTO dto, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<EstacionCocinaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_GetAll");

        group.MapGet("/getall-async",
                async (IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<EstacionCocinaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<EstacionCocinaDTO>>, NotFound<Response<EstacionCocinaDTO>>>
                    (int id, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<EstacionCocinaDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<EstacionCocinaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "EstacionCocina con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<EstacionCocinaDTO>>, NotFound<Response<EstacionCocinaDTO>>>>
                    (int id, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<EstacionCocinaDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<EstacionCocinaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "EstacionCocina con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<EstacionCocinaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<EstacionCocinaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_GetPaged_Async");

        group.MapGet("/count",
                (IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Count");

        group.MapGet("/count-async",
                async (IEstacionCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("EstacionCocina_Count_Async");

        return endpoints;
    }
}

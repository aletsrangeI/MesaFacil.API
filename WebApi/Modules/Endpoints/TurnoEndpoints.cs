using Common;
using DTO.Turno;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class TurnoEndpoints
{
    public static IEndpointRouteBuilder MapTurnoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/turno")
            .WithTags("Turno")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (TurnoDTO dto, ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Insert");

        group.MapPost("/insert-async",
                async (TurnoDTO dto, ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, TurnoDTO dto, ITurnoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, TurnoDTO dto, ITurnoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<TurnoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_GetAll");

        group.MapGet("/getall-async",
                async (ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<TurnoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<TurnoDTO>>, NotFound<Response<TurnoDTO>>>
                    (int id, ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<TurnoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<TurnoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Turno con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<TurnoDTO>>, NotFound<Response<TurnoDTO>>>>
                    (int id, ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<TurnoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<TurnoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Turno con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, ITurnoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<TurnoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, ITurnoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<TurnoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_GetPaged_Async");

        group.MapGet("/count",
                (ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Count");

        group.MapGet("/count-async",
                async (ITurnoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Turno_Count_Async");

        return endpoints;
    }
}

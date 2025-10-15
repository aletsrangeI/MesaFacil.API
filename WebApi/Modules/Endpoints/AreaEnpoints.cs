using Common;
using DTO.Area;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class AreaEnpoints
{
    public static IEndpointRouteBuilder MapAreaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/area")
            .WithTags("Area")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        // Sync
        group.MapPost("/insert",
                (AreaDTO dto, IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Insert");

        // Async
        group.MapPost("/insert-async",
                async (AreaDTO dto, IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        // Sync
        group.MapPut("/update/{id:int}",
                (int id, AreaDTO dto, IAreaApplication svc, CancellationToken ct) =>
                {
                    try
                    {
                        dto.Id = id;
                    }
                    catch
                    {
                        /* si no hay Id en el DTO, se ignora */
                    }

                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Update");

        // Async
        group.MapPut("/update-async/{id:int}",
                async (int id, AreaDTO dto, IAreaApplication svc, CancellationToken ct) =>
                {
                    try
                    {
                        dto.Id = id;
                    }
                    catch
                    {
                        /* si no hay Id en el DTO, se ignora */
                    }

                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        // Sync
        group.MapDelete("/delete/{id:int}",
                (int id, IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Delete");

        // Async
        group.MapDelete("/delete-async/{id:int}",
                async (int id, IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        // Sync
        group.MapGet("/getall",
                (IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<AreaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Area_GetAll");

        // Async
        group.MapGet("/getall-async",
                async (IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<AreaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Area_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        // Sync (tipos fuertes para evitar "delegate type could not be inferred")
        group.MapGet(
                "/getbyid/{id:int}",
                Results<Ok<Response<AreaDTO>>, NotFound<Response<AreaDTO>>>
                    (int id, IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<AreaDTO>? result = svc.Get(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<AreaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Área con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Area_GetById");

        // Async
        group.MapGet(
                "/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<AreaDTO>>, NotFound<Response<AreaDTO>>>>
                    (int id, IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<AreaDTO>? result = await svc.GetAsync(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<AreaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Área con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Area_GetById_Async");

        // Sync paginado
        group.MapGet("/getpaged",
                (int page, int pageSize, IAreaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<AreaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_GetPaged");

        // Async paginado
        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IAreaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<AreaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Area_GetPaged_Async");

        // Sync count
        group.MapGet("/count",
                (IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Count");

        // Async count
        group.MapGet("/count-async",
                async (IAreaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Area_Count_Async");

        return endpoints;
    }
}
using Common;
using DTO.CorteCaja;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class CorteCajaEndpoints
{
    public static IEndpointRouteBuilder MapCorteCajaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/corteCaja")
            .WithTags("CorteCaja")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        // Sync
        group.MapPost("/insert",
                (CorteCajaDTO dto, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_Insert");

        // Async
        group.MapPost("/insert-async",
                async (CorteCajaDTO dto, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        // Sync
        group.MapPut("/update/{id:int}",
                (int id, CorteCajaDTO dto, ICorteCajaApplication svc, CancellationToken ct) =>
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
            .WithName("CorteCaja_Update");

        // Async
        group.MapPut("/update-async/{id:int}",
                async (int id, CorteCajaDTO dto, ICorteCajaApplication svc, CancellationToken ct) =>
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
            .WithName("CorteCaja_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        // Sync
        group.MapDelete("/delete/{id:int}",
                (int id, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_Delete");

        // Async
        group.MapDelete("/delete-async/{id:int}",
                async (int id, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        // Sync
        group.MapGet("/getall",
                (ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CorteCajaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_GetAll");

        // Async
        group.MapGet("/getall-async",
                async (ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CorteCajaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        // Sync — tipos fuertes para evitar “delegate type could not be inferred”
        group.MapGet(
                "/getbyid/{id:int}",
                Results<Ok<Response<CorteCajaDTO>>, NotFound<Response<CorteCajaDTO>>>
                    (int id, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<CorteCajaDTO>? result = svc.Get(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CorteCajaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Categoría de menú con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_GetById");

        // Async
        group.MapGet(
                "/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<CorteCajaDTO>>, NotFound<Response<CorteCajaDTO>>>>
                    (int id, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<CorteCajaDTO>? result = await svc.GetAsync(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CorteCajaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Categoría de menú con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_GetById_Async");

        group.MapGet("/getpaged",
                (int page, int pageSize, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<CorteCajaDTO>> result =
                        svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_GetPaged");

        // Async paginado
        group.MapGet("/getpaged-async",
                async (int page, int pageSize, ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<CorteCajaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_GetPaged_Async");

        // Sync count
        group.MapGet("/count",
                (ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_Count");

        // Async count
        group.MapGet("/count-async",
                async (ICorteCajaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("CorteCaja_Count_Async");

        return endpoints;
    }
}
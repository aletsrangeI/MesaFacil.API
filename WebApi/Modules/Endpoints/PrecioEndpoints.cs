using Common;
using DTO.Precio;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class PrecioEndpoints
{
    public static IEndpointRouteBuilder MapPrecioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/precio")
            .WithTags("Precio")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (PrecioDTO dto, IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Insert");

        group.MapPost("/insert-async",
                async (PrecioDTO dto, IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, PrecioDTO dto, IPrecioApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, PrecioDTO dto, IPrecioApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PrecioDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_GetAll");

        group.MapGet("/getall-async",
                async (IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PrecioDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<PrecioDTO>>, NotFound<Response<PrecioDTO>>>
                    (int id, IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<PrecioDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PrecioDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Precio con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<PrecioDTO>>, NotFound<Response<PrecioDTO>>>>
                    (int id, IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<PrecioDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PrecioDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Precio con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IPrecioApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PrecioDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IPrecioApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PrecioDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_GetPaged_Async");

        group.MapGet("/count",
                (IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Count");

        group.MapGet("/count-async",
                async (IPrecioApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Precio_Count_Async");

        return endpoints;
    }
}

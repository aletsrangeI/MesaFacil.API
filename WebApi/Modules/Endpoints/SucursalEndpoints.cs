using Common;
using DTO.Sucursal;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class SucursalEndpoints
{
    public static IEndpointRouteBuilder MapSucursalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/sucursal")
            .WithTags("Sucursal")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (SucursalDTO dto, ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Insert");

        group.MapPost("/insert-async",
                async (SucursalDTO dto, ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, SucursalDTO dto, ISucursalApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, SucursalDTO dto, ISucursalApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<SucursalDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_GetAll");

        group.MapGet("/getall-async",
                async (ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<SucursalDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<SucursalDTO>>, NotFound<Response<SucursalDTO>>>
                    (int id, ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<SucursalDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<SucursalDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Sucursal con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<SucursalDTO>>, NotFound<Response<SucursalDTO>>>>
                    (int id, ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<SucursalDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<SucursalDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Sucursal con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, ISucursalApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<SucursalDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, ISucursalApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<SucursalDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_GetPaged_Async");

        group.MapGet("/count",
                (ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Count");

        group.MapGet("/count-async",
                async (ISucursalApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Sucursal_Count_Async");

        return endpoints;
    }
}

using Common;
using DTO.DetalleCuenta;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class DetalleCuentaEndpoints
{
    public static IEndpointRouteBuilder MapDetalleCuentaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/detallecuenta")
            .WithTags("DetalleCuenta")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (DetalleCuentaDTO dto, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Insert");

        group.MapPost("/insert-async",
                async (DetalleCuentaDTO dto, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, DetalleCuentaDTO dto, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, DetalleCuentaDTO dto, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<DetalleCuentaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_GetAll");

        group.MapGet("/getall-async",
                async (IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<DetalleCuentaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<DetalleCuentaDTO>>, NotFound<Response<DetalleCuentaDTO>>>
                    (int id, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<DetalleCuentaDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<DetalleCuentaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "DetalleCuenta con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<DetalleCuentaDTO>>, NotFound<Response<DetalleCuentaDTO>>>>
                    (int id, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<DetalleCuentaDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<DetalleCuentaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "DetalleCuenta con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<DetalleCuentaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<DetalleCuentaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_GetPaged_Async");

        group.MapGet("/count",
                (IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Count");

        group.MapGet("/count-async",
                async (IDetalleCuentaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("DetalleCuenta_Count_Async");

        return endpoints;
    }
}

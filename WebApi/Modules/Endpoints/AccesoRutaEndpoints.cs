using Common;
using DTO.AccesoRuta;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class AccesoRutaEndpoints
{
    public static IEndpointRouteBuilder MapAccesoRutaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/accesoruta")
            .WithTags("AccesoRuta")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (AccesoRutaDTO dto, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Insert");

        group.MapPost("/insert-async",
                async (AccesoRutaDTO dto, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, AccesoRutaDTO dto, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, AccesoRutaDTO dto, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<AccesoRutaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_GetAll");

        group.MapGet("/getall-async",
                async (IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<AccesoRutaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<AccesoRutaDTO>>, NotFound<Response<AccesoRutaDTO>>>
                    (int id, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<AccesoRutaDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<AccesoRutaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "AccesoRuta con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<AccesoRutaDTO>>, NotFound<Response<AccesoRutaDTO>>>>
                    (int id, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<AccesoRutaDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<AccesoRutaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "AccesoRuta con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<AccesoRutaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<AccesoRutaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_GetPaged_Async");

        group.MapGet("/count",
                (IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Count");

        group.MapGet("/count-async",
                async (IAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("AccesoRuta_Count_Async");

        return endpoints;
    }
}

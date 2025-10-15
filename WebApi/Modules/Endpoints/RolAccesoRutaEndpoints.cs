using Common;
using DTO.RolAccesoRuta;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class RolAccesoRutaEndpoints
{
    public static IEndpointRouteBuilder MapRolAccesoRutaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/rolaccesoruta")
            .WithTags("RolAccesoRuta")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (RolAccesoRutaDTO dto, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Insert");

        group.MapPost("/insert-async",
                async (RolAccesoRutaDTO dto, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, RolAccesoRutaDTO dto, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, RolAccesoRutaDTO dto, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<RolAccesoRutaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_GetAll");

        group.MapGet("/getall-async",
                async (IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<RolAccesoRutaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<RolAccesoRutaDTO>>, NotFound<Response<RolAccesoRutaDTO>>>
                    (int id, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<RolAccesoRutaDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<RolAccesoRutaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "RolAccesoRuta con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<RolAccesoRutaDTO>>, NotFound<Response<RolAccesoRutaDTO>>>>
                    (int id, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<RolAccesoRutaDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<RolAccesoRutaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "RolAccesoRuta con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<RolAccesoRutaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<RolAccesoRutaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_GetPaged_Async");

        group.MapGet("/count",
                (IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Count");

        group.MapGet("/count-async",
                async (IRolAccesoRutaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("RolAccesoRuta_Count_Async");

        return endpoints;
    }
}

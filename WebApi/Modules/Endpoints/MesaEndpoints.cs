using Common;
using DTO.Mesa;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class MesaEndpoints
{
    public static IEndpointRouteBuilder MapMesaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/mesa")
            .WithTags("Mesa")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (MesaDTO dto, IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Insert");

        group.MapPost("/insert-async",
                async (MesaDTO dto, IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, MesaDTO dto, IMesaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, MesaDTO dto, IMesaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<MesaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_GetAll");

        group.MapGet("/getall-async",
                async (IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<MesaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<MesaDTO>>, NotFound<Response<MesaDTO>>>
                    (int id, IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<MesaDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<MesaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Mesa con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<MesaDTO>>, NotFound<Response<MesaDTO>>>>
                    (int id, IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<MesaDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<MesaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Mesa con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IMesaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<MesaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IMesaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<MesaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_GetPaged_Async");

        group.MapGet("/count",
                (IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Count");

        group.MapGet("/count-async",
                async (IMesaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Mesa_Count_Async");

        return endpoints;
    }
}

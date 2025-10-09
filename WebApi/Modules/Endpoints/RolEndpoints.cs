using Common;
using DTO.Rol;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class RolEndpoints
{
    public static IEndpointRouteBuilder MapRolEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/rol")
            .WithTags("Rol")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (RolDTO dto, IRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Insert");

        group.MapPost("/insert-async",
                async (RolDTO dto, IRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, RolDTO dto, IRolApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, RolDTO dto, IRolApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IRolApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<RolDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_GetAll");

        group.MapGet("/getall-async",
                async (IRolApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<RolDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<RolDTO>>, NotFound<Response<RolDTO>>>
                    (int id, IRolApplication svc, CancellationToken ct) =>
                {
                    Response<RolDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<RolDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Rol con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<RolDTO>>, NotFound<Response<RolDTO>>>>
                    (int id, IRolApplication svc, CancellationToken ct) =>
                {
                    Response<RolDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<RolDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Rol con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IRolApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<RolDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IRolApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<RolDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_GetPaged_Async");

        group.MapGet("/count",
                (IRolApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Count");

        group.MapGet("/count-async",
                async (IRolApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Rol_Count_Async");

        return endpoints;
    }
}

using Common;
using DTO.UsuarioRol;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class UsuarioRolEndpoints
{
    public static IEndpointRouteBuilder MapUsuarioRolEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/usuariorol")
            .WithTags("UsuarioRol")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (UsuarioRolDTO dto, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Insert");

        group.MapPost("/insert-async",
                async (UsuarioRolDTO dto, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, UsuarioRolDTO dto, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, UsuarioRolDTO dto, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<UsuarioRolDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_GetAll");

        group.MapGet("/getall-async",
                async (IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<UsuarioRolDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<UsuarioRolDTO>>, NotFound<Response<UsuarioRolDTO>>>
                    (int id, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<UsuarioRolDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<UsuarioRolDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "UsuarioRol con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<UsuarioRolDTO>>, NotFound<Response<UsuarioRolDTO>>>>
                    (int id, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<UsuarioRolDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<UsuarioRolDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "UsuarioRol con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<UsuarioRolDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<UsuarioRolDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_GetPaged_Async");

        group.MapGet("/count",
                (IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Count");

        group.MapGet("/count-async",
                async (IUsuarioRolApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("UsuarioRol_Count_Async");

        return endpoints;
    }
}

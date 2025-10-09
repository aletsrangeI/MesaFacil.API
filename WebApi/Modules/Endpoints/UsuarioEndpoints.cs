using Common;
using DTO.Usuario;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class UsuarioEndpoints
{
    public static IEndpointRouteBuilder MapUsuarioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/usuario")
            .WithTags("Usuario")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (UsuarioDTO dto, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Insert");

        group.MapPost("/insert-async",
                async (UsuarioDTO dto, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, UsuarioDTO dto, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, UsuarioDTO dto, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<UsuarioDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_GetAll");

        group.MapGet("/getall-async",
                async (IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<UsuarioDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<UsuarioDTO>>, NotFound<Response<UsuarioDTO>>>
                    (int id, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<UsuarioDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<UsuarioDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Usuario con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<UsuarioDTO>>, NotFound<Response<UsuarioDTO>>>>
                    (int id, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<UsuarioDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<UsuarioDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Usuario con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<UsuarioDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IUsuarioApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<UsuarioDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_GetPaged_Async");

        group.MapGet("/count",
                (IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Count");

        group.MapGet("/count-async",
                async (IUsuarioApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Usuario_Count_Async");

        return endpoints;
    }
}

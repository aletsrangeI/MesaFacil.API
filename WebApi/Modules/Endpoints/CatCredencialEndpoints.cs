using Common;
using DTO.CatCredencial;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class CatCredencialEndpoints
{
    public static IEndpointRouteBuilder MapCatCredencialEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Se agrupan todas las rutas bajo /api/catcredencial (en minúsculas por convención REST)
        var group = endpoints.MapGroup("/api/catcredencial")
            .WithTags("CatCredencial")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (CatCredencialDTO dto, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Insert");

        group.MapPost("/insert-async",
                async (CatCredencialDTO dto, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, CatCredencialDTO dto, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    // Forzamos el ID de la URL al DTO por seguridad
                    try { dto.Id = id; } catch { /* Ignorar si falla la asignación */ }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, CatCredencialDTO dto, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CatCredencialDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_GetAll");

        group.MapGet("/getall-async",
                async (ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CatCredencialDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<CatCredencialDTO>>, NotFound<Response<CatCredencialDTO>>>
                    (int id, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<CatCredencialDTO>? result = svc.Get(id);
                    
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CatCredencialDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"CatCredencial con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<CatCredencialDTO>>, NotFound<Response<CatCredencialDTO>>>>
                    (int id, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<CatCredencialDTO>? result = await svc.GetAsync(id);
                    
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CatCredencialDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"CatCredencial con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<CatCredencialDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<CatCredencialDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_GetPaged_Async");

        group.MapGet("/count",
                (ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Count");

        group.MapGet("/count-async",
                async (ICatCredencialApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("CatCredencial_Count_Async");

        return endpoints;
    }
}
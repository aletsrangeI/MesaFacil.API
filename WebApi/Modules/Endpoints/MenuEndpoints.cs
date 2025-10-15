using Common;
using DTO.Menu;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class MenuEndpoints
{
    public static IEndpointRouteBuilder MapMenuEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/menu")
            .WithTags("Menu")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (MenuDTO dto, IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Insert");

        group.MapPost("/insert-async",
                async (MenuDTO dto, IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, MenuDTO dto, IMenuApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, MenuDTO dto, IMenuApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<MenuDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_GetAll");

        group.MapGet("/getall-async",
                async (IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<MenuDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<MenuDTO>>, NotFound<Response<MenuDTO>>>
                    (int id, IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<MenuDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<MenuDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Menu con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<MenuDTO>>, NotFound<Response<MenuDTO>>>>
                    (int id, IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<MenuDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<MenuDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Menu con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IMenuApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<MenuDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IMenuApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<MenuDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_GetPaged_Async");

        group.MapGet("/count",
                (IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Count");

        group.MapGet("/count-async",
                async (IMenuApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Menu_Count_Async");

        return endpoints;
    }
}

using Common;
using DTO.CategoriaMenu; // Ajusta el namespace si tu DTO vive en otro lugar
using Interface.UseCases; // Asegúrate de tener ICategoriaMenuApplication aquí
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class CategoriaMenuEndpoints
{
    public static IEndpointRouteBuilder MapCategoriaMenuEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/categoriamenu")
            .WithTags("CategoriaMenu")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        // Sync
        group.MapPost("/insert",
                (CategoriaMenuDTO dto, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Insert");

        // Async
        group.MapPost("/insert-async",
                async (CategoriaMenuDTO dto, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        // Sync
        group.MapPut("/update/{id:int}",
                (int id, CategoriaMenuDTO dto, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    try
                    {
                        dto.Id = id;
                    }
                    catch
                    {
                        /* si no hay Id en el DTO, se ignora */
                    }

                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Update");

        // Async
        group.MapPut("/update-async/{id:int}",
                async (int id, CategoriaMenuDTO dto, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    try
                    {
                        dto.Id = id;
                    }
                    catch
                    {
                        /* si no hay Id en el DTO, se ignora */
                    }

                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        // Sync
        group.MapDelete("/delete/{id:int}",
                (int id, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Delete");

        // Async
        group.MapDelete("/delete-async/{id:int}",
                async (int id, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        // Sync
        group.MapGet("/getall",
                (ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CategoriaMenuDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_GetAll");

        // Async
        group.MapGet("/getall-async",
                async (ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CategoriaMenuDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        // Sync — tipos fuertes para evitar “delegate type could not be inferred”
        group.MapGet(
                "/getbyid/{id:int}",
                Results<Ok<Response<CategoriaMenuDTO>>, NotFound<Response<CategoriaMenuDTO>>>
                    (int id, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<CategoriaMenuDTO>? result = svc.Get(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CategoriaMenuDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Categoría de menú con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_GetById");

        // Async
        group.MapGet(
                "/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<CategoriaMenuDTO>>, NotFound<Response<CategoriaMenuDTO>>>>
                    (int id, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<CategoriaMenuDTO>? result = await svc.GetAsync(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CategoriaMenuDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Categoría de menú con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_GetById_Async");

        group.MapGet("/getpaged",
                (int page, int pageSize, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<CategoriaMenuDTO>> result =
                        svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_GetPaged");

        // Async paginado
        group.MapGet("/getpaged-async",
                async (int page, int pageSize, ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<CategoriaMenuDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_GetPaged_Async");

        // Sync count
        group.MapGet("/count",
                (ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Count");

        // Async count
        group.MapGet("/count-async",
                async (ICategoriaMenuApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("CategoriaMenu_Count_Async");

        return endpoints;
    }
}
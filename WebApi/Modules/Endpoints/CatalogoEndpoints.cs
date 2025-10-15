using Common;
using DTO.Catalog;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class CatalogoEndpoints
{
    public static IEndpointRouteBuilder MapCatalogoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/catalogo")
            .WithTags("Catalogo")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        // Sync
        group.MapPost("/insert",
                (CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_Insert");

        // Async
        group.MapPost("/insert-async",
                async (CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        // Sync
        group.MapPut("/update/{id:int}",
                (int id, CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
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
            .WithName("Catalogo_Update");

        // Async
        group.MapPut("/update-async/{id:int}",
                async (int id, CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
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
            .WithName("Catalogo_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        // Sync
        group.MapDelete("/delete/{id:int}",
                (int id, ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return result.isSuccess
                        ? TypedResults.Ok(result)
                        : TypedResults.Ok(result); // si prefieres 404 cuando no borra, cambia a NotFound abajo
                })
            .WithName("Catalogo_Delete");

        // Async
        group.MapDelete("/delete-async/{id:int}",
                async (int id, ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return result.isSuccess
                        ? TypedResults.Ok(result)
                        : TypedResults.Ok(result);
                })
            .WithName("Catalogo_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        // Sync
        group.MapGet("/getall",
                (ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CatalogDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_GetAll");

        // Async
        group.MapGet("/getall-async",
                async (ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CatalogDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        // Sync
        group.MapGet(
                "/getbyid/{id:int}",
                Results<Ok<Response<CatalogDTO>>, NotFound<Response<CatalogDTO>>> // <= retorno explícito
                    (int id, ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<CatalogDTO>? result = svc.Get(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CatalogDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Catálogo con id {id} no encontrado",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_GetById");

        // Async
        group.MapGet(
                "/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<CatalogDTO>>, NotFound<Response<CatalogDTO>>>> // <= retorno explícito
                    (int id, ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<CatalogDTO>? result = await svc.GetAsync(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CatalogDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Catálogo con id {id} no encontrado",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_GetById_Async");

        // =========================================================
        // GET PAGED
        // =========================================================

        // Sync
        group.MapGet("/getpaged",
                (int page, int pageSize, ICatalogApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;

                    ResponsePagination<IEnumerable<CatalogDTO>> result =
                        svc.GetAllWithPagination(page, pageSize);

                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_GetPaged");

        // Async
        group.MapGet("/getpaged-async",
                async (int page, int pageSize, ICatalogApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;

                    ResponsePagination<IEnumerable<CatalogDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);

                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_GetPaged_Async");

        // =========================================================
        // COUNT
        // =========================================================

        // Sync
        group.MapGet("/count",
                (ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_Count");

        // Async
        group.MapGet("/count-async",
                async (ICatalogApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Catalogo_Count_Async");

        return app;
    }
}
using Common;
using DTO;
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

        // INSERT
        group.MapPost("/insert", async (CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.InsertAsync(dto);
                return Results.Ok(result);
            })
            .WithName("Catalogo_Insert");

        // UPDATE
        group.MapPut("/update/{id:int}", async (int id, CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
            {
                try { dto.Id = id; } catch { /* si no hay Id en el DTO, se ignora */ }
                var result = await svc.UpdateAsync(dto);
                return Results.Ok(result);
            })
            .WithName("Catalogo_Update");

        // DELETE
        group.MapDelete("/delete/{id:int}", async (int id, ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.DeleteAsync(id);
                return Results.Ok(result);
            })
            .WithName("Catalogo_Delete");

        // GET ALL
        group.MapGet("/getall", async (ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.GetAllAsync();
                return Results.Ok(result);
            })
            .WithName("Catalogo_GetAll");

        // GET BY ID
        group.MapGet("/getbyid/{id:int}",
                async Task<Results<Ok<Response<CatalogDTO>>, NotFound>> (int id, ICatalogApplication svc) =>
                {
                    var result = await svc.GetAsync(id);
                    return (result is null || result.Data is null)
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(result);
                })
            .WithName("Catalogo_GetById");

        // GET PAGED
        group.MapGet("/getpaged", async (int page, int pageSize, ICatalogApplication svc, CancellationToken ct) =>
            {
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 10 : pageSize;

                var result = await svc.GetAllWithPaginationAsync(page, pageSize);
                return Results.Ok(result);
            })
            .WithName("Catalogo_GetPaged");

        // COUNT
        group.MapGet("/count", async (ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.CountAsync();
                return Results.Ok(result);
            })
            .WithName("Catalogo_Count");

        // (Opcional) Rutas REST anteriores — si quieres compatibilidad temporal, descomenta:
        /*
        group.MapPost("/", async (CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) => Results.Ok(await svc.InsertAsync(dto))).WithName("Catalogo_Create_LEGACY");
        group.MapPut("/{id:int}", async (int id, CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) => { try { dto.Id = id; } catch {} return Results.Ok(await svc.UpdateAsync(dto)); }).WithName("Catalogo_Update_LEGACY");
        group.MapDelete("/{id:int}", async (int id, ICatalogApplication svc, CancellationToken ct) => Results.Ok(await svc.DeleteAsync(id))).WithName("Catalogo_Delete_LEGACY");
        group.MapGet("/", async (ICatalogApplication svc, CancellationToken ct) => Results.Ok(await svc.GetAllAsync())).WithName("Catalogo_GetAll_LEGACY");
        group.MapGet("/{id:int}", async Task<Results<Ok<Response<CatalogDTO>>, NotFound>> (int id, ICatalogApplication svc) => {
            var result = await svc.GetAsync(id);
            return (result is null || result.Data is null) ? TypedResults.NotFound() : TypedResults.Ok(result);
        }).WithName("Catalogo_GetById_LEGACY");
        group.MapGet("/paged", async (int page, int pageSize, ICatalogApplication svc, CancellationToken ct) => Results.Ok(await svc.GetAllWithPaginationAsync(page <= 0 ? 1 : page, pageSize <= 0 ? 10 : pageSize))).WithName("Catalogo_GetPaged_LEGACY");
        group.MapGet("/count", async (ICatalogApplication svc, CancellationToken ct) => Results.Ok(await svc.CountAsync())).WithName("Catalogo_Count_LEGACY_DUP");
        */

        return app;
    }
}

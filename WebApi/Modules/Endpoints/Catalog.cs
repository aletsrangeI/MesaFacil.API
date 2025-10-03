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
        // /api/catalogo
        var group = app.MapGroup("/api/catalogo")
            .RequireAuthorization() // equivale a [Authorize]
            .WithTags("Catalogo") // tag de OpenAPI
            .WithOpenApi();

        // ---------- Estilo RESTful y async (recomendado) ----------

        // Crear
        group.MapPost("/", async (CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.InsertAsync(dto);
                return Results.Ok(result);
            })
            .WithName("Catalogo_Create");

        // Actualizar (id en ruta; si tu DTO ya trae Id, puedes ignorar esta asignación)
        group.MapPut("/{id:int}", async (int id, CatalogDTO dto, ICatalogApplication svc, CancellationToken ct) =>
            {
                try
                {
                    dto.Id = id;
                }
                catch
                {
                    /* si no hay Id en el DTO simplemente omite */
                }

                var result = await svc.UpdateAsync(dto);
                return Results.Ok(result);
            })
            .WithName("Catalogo_Update");

        // Eliminar
        group.MapDelete("/{id:int}", async (int id, ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.DeleteAsync(id);
                return Results.Ok(result);
            })
            .WithName("Catalogo_Delete");

        // Obtener todos
        group.MapGet("/", async (ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.GetAllAsync();
                return Results.Ok(result);
            })
            .WithName("Catalogo_GetAll");

        // Obtener por Id
        group.MapGet("/{id:int}",
                async Task<Results<Ok<Response<CatalogDTO>>, NotFound>> (int id, ICatalogApplication svc) =>
                {
                    var result = await svc.GetAsync(id); // devuelve Response<CatalogDTO> o null
                    return (result is null || result.Data is null)
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(result); // Ok<Response<CatalogDTO>>
                })
            .WithName("Catalogo_GetById");

        // Paginado
        group.MapGet("/paged", async (int page, int pageSize, ICatalogApplication svc, CancellationToken ct) =>
            {
                // Guardas mínimas
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 10 : pageSize;

                var result = await svc.GetAllWithPaginationAsync(page, pageSize);
                return Results.Ok(result);
            })
            .WithName("Catalogo_GetPaged");

        // Conteo
        group.MapGet("/count", async (ICatalogApplication svc, CancellationToken ct) =>
            {
                var result = await svc.CountAsync();
                return Results.Ok(result);
            })
            .WithName("Catalogo_Count");

        // ---------- (Opcional) Compat: mismas rutas del controller original ----------
        // Si necesitas mantener URLs existentes mientras migras, descomenta:
        /*
        group.MapPost("/InsertAsync", async (CatalogDTO dto, ICatalogApplication svc) => Results.Ok(await svc.InsertAsync(dto)));
        group.MapPut("/UpdateAsync", async (CatalogDTO dto, ICatalogApplication svc) => Results.Ok(await svc.UpdateAsync(dto)));
        group.MapDelete("/DeleteAsync", async (int id, ICatalogApplication svc) => Results.Ok(await svc.DeleteAsync(id)));
        group.MapGet("/GetAllAsync", async (ICatalogApplication svc) => Results.Ok(await svc.GetAllAsync()));
        group.MapGet("/GetAllWithPaginationAsync", async (int page, int pageSize, ICatalogApplication svc) => Results.Ok(await svc.GetAllWithPaginationAsync(page, pageSize)));
        group.MapGet("/CountAsync", async (ICatalogApplication svc) => Results.Ok(await svc.CountAsync()));
        */

        return app;
    }
}
using Common;
using DTO.Producto;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class ProductoEndpoints
{
    public static IEndpointRouteBuilder MapProductoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/producto")
            .WithTags("Producto")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (ProductoDTO dto, IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Insert");

        group.MapPost("/insert-async",
                async (ProductoDTO dto, IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, ProductoDTO dto, IProductoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, ProductoDTO dto, IProductoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<ProductoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_GetAll");

        group.MapGet("/getall-async",
                async (IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<ProductoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<ProductoDTO>>, NotFound<Response<ProductoDTO>>>
                    (int id, IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<ProductoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<ProductoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Producto con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<ProductoDTO>>, NotFound<Response<ProductoDTO>>>>
                    (int id, IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<ProductoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<ProductoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Producto con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IProductoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<ProductoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IProductoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<ProductoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_GetPaged_Async");

        group.MapGet("/count",
                (IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Count");

        group.MapGet("/count-async",
                async (IProductoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Producto_Count_Async");

        return endpoints;
    }
}

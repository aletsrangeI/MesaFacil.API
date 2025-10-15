using Common;
using DTO.VarianteProducto;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class VarianteProductoEndpoints
{
    public static IEndpointRouteBuilder MapVarianteProductoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/varianteproducto")
            .WithTags("VarianteProducto")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (VarianteProductoDTO dto, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Insert");

        group.MapPost("/insert-async",
                async (VarianteProductoDTO dto, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, VarianteProductoDTO dto, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, VarianteProductoDTO dto, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<VarianteProductoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_GetAll");

        group.MapGet("/getall-async",
                async (IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<VarianteProductoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<VarianteProductoDTO>>, NotFound<Response<VarianteProductoDTO>>>
                    (int id, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<VarianteProductoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<VarianteProductoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "VarianteProducto con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<VarianteProductoDTO>>, NotFound<Response<VarianteProductoDTO>>>>
                    (int id, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<VarianteProductoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<VarianteProductoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "VarianteProducto con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<VarianteProductoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<VarianteProductoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_GetPaged_Async");

        group.MapGet("/count",
                (IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Count");

        group.MapGet("/count-async",
                async (IVarianteProductoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("VarianteProducto_Count_Async");

        return endpoints;
    }
}

using Common;
using DTO.DescuentoAplicado;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class DescuentoAplicadoEndpoints
{
    public static IEndpointRouteBuilder MapDescuentoAplicadoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/descuentoaplicado")
            .WithTags("DescuentoAplicado")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (DescuentoAplicadoDTO dto, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Insert");

        group.MapPost("/insert-async",
                async (DescuentoAplicadoDTO dto, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, DescuentoAplicadoDTO dto, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, DescuentoAplicadoDTO dto, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<DescuentoAplicadoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_GetAll");

        group.MapGet("/getall-async",
                async (IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<DescuentoAplicadoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<DescuentoAplicadoDTO>>, NotFound<Response<DescuentoAplicadoDTO>>>
                    (int id, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<DescuentoAplicadoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<DescuentoAplicadoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "DescuentoAplicado con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<DescuentoAplicadoDTO>>, NotFound<Response<DescuentoAplicadoDTO>>>>
                    (int id, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<DescuentoAplicadoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<DescuentoAplicadoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "DescuentoAplicado con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<DescuentoAplicadoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<DescuentoAplicadoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_GetPaged_Async");

        group.MapGet("/count",
                (IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Count");

        group.MapGet("/count-async",
                async (IDescuentoAplicadoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("DescuentoAplicado_Count_Async");

        return endpoints;
    }
}

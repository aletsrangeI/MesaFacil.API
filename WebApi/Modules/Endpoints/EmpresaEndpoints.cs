using Common;
using DTO.Empresa;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class EmpresaEndpoints
{
    public static IEndpointRouteBuilder MapEmpresaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/empresa")
            .WithTags("Empresa")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (EmpresaDTO dto, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Insert");

        group.MapPost("/insert-async",
                async (EmpresaDTO dto, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, EmpresaDTO dto, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, EmpresaDTO dto, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<EmpresaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_GetAll");

        group.MapGet("/getall-async",
                async (IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<EmpresaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<EmpresaDTO>>, NotFound<Response<EmpresaDTO>>>
                    (int id, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<EmpresaDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<EmpresaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Empresa con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<EmpresaDTO>>, NotFound<Response<EmpresaDTO>>>>
                    (int id, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<EmpresaDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<EmpresaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Empresa con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<EmpresaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IEmpresaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<EmpresaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_GetPaged_Async");

        group.MapGet("/count",
                (IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Count");

        group.MapGet("/count-async",
                async (IEmpresaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Empresa_Count_Async");

        return endpoints;
    }
}

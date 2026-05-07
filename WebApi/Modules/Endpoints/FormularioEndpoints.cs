using Common;
using DTO.Formulario;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class FormularioEndpoints
{
    public static IEndpointRouteBuilder MapFormularioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Se agrupan todas las rutas bajo /api/formulario (en minúsculas por convención REST)
        var group = endpoints.MapGroup("/api/formulario")
            .WithTags("Formulario")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (FormularioDTO dto, IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Insert");

        group.MapPost("/insert-async",
                async (FormularioDTO dto, IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, FormularioDTO dto, IFormularioApplication svc, CancellationToken ct) =>
                {
                    // Forzamos el ID de la URL al DTO por seguridad
                    try { dto.Id = id; } catch { /* Ignorar si falla la asignación */ }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, FormularioDTO dto, IFormularioApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<FormularioDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_GetAll");

        group.MapGet("/getall-async",
                async (IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<FormularioDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<FormularioDTO>>, NotFound<Response<FormularioDTO>>>
                    (int id, IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<FormularioDTO>? result = svc.Get(id);
                    
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<FormularioDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Formulario con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<FormularioDTO>>, NotFound<Response<FormularioDTO>>>>
                    (int id, IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<FormularioDTO>? result = await svc.GetAsync(id);
                    
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<FormularioDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Formulario con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IFormularioApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<FormularioDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IFormularioApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<FormularioDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_GetPaged_Async");

        group.MapGet("/count",
                (IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Count");

        group.MapGet("/count-async",
                async (IFormularioApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Formulario_Count_Async");

        return endpoints;
    }
}
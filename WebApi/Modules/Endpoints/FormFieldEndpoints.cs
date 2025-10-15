using Common;
using DTO.FormField;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class FormFieldEndpoints
{
    public static IEndpointRouteBuilder MapFormFieldEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/formfield")
            .WithTags("FormField")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (FormFieldDTO dto, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Insert");

        group.MapPost("/insert-async",
                async (FormFieldDTO dto, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, FormFieldDTO dto, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    try
                    {
                        dto.Id = id;
                    }
                    catch
                    {
                    }

                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, FormFieldDTO dto, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    try
                    {
                        dto.Id = id;
                    }
                    catch
                    {
                    }

                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<FormFieldDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetAll");

        group.MapGet("/getall-async",
                async (IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<FormFieldDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<FormFieldDTO>>, NotFound<Response<FormFieldDTO>>>
                    (int id, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<FormFieldDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<FormFieldDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "FormField con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<FormFieldDTO>>, NotFound<Response<FormFieldDTO>>>>
                    (int id, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<FormFieldDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<FormFieldDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "FormField con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<FormFieldDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<FormFieldDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetPaged_Async");

        group.MapGet("/count",
                (IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Count");

        group.MapGet("/count-async",
                async (IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_Count_Async");

        group.MapGet("/GetFormFieldByFormCatId/{id:int}",
                (int id, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<FormFieldDTO>> result = svc.GetFormFieldByFormCatId(id);
                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetFormFieldByFormCatId");

        //GetFormFieldByFormCatIdAsync

        group.MapGet("/GetFormFieldByFormCatIdAsync/{id:int}",
                async Task<Results<Ok<Response<IEnumerable<FormFieldDTO>>>,
                        NotFound<Response<IEnumerable<FormFieldDTO>>>>>
                    (int id, IFormFieldApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<FormFieldDTO>>? result = await svc.GetFormFieldByFormCatIdAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<IEnumerable<FormFieldDTO>>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "FormField con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("FormField_GetFormFieldByFormCatIdAsync_Async");

        return endpoints;
    }
}
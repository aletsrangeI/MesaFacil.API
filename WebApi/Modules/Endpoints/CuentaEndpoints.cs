using Common;
using DTO.Cuenta; // Ajusta si tu DTO vive en otro namespace
using Interface.UseCases; // Asegúrate de tener ICuentaApplication aquí
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class CuentaEndpoints
{
    public static IEndpointRouteBuilder MapCuentaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/cuenta")
            .WithTags("Cuenta")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        // Sync
        group.MapPost("/insert",
                (CuentaDTO dto, ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_Insert");

        // Async
        group.MapPost("/insert-async",
                async (CuentaDTO dto, ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        // Sync
        group.MapPut("/update/{id:int}",
                (int id, CuentaDTO dto, ICuentaApplication svc, CancellationToken ct) =>
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
            .WithName("Cuenta_Update");

        // Async
        group.MapPut("/update-async/{id:int}",
                async (int id, CuentaDTO dto, ICuentaApplication svc, CancellationToken ct) =>
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
            .WithName("Cuenta_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        // Sync
        group.MapDelete("/delete/{id:int}",
                (int id, ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_Delete");

        // Async
        group.MapDelete("/delete-async/{id:int}",
                async (int id, ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        // Sync
        group.MapGet("/getall",
                (ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CuentaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_GetAll");

        // Async
        group.MapGet("/getall-async",
                async (ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<CuentaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        // Sync — tipos fuertes para evitar “delegate type could not be inferred”
        group.MapGet(
                "/getbyid/{id:int}",
                Results<Ok<Response<CuentaDTO>>, NotFound<Response<CuentaDTO>>>
                    (int id, ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<CuentaDTO>? result = svc.Get(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CuentaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Cuenta con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_GetById");

        // Async
        group.MapGet(
                "/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<CuentaDTO>>, NotFound<Response<CuentaDTO>>>>
                    (int id, ICuentaApplication svc, CancellationToken ct) =>
                {
                    Response<CuentaDTO>? result = await svc.GetAsync(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<CuentaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Cuenta con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Cuenta_GetById_Async");

        // =========================================================
        // OPCIONALES (descomenta si tu interfaz los expone)
        // =========================================================

        // // Sync paginado
        // group.MapGet("/getpaged",
        //     (int page, int pageSize, ICuentaApplication svc, CancellationToken ct) =>
        //     {
        //         page = page <= 0 ? 1 : page;
        //         pageSize = pageSize <= 0 ? 10 : pageSize;
        //         ResponsePagination<IEnumerable<CuentaDTO>> result =
        //             svc.GetAllWithPagination(page, pageSize);
        //         return TypedResults.Ok(result);
        //     })
        //     .WithName("Cuenta_GetPaged");
        //
        // // Async paginado
        // group.MapGet("/getpaged-async",
        //     async (int page, int pageSize, ICuentaApplication svc, CancellationToken ct) =>
        //     {
        //         page = page <= 0 ? 1 : page;
        //         pageSize = pageSize <= 0 ? 10 : pageSize;
        //         ResponsePagination<IEnumerable<CuentaDTO>> result =
        //             await svc.GetAllWithPaginationAsync(page, pageSize);
        //         return TypedResults.Ok(result);
        //     })
        //     .WithName("Cuenta_GetPaged_Async");
        //
        // // Sync count
        // group.MapGet("/count",
        //     (ICuentaApplication svc, CancellationToken ct) =>
        //     {
        //         Response<int> result = svc.Count();
        //         return TypedResults.Ok(result);
        //     })
        //     .WithName("Cuenta_Count");
        //
        // // Async count
        // group.MapGet("/count-async",
        //     async (ICuentaApplication svc, CancellationToken ct) =>
        //     {
        //         Response<int> result = await svc.CountAsync();
        //         return TypedResults.Ok(result);
        //     })
        //     .WithName("Cuenta_Count_Async");

        return endpoints;
    }
}
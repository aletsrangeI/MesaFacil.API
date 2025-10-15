using Common;
using DTO.Cliente; // Ajusta el namespace si tu DTO vive en otro lugar
using Interface.UseCases; // Asegúrate de tener IClienteApplication aquí
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class ClienteEndpoints
{
    public static IEndpointRouteBuilder MapClienteEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/cliente")
            .WithTags("Cliente")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        // Sync
        group.MapPost("/insert",
                (ClienteDTO dto, IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_Insert");

        // Async
        group.MapPost("/insert-async",
                async (ClienteDTO dto, IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        // Sync
        group.MapPut("/update/{id:int}",
                (int id, ClienteDTO dto, IClienteApplication svc, CancellationToken ct) =>
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
            .WithName("Cliente_Update");

        // Async
        group.MapPut("/update-async/{id:int}",
                async (int id, ClienteDTO dto, IClienteApplication svc, CancellationToken ct) =>
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
            .WithName("Cliente_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        // Sync
        group.MapDelete("/delete/{id:int}",
                (int id, IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_Delete");

        // Async
        group.MapDelete("/delete-async/{id:int}",
                async (int id, IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        // Sync
        group.MapGet("/getall",
                (IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<ClienteDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_GetAll");

        // Async
        group.MapGet("/getall-async",
                async (IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<ClienteDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        // Sync — tipos fuertes para evitar “delegate type could not be inferred”
        group.MapGet(
                "/getbyid/{id:int}",
                Results<Ok<Response<ClienteDTO>>, NotFound<Response<ClienteDTO>>>
                    (int id, IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<ClienteDTO>? result = svc.Get(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<ClienteDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Categoría de menú con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_GetById");

        // Async
        group.MapGet(
                "/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<ClienteDTO>>, NotFound<Response<ClienteDTO>>>>
                    (int id, IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<ClienteDTO>? result = await svc.GetAsync(id);

                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<ClienteDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = $"Categoría de menú con id {id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_GetById_Async");

        group.MapGet("/getpaged",
                (int page, int pageSize, IClienteApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<ClienteDTO>> result =
                        svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_GetPaged");

        // Async paginado
        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IClienteApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<ClienteDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_GetPaged_Async");

        // Sync count
        group.MapGet("/count",
                (IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_Count");

        // Async count
        group.MapGet("/count-async",
                async (IClienteApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Cliente_Count_Async");

        return endpoints;
    }
}
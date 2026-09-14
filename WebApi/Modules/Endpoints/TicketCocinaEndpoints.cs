using Common;
using DTO.TicketCocina;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class TicketCocinaEndpoints
{
    public static IEndpointRouteBuilder MapTicketCocinaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ticketcocina")
            .WithTags("TicketCocina")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                Results<Ok<Response<Guid>>, BadRequest<Response<Guid>>> (TicketCocinaDTO dto, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<Guid> result = svc.Insert(dto);
                    return result.isSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
                })
            .WithName("TicketCocina_Insert");

        group.MapPost("/insert-async",
                async Task<Results<Ok<Response<Guid>>, BadRequest<Response<Guid>>>> (TicketCocinaDTO dto, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<Guid> response = await svc.InsertAsync(dto);
                    return response.isSuccess ? TypedResults.Ok(response) : TypedResults.BadRequest(response);
                })
            .WithName("TicketCocina_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:guid}",
                (Guid id, TicketCocinaDTO dto, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_Update");

        group.MapPut("/update-async/{id:guid}",
                async (Guid id, TicketCocinaDTO dto, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:guid}",
                (Guid id, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_Delete");

        group.MapDelete("/delete-async/{id:guid}",
                async (Guid id, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<TicketCocinaDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_GetAll");

        group.MapGet("/getall-async",
                async (ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<TicketCocinaDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:guid}",
                Results<Ok<Response<TicketCocinaDTO>>, NotFound<Response<TicketCocinaDTO>>>
                    (Guid id, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<TicketCocinaDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<TicketCocinaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "TicketCocina con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_GetById");

        group.MapGet("/getbyid-async/{id:guid}",
                async Task<Results<Ok<Response<TicketCocinaDTO>>, NotFound<Response<TicketCocinaDTO>>>>
                    (Guid id, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<TicketCocinaDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<TicketCocinaDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "TicketCocina con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<TicketCocinaDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<TicketCocinaDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_GetPaged_Async");

        group.MapGet("/count",
                (ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_Count");

        group.MapGet("/count-async",
                async (ITicketCocinaApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("TicketCocina_Count_Async");

        return endpoints;
    }
}

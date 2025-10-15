using Common;
using DTO.Pago;
using Interface.UseCases;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MesaFacil.API.Modules.Endpoints;

public static class PagoEndpoints
{
    public static IEndpointRouteBuilder MapPagoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pago")
            .WithTags("Pago")
            .WithOpenApi();

        // =========================================================
        // INSERT
        // =========================================================

        group.MapPost("/insert",
                (PagoDTO dto, IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Insert(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Insert");

        group.MapPost("/insert-async",
                async (PagoDTO dto, IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.InsertAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Insert_Async");

        // =========================================================
        // UPDATE
        // =========================================================

        group.MapPut("/update/{id:int}",
                (int id, PagoDTO dto, IPagoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = svc.Update(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Update");

        group.MapPut("/update-async/{id:int}",
                async (int id, PagoDTO dto, IPagoApplication svc, CancellationToken ct) =>
                {
                    try { dto.Id = id; } catch { }
                    Response<bool> result = await svc.UpdateAsync(dto);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Update_Async");

        // =========================================================
        // DELETE
        // =========================================================

        group.MapDelete("/delete/{id:int}",
                (int id, IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = svc.Delete(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Delete");

        group.MapDelete("/delete-async/{id:int}",
                async (int id, IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<bool> result = await svc.DeleteAsync(id);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Delete_Async");

        // =========================================================
        // GET ALL
        // =========================================================

        group.MapGet("/getall",
                (IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PagoDTO>> result = svc.GetAll();
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_GetAll");

        group.MapGet("/getall-async",
                async (IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<IEnumerable<PagoDTO>> result = await svc.GetAllAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_GetAll_Async");

        // =========================================================
        // GET BY ID
        // =========================================================

        group.MapGet("/getbyid/{id:int}",
                Results<Ok<Response<PagoDTO>>, NotFound<Response<PagoDTO>>>
                    (int id, IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<PagoDTO>? result = svc.Get(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PagoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Pago con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_GetById");

        group.MapGet("/getbyid-async/{id:int}",
                async Task<Results<Ok<Response<PagoDTO>>, NotFound<Response<PagoDTO>>>>
                    (int id, IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<PagoDTO>? result = await svc.GetAsync(id);
                    if (result is null || result.Data is null)
                    {
                        var notFound = new Response<PagoDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Pago con id ${id} no encontrada",
                            Errors = Array.Empty<FluentValidation.Results.ValidationFailure>()
                        };
                        return TypedResults.NotFound(notFound);
                    }
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_GetById_Async");

        // =========================================================
        // PAGINADO & COUNT
        // =========================================================

        group.MapGet("/getpaged",
                (int page, int pageSize, IPagoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PagoDTO>> result = svc.GetAllWithPagination(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_GetPaged");

        group.MapGet("/getpaged-async",
                async (int page, int pageSize, IPagoApplication svc, CancellationToken ct) =>
                {
                    page = page <= 0 ? 1 : page;
                    pageSize = pageSize <= 0 ? 10 : pageSize;
                    ResponsePagination<IEnumerable<PagoDTO>> result =
                        await svc.GetAllWithPaginationAsync(page, pageSize);
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_GetPaged_Async");

        group.MapGet("/count",
                (IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = svc.Count();
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Count");

        group.MapGet("/count-async",
                async (IPagoApplication svc, CancellationToken ct) =>
                {
                    Response<int> result = await svc.CountAsync();
                    return TypedResults.Ok(result);
                })
            .WithName("Pago_Count_Async");

        return endpoints;
    }
}

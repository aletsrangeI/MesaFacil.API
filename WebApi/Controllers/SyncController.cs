using Common;
using DTO.Sync;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

/// <summary>
/// Spec 019 (Fase 3): endpoints mínimos de sincronización Edge-Cloud.
/// - POST push-events: ingesta por lotes de OutboxEvents generados en el Edge Node, con
///   idempotencia estricta por Id (un evento con el mismo Id nunca se procesa dos veces).
/// - GET pull-catalogs: catálogos vigentes que el Edge Node necesita para operar sin conexión.
/// No implementa: Edge Node físico, sockets ESC/POS, ni reintentos con backoff exponencial en
/// el lado servidor (eso corre en el CloudSyncWorker del lado Edge) — fuera de alcance de esta tarea.
/// </summary>
[Authorize]
[Route("api/sync")]
[ApiController]
public class SyncController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SyncController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("push-events")]
    public async Task<ActionResult<Response<PushEventsResultDTO>>> PushEvents([FromBody] PushEventsRequestDTO request, CancellationToken ct)
    {
        var result = new PushEventsResultDTO { Recibidos = request.Events.Count };

        if (request.Events.Count == 0)
        {
            return Ok(new Response<PushEventsResultDTO> { Data = result, isSuccess = true, Message = "Sin eventos por procesar." });
        }

        var incomingIds = request.Events.Select(e => e.Id).ToList();
        var yaExistentes = await _context.OutboxEvents
            .Where(e => incomingIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync(ct);

        var yaExistentesSet = yaExistentes.ToHashSet();

        var nuevos = request.Events
            .Where(e => !yaExistentesSet.Contains(e.Id))
            .Select(e => new Domain.Entities.OutboxEvent
            {
                Id = e.Id,
                AggregateType = e.AggregateType,
                AggregateId = e.AggregateId,
                EventType = e.EventType,
                PayloadJson = e.PayloadJson,
                CreatedAt = e.CreatedAt,
                SyncedAt = DateTime.UtcNow,
                SyncStatus = Domain.Entities.OutboxSyncStatus.Synced
            })
            .ToList();

        if (nuevos.Count > 0)
        {
            _context.OutboxEvents.AddRange(nuevos);
            await _context.SaveChangesAsync(ct);
        }

        result.Aceptados = nuevos.Count;
        result.YaProcesadosPreviamente = yaExistentesSet.Count;
        result.IdsAceptados = nuevos.Select(n => n.Id).ToList();
        result.IdsDuplicados = yaExistentesSet.ToList();

        return Ok(new Response<PushEventsResultDTO>
        {
            Data = result,
            isSuccess = true,
            Message = $"Procesados {result.Aceptados} eventos nuevos, {result.YaProcesadosPreviamente} ya existentes (idempotencia por Id)."
        });
    }

    [HttpGet("pull-catalogs")]
    public async Task<ActionResult<Response<PullCatalogsResultDTO>>> PullCatalogs(CancellationToken ct)
    {
        var dto = new PullCatalogsResultDTO
        {
            EstadosPedido = await _context.CatEstadosPedido.Where(c => c.IsActive)
                .Select(c => new CatalogItemLiteDTO { Id = c.Id, Descripcion = c.Descripcion }).ToListAsync(ct),
            EstadosPedidoDetalle = await _context.CatEstadosPedidoDetalle.Where(c => c.IsActive)
                .Select(c => new CatalogItemLiteDTO { Id = c.Id, Descripcion = c.Descripcion }).ToListAsync(ct),
            EstadosTicketCocina = await _context.CatEstadosTicketCocina.Where(c => c.IsActive)
                .Select(c => new CatalogItemLiteDTO { Id = c.Id, Descripcion = c.Descripcion }).ToListAsync(ct),
            EstadosItemKDS = await _context.CatEstadosItemKDS.Where(c => c.IsActive)
                .Select(c => new CatalogItemLiteDTO { Id = c.Id, Descripcion = c.Descripcion }).ToListAsync(ct),
            MetodosDePago = await _context.CatMetodosDePago.Where(c => c.IsActive)
                .Select(c => new CatalogItemLiteDTO { Id = c.Id, Descripcion = c.Descripcion }).ToListAsync(ct),
            TiposPedido = await _context.CatTiposPedido.Where(c => c.IsActive)
                .Select(c => new CatalogItemLiteDTO { Id = c.Id, Descripcion = c.Descripcion }).ToListAsync(ct)
        };

        return Ok(new Response<PullCatalogsResultDTO> { Data = dto, isSuccess = true, Message = "Catálogos vigentes obtenidos correctamente." });
    }
}

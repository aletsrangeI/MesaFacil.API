using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Persistence.Interceptors;

/// <summary>
/// Spec 019 (Outbox Pattern): al guardar cambios, encola automáticamente en
/// <see cref="OutboxEvent"/> cada Insert/Update de las entidades operativas
/// (Pedido, PedidoDetalle, TicketCocina, Pago, MovimientoCaja) para que el
/// CloudSyncWorker las pueda propagar de forma idempotente entre Edge y Cloud.
/// </summary>
public class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly HashSet<Type> TrackedAggregateTypes = new()
    {
        typeof(Pedido),
        typeof(PedidoDetalle),
        typeof(TicketCocina),
        typeof(Pago),
        typeof(MovimientoCaja)
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        EnqueueOutboxEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        EnqueueOutboxEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private static void EnqueueOutboxEvents(DbContext? context)
    {
        if (context is null) return;

        var candidateEntries = context.ChangeTracker.Entries()
            .Where(e => TrackedAggregateTypes.Contains(e.Entity.GetType())
                        && (e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        if (candidateEntries.Count == 0) return;

        var outboxEntries = new List<OutboxEvent>();

        foreach (var entry in candidateEntries)
        {
            var aggregateId = GetGuidId(entry);
            if (aggregateId == Guid.Empty) continue;

            var eventType = entry.State == EntityState.Added ? "Created" : "Updated";
            var payload = SerializeCurrentValues(entry);

            outboxEntries.Add(new OutboxEvent
            {
                Id = Guid.CreateVersion7(),
                AggregateType = entry.Entity.GetType().Name,
                AggregateId = aggregateId,
                EventType = eventType,
                PayloadJson = payload,
                CreatedAt = DateTime.UtcNow,
                SyncStatus = OutboxSyncStatus.Pending
            });
        }

        if (outboxEntries.Count > 0)
        {
            context.Set<OutboxEvent>().AddRange(outboxEntries);
        }
    }

    private static Guid GetGuidId(EntityEntry entry)
    {
        var idProperty = entry.Property("Id");
        return idProperty.CurrentValue is Guid guid ? guid : Guid.Empty;
    }

    private static string SerializeCurrentValues(EntityEntry entry)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var prop in entry.Properties)
        {
            dict[prop.Metadata.Name] = prop.CurrentValue;
        }

        return JsonSerializer.Serialize(dict);
    }
}

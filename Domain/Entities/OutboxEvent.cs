namespace Domain.Entities;

/// <summary>
/// Spec 019 (Outbox Pattern): registro de cada cambio en las entidades operativas
/// (Pedido, PedidoDetalle, TicketCocina, Pago, MovimientoCaja) para sincronización
/// idempotente y asíncrona entre el Edge Node de la sucursal y la nube.
/// </summary>
public class OutboxEvent
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string AggregateType { get; set; } = string.Empty;
    public Guid AggregateId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SyncedAt { get; set; }
    public string SyncStatus { get; set; } = OutboxSyncStatus.Pending;
    public int RetryCount { get; set; }
}

public static class OutboxSyncStatus
{
    public const string Pending = "Pending";
    public const string Synced = "Synced";
    public const string Failed = "Failed";
}

namespace DTO.Sync;

/// <summary>
/// Spec 019: representa un evento de la tabla OutboxEvents tal como lo envía un Edge Node
/// al hacer POST /api/sync/push-events. El Id es la clave de idempotencia: si el mismo Id
/// ya fue procesado, el servidor lo ignora silenciosamente (no se duplica ni se sobreescribe).
/// </summary>
public class SyncEventDTO
{
    public Guid Id { get; set; }
    public string AggregateType { get; set; } = string.Empty;
    public Guid AggregateId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PushEventsRequestDTO
{
    /// <summary>Identificador del Edge Node / sucursal que envía el lote (para trazabilidad).</summary>
    public string? OrigenNodo { get; set; }
    public List<SyncEventDTO> Events { get; set; } = new();
}

public class PushEventsResultDTO
{
    public int Recibidos { get; set; }
    public int Aceptados { get; set; }
    public int YaProcesadosPreviamente { get; set; }
    public List<Guid> IdsAceptados { get; set; } = new();
    public List<Guid> IdsDuplicados { get; set; } = new();
}

public class CatalogItemLiteDTO
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

/// <summary>
/// Catálogos vigentes que un Edge Node necesita para operar de forma autónoma sin conexión
/// (estados de pedido/ticket/item, métodos de pago, tipos de pedido). No incluye el catálogo
/// completo de menú/precios: eso queda para una fase posterior de sincronización de catálogos.
/// </summary>
public class PullCatalogsResultDTO
{
    public DateTime GeneradoEnUtc { get; set; } = DateTime.UtcNow;
    public List<CatalogItemLiteDTO> EstadosPedido { get; set; } = new();
    public List<CatalogItemLiteDTO> EstadosPedidoDetalle { get; set; } = new();
    public List<CatalogItemLiteDTO> EstadosTicketCocina { get; set; } = new();
    public List<CatalogItemLiteDTO> EstadosItemKDS { get; set; } = new();
    public List<CatalogItemLiteDTO> MetodosDePago { get; set; } = new();
    public List<CatalogItemLiteDTO> TiposPedido { get; set; } = new();
}

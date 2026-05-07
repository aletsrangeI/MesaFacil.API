namespace Domain.Entities;

public class TicketDetalle : BaseAuditableEntity // Asume que hereda IdTicketDetalle
{
    public int IdTicket { get; set; }
    public int IdDetalle { get; set; }

    // [CORREGIDO] - Eliminamos las llaves genéricas y usamos el ID tipado
    public int IdEstadoItemKDS { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public TicketCocina Ticket { get; set; } = null!;
    public PedidoDetalle DetallePedido { get; set; } = null!;
    
    // [CORREGIDO] - Navegación directa a la entidad de catálogo específica
    public CatEstadoItemKDS EstadoItemKDS { get; set; } = null!;
}
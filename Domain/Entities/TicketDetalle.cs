namespace Domain.Entities;

public class TicketDetalle : BaseAuditableEntity
{
    public int IdTicket { get; set; }
    public int IdDetalle { get; set; }

    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }

    public TicketCocina Ticket { get; set; } = null!;
    public PedidoDetalle DetallePedido { get; set; } = null!;
    public CatalogItem EstadoItem { get; set; } = null!;
}
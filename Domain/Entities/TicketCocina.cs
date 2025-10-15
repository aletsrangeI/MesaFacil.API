namespace Domain.Entities;

public class TicketCocina : BaseAuditableEntity
{
    public int IdEstacion { get; set; }
    public int IdPedido { get; set; }

    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }
    
    public DateTime? CompletadoEn { get; set; }

    public EstacionCocina Estacion { get; set; } = null!;
    public Pedido Pedido { get; set; } = null!;
    public CatalogItem EstadoItem { get; set; } = null!;

    public ICollection<TicketDetalle> Detalles { get; set; } = new List<TicketDetalle>();
}
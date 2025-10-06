namespace Domain.Entities;

public class PedidoDetalle : BaseAuditableEntity
{
    public int IdPedido { get; set; }
    public int? IdAsiento { get; set; }
    public int IdProducto { get; set; }
    public int? IdVariante { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Notas { get; set; }

    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }

    public int ImpuestoCatalogId { get; set; }
    public int ImpuestoItemId { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public PedidoAsiento? Asiento { get; set; }
    public Producto Producto { get; set; } = null!;
    public VarianteProducto? Variante { get; set; }

    public CatalogItem EstadoItem { get; set; } = null!;
    public CatalogItem ImpuestoItem { get; set; } = null!;

    public ICollection<PedidoModificador> Modificadores { get; set; } = new List<PedidoModificador>();
    public ICollection<TicketDetalle> TicketsDetalle { get; set; } = new List<TicketDetalle>();
}
namespace Domain.Entities;

public class Cuenta : BaseAuditableEntity
{
    public int IdPedido { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DescuentoTotal { get; set; }
    public decimal CargoServicio { get; set; }
    public decimal ImpuestoTotal { get; set; }
    public decimal Total { get; set; }

    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public CatalogItem EstadoItem { get; set; } = null!;

    public ICollection<DetalleCuenta> Detalles { get; set; } = new List<DetalleCuenta>();
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    public ICollection<DescuentoAplicado> Descuentos { get; set; } = new List<DescuentoAplicado>();
}
namespace Domain.Entities;

public class VarianteProducto : BaseAuditableEntity
{
    public int IdProducto { get; set; }
    public string? Nombre { get; set; }
    public string? Codigo { get; set; }
    public bool EsDefault { get; set; } = false;

    public Producto Producto { get; set; } = null!;
    public ICollection<Precio> Precios { get; set; } = new List<Precio>();
    public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
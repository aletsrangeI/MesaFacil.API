namespace Domain.Entities;

public class Producto : BaseAuditableEntity
{
    public int IdMenu { get; set; }
    public int IdCategoria { get; set; }
    public string? Codigo { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    public int? EstacionCatalogId { get; set; }
    public int? EstacionItemId { get; set; }

    public Menu Menu { get; set; } = null!;
    public CategoriaMenu Categoria { get; set; } = null!;
    public CatalogItem? EstacionItem { get; set; }

    public ICollection<VarianteProducto> Variantes { get; set; } = new List<VarianteProducto>();

    public ICollection<Precio> PreciosDeprecatedIgnore { get; set; } =
        new List<Precio>(); // (solo para claridad; precios reales van en Variante)

    public ICollection<GrupoModificador> GruposModificador { get; set; } = new List<GrupoModificador>();
    public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
namespace Domain.Entities;

public class Mesa : BaseAuditableEntity
{
    public int IdSucursal { get; set; }
    public int? IdArea { get; set; }
    public string Codigo { get; set; } = null!;
    public int Asientos { get; set; } = 2;

    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }
    

    public Sucursal Sucursal { get; set; } = null!;
    public Area? Area { get; set; }

    public CatalogItem EstadoItem { get; set; } = null!; // (EstadoCatalogId, EstadoItemId)

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
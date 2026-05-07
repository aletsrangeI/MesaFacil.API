namespace Domain.Entities;

public class Mesa : BaseAuditableEntity // Hereda Id y campos de auditoría (incluyendo IsActive)
{
    public int IdSucursal { get; set; }
    public int? IdArea { get; set; }
    public string Codigo { get; set; } = null!;
    public int Asientos { get; set; } = 2;

    // [CORREGIDO] - Referencia única al catálogo específico
    public int IdEstadoMesa { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Sucursal Sucursal { get; set; } = null!;
    public Area? Area { get; set; }

    // [CORREGIDO] - Navegación tipada
    public virtual CatEstadoMesa EstadoMesa { get; set; } = null!;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
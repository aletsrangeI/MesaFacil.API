namespace Domain.Entities;

public class DescuentoAplicado : BaseAuditableEntity // Hereda Id y auditoría
{
    public int IdCuenta { get; set; }

    // [CORREGIDO] - Referencia única al catálogo tipado
    public int IdTipoDescuento { get; set; }

    public decimal Valor { get; set; }
    public string? Alcance { get; set; }
    public string? Condiciones { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Cuenta Cuenta { get; set; } = null!;
    public virtual CatTipoDescuento TipoDescuento { get; set; } = null!;
}
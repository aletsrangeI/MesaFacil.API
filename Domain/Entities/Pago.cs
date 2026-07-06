namespace Domain.Entities;

public class Pago : BaseAuditableEntity // Hereda Id y campos de auditoría
{
    public int IdCuenta { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "MXN";
    public decimal Propina { get; set; }
    public DateTime PagadoEn { get; set; }
    public string? Referencia { get; set; }
    public int? RecibidoPor { get; set; }

    // [CORREGIDO] - Referencia única al catálogo de métodos de pago
    public int IdMetodoDePago { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Cuenta Cuenta { get; set; } = null!;
    public Usuario? RecibidoPorUsuario { get; set; }
    
    // [CORREGIDO] - Navegación tipada hacia el catálogo específico
    public virtual CatMetodoDePago MetodoDePago { get; set; } = null!;
}
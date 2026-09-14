namespace Domain.Entities;

public class PedidoModificador : BaseAuditableGuidEntity // Spec 019: Id Guid/UUIDv7
{
    public Guid IdDetalle { get; set; }
    public int IdOpcion { get; set; }
    
    // ==========================================
    // SNAPSHOT (Inmutabilidad Histórica)
    // ==========================================
    public string OpcionNombre { get; set; } = string.Empty;
    
    public decimal PrecioExtra { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public PedidoDetalle Detalle { get; set; } = null!;
    public OpcionModificador Opcion { get; set; } = null!;
}
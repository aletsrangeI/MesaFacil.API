namespace Domain.Entities;

public class CatMotivoMovimientoInventario : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public string TipoMovimiento { get; set; } = string.Empty; // EntradaManual, SalidaMerma, AjusteInventario
    public string? Codigo { get; set; }
}

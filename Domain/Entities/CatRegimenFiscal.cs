namespace Domain.Entities;

/// <summary>
/// Catálogo oficial de Regímenes Fiscales del SAT (c_RegimenFiscal para CFDI 4.0 / 3.3).
/// </summary>
public class CatRegimenFiscal : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public bool Fisica { get; set; } = true;
    public bool Moral { get; set; } = true;
}

namespace Domain.Entities;

public class CatConceptoMovimientoCaja : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // Egreso, Ingreso
}

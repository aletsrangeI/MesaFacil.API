namespace Domain.Entities;

public class CatImpuesto : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
namespace Domain.Entities;

public class CatMoneda : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
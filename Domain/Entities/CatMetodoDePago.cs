namespace Domain.Entities;

public class CatMetodoDePago : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
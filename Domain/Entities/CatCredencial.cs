namespace Domain.Entities;

public class CatCredencial : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
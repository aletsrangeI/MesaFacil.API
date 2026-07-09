namespace Domain.Entities;

public class CatEstadoMesa : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
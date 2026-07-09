namespace Domain.Entities;

public class CatTipoDescuento : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
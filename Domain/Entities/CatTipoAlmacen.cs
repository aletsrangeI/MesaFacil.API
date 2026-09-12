namespace Domain.Entities;

public class CatTipoAlmacen : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; }
}

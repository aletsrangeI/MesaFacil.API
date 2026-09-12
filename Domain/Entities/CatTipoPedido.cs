namespace Domain.Entities;

public class CatTipoPedido : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
    public bool IsComedor { get; set; }
}
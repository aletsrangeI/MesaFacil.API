namespace Domain.Entities;

public class CatEstadoPedido : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
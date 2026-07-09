namespace Domain.Entities;

public class CatEstadoPedidoDetalle : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
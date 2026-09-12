namespace Domain.Entities;

public class CatMotivoCancelacionPedido : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
}

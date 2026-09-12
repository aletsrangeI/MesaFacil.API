namespace Domain.Entities;

public class CatCanalVenta : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public bool EsDelivery { get; set; }
}

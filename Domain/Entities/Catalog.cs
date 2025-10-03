namespace Domain.Entities;

public class Catalog : BaseAuditableEntity
{
    public string Code { get; set; }
    public string Name { get; set; }
    public ICollection<CatalogItem> Items { get; set; } = new List<CatalogItem>();
}
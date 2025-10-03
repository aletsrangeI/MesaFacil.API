namespace Domain.Entities;

public class CatalogItem : BaseAuditableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int SortOrder { get; set; } = 0;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? ExtraJson { get; set; }

    public int CatalogId { get; set; }
    public Catalog Catalog { get; set; } = default!;
}
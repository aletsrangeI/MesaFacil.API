namespace DTO.CatalogItem;

public class CatalogItemDTO
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int SortOrder { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? ExtraJson { get; set; }
    public int CatalogId { get; set; }
}

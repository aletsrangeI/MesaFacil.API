namespace DTO.GenericCatalog;

/// <summary>
/// DTO unificado para todas las operaciones CRUD de catálogos simples (Cat*).
/// Contiene únicamente los campos que comparten todas las entidades de este tipo.
/// </summary>
public class GenericCatalogDTO
{
    public int      Id          { get; set; }
    public string   Descripcion { get; set; }
    public string?  Codigo      { get; set; }
    public bool     IsActive    { get; set; }
    public DateTime  CreatedAt  { get; set; }
    public string?   CreatedBy  { get; set; }
    public DateTime? UpdatedAt  { get; set; }
    public string?   UpdatedBy  { get; set; }
}

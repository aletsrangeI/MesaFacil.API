namespace DTO.Producto;

public class ProductoDTO
{
    public int Id { get; set; }
    public int IdMenu { get; set; }
    public int IdCategoria { get; set; }
    public string? Codigo { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public int? EstacionCatalogId { get; set; }
    public int? EstacionItemId { get; set; }
}

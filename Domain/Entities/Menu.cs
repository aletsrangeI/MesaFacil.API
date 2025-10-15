namespace Domain.Entities;

public class Menu : BaseAuditableEntity
{
    public int IdSucursal { get; set; }
    public string? Nombre { get; set; }

    public Sucursal Sucursal { get; set; } = null!;
    public ICollection<CategoriaMenu> Categorias { get; set; } = new List<CategoriaMenu>();
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
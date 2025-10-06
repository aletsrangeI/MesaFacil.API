namespace Domain.Entities;

public class CategoriaMenu : BaseAuditableEntity
{
    public int IdMenu { get; set; }
    public string? Nombre { get; set; }
    public int Orden { get; set; } = 0;

    public Menu Menu { get; set; } = null!;
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
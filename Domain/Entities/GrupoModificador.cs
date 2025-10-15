namespace Domain.Entities;

public class GrupoModificador : BaseAuditableEntity
{
    public int IdProducto { get; set; }
    public string? Nombre { get; set; }
    public int MinSeleccion { get; set; }
    public int MaxSeleccion { get; set; }
    public bool Obligatorio { get; set; }

    public Producto Producto { get; set; } = null!;
    public ICollection<OpcionModificador> Opciones { get; set; } = new List<OpcionModificador>();
}
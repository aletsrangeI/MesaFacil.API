namespace Domain.Entities;

public class Formulario : BaseAuditableEntity
{
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    
    // Relación uno a muchos con sus campos
    public virtual ICollection<FormField> Campos { get; set; } = new List<FormField>();
}
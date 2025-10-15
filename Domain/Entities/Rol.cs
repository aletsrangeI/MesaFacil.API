namespace Domain.Entities;

public class Rol : BaseAuditableEntity
{
    public string Nombre { get; set; } = null!;
    public bool IsSystem { get; set; } = false;
    public bool IsAssignable { get; set; } = true;
    public string? ConcurrencyStamp { get; set; }

    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
}
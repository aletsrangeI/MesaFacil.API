namespace Domain.Entities;

public class UsuarioRol : BaseAuditableEntity
{
    public int IdRol { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Rol Rol { get; set; } = null!;
}
namespace Domain.Entities;

public class AccesoRuta : BaseAuditableEntity
{
    public string Nombre { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string? Descripcion { get; set; }
    
    public string Key { get; set; } = null!;   // p.ej. "USERS_READ"
    public string? Group { get; set; }         // p.ej. "USERS", "ROLES", "ORG"
    public bool IsMenu { get; set; } = false;

    public ICollection<RolAccesoRuta> RolesConAcceso { get; set; } = new List<RolAccesoRuta>();
}
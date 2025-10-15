namespace Domain.Entities;

public class RolAccesoRuta : BaseAuditableEntity
{
    public int IdRol { get; set; }
    public int IdAccesoRuta { get; set; }

    public Rol Rol { get; set; } = null!;
    public AccesoRuta AccesoRuta { get; set; } = null!;
}
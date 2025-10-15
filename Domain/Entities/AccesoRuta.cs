namespace Domain.Entities;

public class AccesoRuta : BaseAuditableEntity
{
    public string Nombre { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<RolAccesoRuta> RolesConAcceso { get; set; } = new List<RolAccesoRuta>();
}
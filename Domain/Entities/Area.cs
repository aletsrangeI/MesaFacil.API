namespace Domain.Entities;

public class Area : BaseAuditableEntity
{
    public int IdSucursal { get; set; }
    public string? Nombre { get; set; }
    public int Orden { get; set; } = 0;
    public DateTime CreadoEn { get; set; }

    public Sucursal Sucursal { get; set; } = null!;
    public ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
}
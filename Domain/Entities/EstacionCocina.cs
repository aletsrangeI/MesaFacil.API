namespace Domain.Entities;

public class EstacionCocina : BaseAuditableEntity
{
    public int IdSucursal { get; set; }
    public string? Nombre { get; set; }

    public Sucursal Sucursal { get; set; } = null!;
    public ICollection<TicketCocina> Tickets { get; set; } = new List<TicketCocina>();
}
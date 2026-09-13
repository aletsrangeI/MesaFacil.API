namespace DTO.TicketDetalle;

public class TicketDetalleDTO
{
    public int Id { get; set; }
    public Guid IdTicket { get; set; }
    public Guid IdDetalle { get; set; }
    public int IdEstadoItemKDS { get; set; }
    public bool Activo { get; set; }
}
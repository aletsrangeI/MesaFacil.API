namespace DTO.TicketDetalle;

public class TicketDetalleDTO
{
    public int Id { get; set; }
    public int IdTicket { get; set; }
    public int IdDetalle { get; set; }
    public int IdEstadoItemKDS { get; set; }
    public bool Activo { get; set; }
}
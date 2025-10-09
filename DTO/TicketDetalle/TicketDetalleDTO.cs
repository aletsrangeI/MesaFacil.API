namespace DTO.TicketDetalle;

public class TicketDetalleDTO
{
    public int Id { get; set; }
    public int IdTicket { get; set; }
    public int IdDetalle { get; set; }
    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }
}
namespace DTO.TicketCocina;

public class TicketCocinaDTO
{
    public int Id { get; set; }
    public int IdEstacion { get; set; }
    public int IdPedido { get; set; }
    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }
    public DateTime? CompletadoEn { get; set; }
}

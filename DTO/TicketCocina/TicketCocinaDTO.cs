namespace DTO.TicketCocina;

public class TicketCocinaDTO
{
    public int Id { get; set; }
    public int IdEstacion { get; set; }
    public int IdPedido { get; set; }
    public int IdEstadoTicketCocina { get; set; }
    public DateTime? CompletadoEn { get; set; }
    public bool Activo { get; set; }
}

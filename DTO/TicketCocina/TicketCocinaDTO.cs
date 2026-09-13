namespace DTO.TicketCocina;

public class TicketCocinaDTO
{
    public Guid Id { get; set; }
    public int IdEstacion { get; set; }
    public Guid IdPedido { get; set; }
    public int IdEstadoTicketCocina { get; set; }
    public DateTime? CompletadoEn { get; set; }
    public bool Activo { get; set; }
}

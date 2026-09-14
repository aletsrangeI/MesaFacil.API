namespace DTO.EventoPedido;

public class EventoPedidoDTO
{
    public int Id { get; set; }
    public Guid IdPedido { get; set; }
    public int? IdUsuario { get; set; }
    public string? TipoEvento { get; set; }
    public string? Payload { get; set; }
}

namespace DTO.EventoPedido;

public class EventoPedidoHistorialDTO
{
    public int Id { get; set; }
    public int IdPedido { get; set; }
    public int? IdUsuario { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? TipoEvento { get; set; }
    public string? Payload { get; set; }
    public DateTime FechaUtc { get; set; }
}

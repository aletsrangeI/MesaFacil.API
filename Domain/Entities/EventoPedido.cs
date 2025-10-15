namespace Domain.Entities;

public class EventoPedido : BaseAuditableEntity
{
    public int IdPedido { get; set; }
    public int? IdUsuario { get; set; }
    public string? TipoEvento { get; set; }
    public string? Payload { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Usuario? Usuario { get; set; }
}
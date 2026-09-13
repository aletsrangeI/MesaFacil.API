namespace Domain.Entities;

public class EventoPedido : BaseAuditableEntity // Id propio permanece int; FK a Pedido es Guid (Spec 019)
{
    public Guid IdPedido { get; set; }
    public int? IdUsuario { get; set; }
    public string? TipoEvento { get; set; }
    public string? Payload { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Usuario? Usuario { get; set; }
}
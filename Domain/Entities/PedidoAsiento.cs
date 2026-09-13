namespace Domain.Entities;

public class PedidoAsiento : BaseAuditableGuidEntity // Spec 019: Id Guid/UUIDv7
{
    public Guid IdPedido { get; set; }
    public int NumeroAsiento { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
}
namespace Domain.Entities;

public class PedidoAsiento : BaseAuditableEntity
{
    public int IdPedido { get; set; }
    public int NumeroAsiento { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
}
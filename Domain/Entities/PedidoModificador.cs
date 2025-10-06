namespace Domain.Entities;

public class PedidoModificador : BaseAuditableEntity
{
    public int IdDetalle { get; set; }
    public int IdOpcion { get; set; }
    public decimal PrecioExtra { get; set; }

    public PedidoDetalle Detalle { get; set; } = null!;
    public OpcionModificador Opcion { get; set; } = null!;
}
namespace DTO.PedidoDetalle;

public class PedidoDetalleDTO
{
    public Guid Id { get; set; }
    public Guid IdPedido { get; set; }
    public Guid? IdAsiento { get; set; }
    public int IdProducto { get; set; }
    public int? IdVariante { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Notas { get; set; }
    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }
    public int ImpuestoCatalogId { get; set; }
    public int ImpuestoItemId { get; set; }
}

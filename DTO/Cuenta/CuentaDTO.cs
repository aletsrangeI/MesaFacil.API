namespace DTO.Cuenta;

public class CuentaDTO
{
    public int Id { get; set; }
    public int IdPedido { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DescuentoTotal { get; set; }
    public decimal CargoServicio { get; set; }
    public decimal ImpuestoTotal { get; set; }
    public decimal Total { get; set; }
    public DateTime CreadaEn { get; set; }
    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }
}
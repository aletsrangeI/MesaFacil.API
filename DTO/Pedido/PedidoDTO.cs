namespace DTO.Pedido;

public class PedidoDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }
    public int? IdCliente { get; set; }
    public int? AbiertoPor { get; set; }
    public int? CerradoPor { get; set; }
    public DateTime AbiertoEn { get; set; }
    public DateTime? CerradoEn { get; set; }
    public string? Notas { get; set; }
    public int IdTipoPedido { get; set; }
    public int IdEstadoPedido { get; set; }
    public decimal CargoServicioPct { get; set; }
}
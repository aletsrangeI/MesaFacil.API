namespace DTO.Pedido;

public class PedidoDTO
{
    public Guid Id { get; set; }
    public int FolioDiario { get; set; }
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
    public string CanalOrigen { get; set; } = "POS";
    public string? IdExterno { get; set; }
    public string? NombreClienteDelivery { get; set; }
    public string? TelefonoDelivery { get; set; }
    public string? DireccionEntrega { get; set; }
    public string? NombreRepartidor { get; set; }
    public string? TelefonoRepartidor { get; set; }
    public DateTime? DespachadoEn { get; set; }
    public DateTime? EntregadoEn { get; set; }
}
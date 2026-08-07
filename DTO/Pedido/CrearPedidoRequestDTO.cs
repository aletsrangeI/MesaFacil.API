using System.Collections.Generic;

namespace DTO.Pedido;

public class CrearPedidoRequestDTO
{
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }
    public int? IdCliente { get; set; }
    public int Personas { get; set; } = 1;
    public string? Notas { get; set; }
    public int IdTipoPedido { get; set; }
    public int IdEstadoPedido { get; set; }
    public decimal CargoServicioPct { get; set; }
    
    public List<CrearPedidoDetalleDTO> Detalles { get; set; } = new List<CrearPedidoDetalleDTO>();
}

public class CrearPedidoDetalleDTO
{
    public int IdProducto { get; set; }
    public int IdVariante { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public string VarianteNombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; } = 1m;
    public decimal PrecioUnitario { get; set; }
    public int IdImpuesto { get; set; }
    public decimal TasaImpuesto { get; set; }
    public decimal MontoImpuesto { get; set; }
    public string? Notas { get; set; }
    public int IdEstadoPedidoDetalle { get; set; }
    public List<int> OpcionesModificador { get; set; } = new List<int>();
}

namespace Domain.Entities;

public class Pedido
{
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }
    public int? IdCliente { get; set; }

    public int? AbiertoPor { get; set; }
    public int? CerradoPor { get; set; }
    public DateTime AbiertoEn { get; set; }
    public DateTime? CerradoEn { get; set; }
    public string? Notas { get; set; }

    public int TipoCatalogId { get; set; }
    public int TipoItemId { get; set; }

    public int EstadoCatalogId { get; set; }
    public int EstadoItemId { get; set; }

    public decimal CargoServicioPct { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public Sucursal Sucursal { get; set; } = null!;
    public Mesa? Mesa { get; set; }
    public Cliente? Cliente { get; set; }

    public Usuario? AbiertoPorUsuario { get; set; }
    public Usuario? CerradoPorUsuario { get; set; }

    public CatalogItem TipoItem { get; set; } = null!;
    public CatalogItem EstadoItem { get; set; } = null!;

    public ICollection<PedidoAsiento> Asientos { get; set; } = new List<PedidoAsiento>();
    public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    public ICollection<EventoPedido> Eventos { get; set; } = new List<EventoPedido>();
    public ICollection<TicketCocina> TicketsCocina { get; set; } = new List<TicketCocina>();
    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}
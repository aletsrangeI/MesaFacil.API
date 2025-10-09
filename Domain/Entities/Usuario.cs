namespace Domain.Entities;

public class Usuario : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Correo { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
    public ICollection<Credencial> Credenciales { get; set; } = new List<Credencial>();
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();

    public ICollection<Pedido> PedidosAbiertos { get; set; } = new List<Pedido>();
    public ICollection<Pedido> PedidosCerrados { get; set; } = new List<Pedido>();
    public ICollection<EventoPedido> EventosPedido { get; set; } = new List<EventoPedido>();
    public ICollection<Pago> PagosRecibidos { get; set; } = new List<Pago>();
    public ICollection<CorteCaja> CortesCajaCreados { get; set; } = new List<CorteCaja>();
}
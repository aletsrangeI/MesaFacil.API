namespace Domain.Entities;

public class Usuario : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }
    public int? IdSucursal { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Correo { get; set; }

    // ==========================================
    // Spec 024: Candado de Supervisor (PIN de 4 dígitos)
    // ==========================================
    // Reutiliza el mismo esquema de hashing (Pbkdf2PasswordHasher / IPasswordHasher) que ya usa
    // el proyecto para contraseñas y PINs de login rápido (Credencial), en vez de agregar una
    // dependencia nueva (BCrypt). Se agrega PinSupervisorSalt junto a PinSupervisorHash porque
    // IPasswordHasher.Verify requiere hash + salt por separado (no estaba contemplado en el
    // draft original del spec, que sólo mencionaba el campo de hash).
    public string? PinSupervisorHash { get; set; }
    public string? PinSupervisorSalt { get; set; }
    public int PinIntentosFallidos { get; set; } = 0;
    public DateTime? PinBloqueadoHasta { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public Sucursal? Sucursal { get; set; }
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
    public ICollection<Credencial> Credenciales { get; set; } = new List<Credencial>();
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();

    public ICollection<Pedido> PedidosAbiertos { get; set; } = new List<Pedido>();
    public ICollection<Pedido> PedidosCerrados { get; set; } = new List<Pedido>();
    public ICollection<EventoPedido> EventosPedido { get; set; } = new List<EventoPedido>();
    public ICollection<Pago> PagosRecibidos { get; set; } = new List<Pago>();
    public ICollection<CorteCaja> CortesCajaCreados { get; set; } = new List<CorteCaja>();
}
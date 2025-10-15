namespace Domain.Entities;

public class Empresa : BaseAuditableEntity
{
    public string? Nombre { get; set; }
    public string? Rfc { get; set; }

    public ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
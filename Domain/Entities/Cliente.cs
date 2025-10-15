namespace Domain.Entities;

public class Cliente : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
namespace Domain.Entities;

public class Sucursal : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }
    public string? Nombre { get; set; }
    public string? Direccion { get; set; }
    public string? ZonaHoraria { get; set; }

    public Empresa Empresa { get; set; } = null!;
    public ICollection<Area> Areas { get; set; } = new List<Area>();
    public ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();
    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<EstacionCocina> EstacionesCocina { get; set; } = new List<EstacionCocina>();
    public ICollection<CorteCaja> CortesCaja { get; set; } = new List<CorteCaja>();
}
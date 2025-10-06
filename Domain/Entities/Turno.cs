namespace Domain.Entities;

public class Turno : BaseAuditableEntity
{
    public int IdUsuario { get; set; }
    public int IdSucursal { get; set; }
    public DateTime Apertura { get; set; }
    public DateTime? Cierre { get; set; }
    public decimal CajaInicial { get; set; }
    public decimal? CajaFinal { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Sucursal Sucursal { get; set; } = null!;

    public ICollection<MovimientoCaja> MovimientosCaja { get; set; } = new List<MovimientoCaja>();
    public ICollection<CorteCaja> CortesCaja { get; set; } = new List<CorteCaja>();
}
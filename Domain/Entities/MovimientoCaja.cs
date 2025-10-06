namespace Domain.Entities;

public class MovimientoCaja : BaseAuditableEntity
{
    public int IdTurno { get; set; }
    public string Tipo { get; set; } = null!; // 'Egreso' | 'Ingreso' | 'Deposito' ...
    public decimal Monto { get; set; }
    public string? Nota { get; set; }
    public DateTime CreadoEn { get; set; }

    public Turno Turno { get; set; } = null!;
}
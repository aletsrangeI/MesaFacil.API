namespace Domain.Entities;

public class MovimientoCaja : BaseAuditableGuidEntity // Spec 019: Id Guid/UUIDv7
{
    public int IdTurno { get; set; }
    public string Tipo { get; set; } = null!; // 'Egreso' | 'Ingreso' | 'Deposito' ...
    public decimal Monto { get; set; }
    public string? Nota { get; set; }

    public Turno Turno { get; set; } = null!;
}
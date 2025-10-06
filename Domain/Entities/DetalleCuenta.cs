namespace Domain.Entities;

public class DetalleCuenta : BaseAuditableEntity
{
    public int IdCuenta { get; set; }
    public string TipoOrigen { get; set; } = null!; // 'Item' | 'Descuento' | 'Servicio' ...
    public int? IdOrigen { get; set; }
    public string? Descripcion { get; set; }
    public decimal Monto { get; set; }
    public Cuenta Cuenta { get; set; } = null!;
}
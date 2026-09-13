namespace Domain.Entities;

/// <summary>
/// Spec 019: contador human-readable correlativo por sucursal/día usado para
/// <see cref="Pedido.FolioDiario"/>. El PK de negocio (IdSucursal, Fecha) se incrementa
/// de forma atómica vía UPSERT (INSERT ... ON CONFLICT DO UPDATE ... RETURNING) para
/// garantizar unicidad bajo alta concurrencia sin necesidad de leer-incrementar-guardar en memoria.
/// </summary>
public class FoliadorSucursal : BaseEntity
{
    public int IdSucursal { get; set; }
    public DateOnly Fecha { get; set; }
    public int UltimoFolio { get; set; }

    public Sucursal Sucursal { get; set; } = null!;
}

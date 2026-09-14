using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("PagosCuentaPorPagar")]
public class PagoCuentaPorPagar : BaseAuditableEntity
{
    public int IdCuentaPorPagar { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Monto { get; set; }

    public DateTime FechaPago { get; set; } = DateTime.UtcNow;

    public int IdMetodoPago { get; set; }

    /// <summary>
    /// ID del movimiento de caja generado si el pago provino de la caja chica del turno activo.
    /// Spec 019: MovimientoCaja.Id es Guid/UUIDv7.
    /// </summary>
    public Guid? IdMovimientoCaja { get; set; }

    [MaxLength(100)]
    public string? ReferenciaBancaria { get; set; }

    [MaxLength(500)]
    public string? ComprobanteUrl { get; set; }

    public int? IdUsuario { get; set; }

    [MaxLength(500)]
    public string? Observaciones { get; set; }

    [ForeignKey(nameof(IdCuentaPorPagar))]
    public virtual CuentaPorPagar? CuentaPorPagar { get; set; }

    [ForeignKey(nameof(IdMetodoPago))]
    public virtual CatMetodoDePago? MetodoPago { get; set; }

    [ForeignKey(nameof(IdMovimientoCaja))]
    public virtual MovimientoCaja? MovimientoCaja { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual Usuario? Usuario { get; set; }
}

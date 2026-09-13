using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Spec 020: Renglón (Concepto CFDI) de una FacturaVenta. PedidoDetalleId es Guid? porque
/// PedidoDetalle.Id ya es Guid/UUIDv7 desde Spec 019.
/// </summary>
[Table("FacturaVentaDetalles")]
public class FacturaVentaDetalle : BaseAuditableEntity
{
    public int IdFacturaVenta { get; set; }

    /// <summary>
    /// Spec 019: PedidoDetalle.Id es Guid/UUIDv7. Nulo si el renglón no proviene 1:1 de un
    /// PedidoDetalle (ej. renglón consolidado o ajuste manual).
    /// </summary>
    public Guid? PedidoDetalleId { get; set; }

    /// <summary>
    /// Clave del catálogo c_ClaveProdServ del SAT (ej. 90101501 - Restaurantes).
    /// </summary>
    [Required]
    [MaxLength(8)]
    public string ClaveProdServ { get; set; } = string.Empty;

    /// <summary>
    /// Clave del catálogo c_ClaveUnidad del SAT (ej. H87 - Pieza, E48 - Unidad de servicio).
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string ClaveUnidad { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,4)")]
    public decimal Cantidad { get; set; }

    [Required]
    [MaxLength(300)]
    public string Descripcion { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,4)")]
    public decimal ValorUnitario { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Importe { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal BaseIva { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TasaIva { get; set; } = 0.16m;

    [Column(TypeName = "decimal(18,4)")]
    public decimal ImporteIva { get; set; }

    [ForeignKey(nameof(IdFacturaVenta))]
    public virtual FacturaVenta? FacturaVenta { get; set; }
}

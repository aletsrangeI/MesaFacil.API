using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("CompraFacturaDetalles")]
public class CompraFacturaDetalle : BaseEntity
{
    public int IdCompraFactura { get; set; }

    public int IdInsumo { get; set; }

    [MaxLength(20)]
    public string? ClaveProdServ { get; set; }

    [MaxLength(250)]
    public string? DescripcionOriginal { get; set; }

    [MaxLength(20)]
    public string? UnidadSAT { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Cantidad { get; set; }

    public int? IdUnidadMedida { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal FactorConversion { get; set; } = 1.0m;

    /// <summary>
    /// Cantidad calculada en la unidad de medida base del insumo (= Cantidad * FactorConversion)
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal CantidadInsumo { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostoUnitario { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Importe { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Descuento { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TasaIVA { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal ImporteIVA { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TasaIEPS { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal ImporteIEPS { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal ImporteTotal { get; set; }

    [ForeignKey(nameof(IdCompraFactura))]
    public virtual CompraFactura? CompraFactura { get; set; }

    [ForeignKey(nameof(IdInsumo))]
    public virtual Insumo? Insumo { get; set; }

    [ForeignKey(nameof(IdUnidadMedida))]
    public virtual UnidadMedida? UnidadMedida { get; set; }
}

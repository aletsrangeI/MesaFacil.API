using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("KardexMovimientos")]
public class KardexMovimiento : BaseEntity
{
    public int IdAlmacen { get; set; }

    public int IdInsumo { get; set; }

    [Required]
    [MaxLength(50)]
    public string TipoMovimiento { get; set; } = string.Empty; // EntradaManual, SalidaMerma, TraspasoSalida, TraspasoEntrada, AjusteInventario, ConsumoVenta

    [MaxLength(100)]
    public string? Submotivo { get; set; } // Caducidad, Caída/Accidente, Descomposición, Degustación/Cortesía, CompraCajaChica, AjusteFisico, etc.

    [Column(TypeName = "decimal(18,4)")]
    public decimal Cantidad { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostoUnitario { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostoTotal { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal SaldoAnterior { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal SaldoNuevo { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostoPromedioResultante { get; set; }

    [MaxLength(100)]
    public string? DocumentoReferencia { get; set; }

    [MaxLength(500)]
    public string? Observaciones { get; set; }

    public int? IdUsuario { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(IdAlmacen))]
    public virtual Almacen? Almacen { get; set; }

    [ForeignKey(nameof(IdInsumo))]
    public virtual Insumo? Insumo { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual Usuario? Usuario { get; set; }
}

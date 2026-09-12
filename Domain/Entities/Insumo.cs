using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Insumos")]
public class Insumo : BaseAuditableEntity
{
    [Required]
    [MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    public int IdCategoriaInsumo { get; set; }

    public int IdUnidadMedidaBase { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostoPromedio { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal UltimoCosto { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal StockMinimo { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal StockMaximo { get; set; }

    public bool EsCritico { get; set; }

    [ForeignKey(nameof(IdCategoriaInsumo))]
    public virtual CategoriaInsumo? CategoriaInsumo { get; set; }

    [ForeignKey(nameof(IdUnidadMedidaBase))]
    public virtual UnidadMedida? UnidadMedidaBase { get; set; }

    public virtual ICollection<InventarioExistencia> Existencias { get; set; } = new List<InventarioExistencia>();
    public virtual ICollection<KardexMovimiento> MovimientosKardex { get; set; } = new List<KardexMovimiento>();
}

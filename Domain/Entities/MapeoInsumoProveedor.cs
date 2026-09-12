using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("MapeosInsumoProveedor")]
public class MapeoInsumoProveedor : BaseEntity
{
    public int IdProveedor { get; set; }

    [Required]
    [MaxLength(250)]
    public string DescripcionSAT { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ClaveProdServ { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? UnidadSAT { get; set; }

    public int IdInsumo { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal FactorConversion { get; set; } = 1.0m;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime? FechaUltimaCompra { get; set; }

    [ForeignKey(nameof(IdProveedor))]
    public virtual Proveedor? Proveedor { get; set; }

    [ForeignKey(nameof(IdInsumo))]
    public virtual Insumo? Insumo { get; set; }
}

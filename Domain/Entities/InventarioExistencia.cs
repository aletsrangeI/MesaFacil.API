using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("InventarioExistencias")]
public class InventarioExistencia : BaseEntity
{
    public int IdAlmacen { get; set; }

    public int IdInsumo { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal StockActual { get; set; }

    public DateTime FechaUltimoMovimiento { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(IdAlmacen))]
    public virtual Almacen? Almacen { get; set; }

    [ForeignKey(nameof(IdInsumo))]
    public virtual Insumo? Insumo { get; set; }
}

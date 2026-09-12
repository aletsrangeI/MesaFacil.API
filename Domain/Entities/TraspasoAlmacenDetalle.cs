using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("TraspasoAlmacenDetalles")]
public class TraspasoAlmacenDetalle : BaseEntity
{
    public int IdTraspasoAlmacen { get; set; }

    public int IdInsumo { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Cantidad { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? CantidadRecibida { get; set; }

    [ForeignKey(nameof(IdTraspasoAlmacen))]
    public virtual TraspasoAlmacen? TraspasoAlmacen { get; set; }

    [ForeignKey(nameof(IdInsumo))]
    public virtual Insumo? Insumo { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("RecetaDetalles")]
public class RecetaDetalle : BaseAuditableEntity
{
    public int IdReceta { get; set; }

    public int? IdInsumo { get; set; }

    public int? IdSubReceta { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Cantidad { get; set; }

    public int IdUnidadMedida { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal PorcentajeMermaEsperada { get; set; } = 0m;

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostoCalculado { get; set; } = 0m;

    // Navegación
    [ForeignKey(nameof(IdReceta))]
    public virtual Receta Receta { get; set; } = null!;

    [ForeignKey(nameof(IdInsumo))]
    public virtual Insumo? Insumo { get; set; }

    [ForeignKey(nameof(IdSubReceta))]
    public virtual Receta? SubReceta { get; set; }

    [ForeignKey(nameof(IdUnidadMedida))]
    public virtual UnidadMedida UnidadMedida { get; set; } = null!;
}

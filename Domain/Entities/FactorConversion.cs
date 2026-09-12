using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("FactoresConversion")]
public class FactorConversion : BaseEntity
{
    public int IdUnidadOrigen { get; set; }
    public int IdUnidadDestino { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal Factor { get; set; }

    [ForeignKey(nameof(IdUnidadOrigen))]
    public virtual UnidadMedida? UnidadOrigen { get; set; }

    [ForeignKey(nameof(IdUnidadDestino))]
    public virtual UnidadMedida? UnidadDestino { get; set; }
}

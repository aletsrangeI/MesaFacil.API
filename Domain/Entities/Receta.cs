using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Recetas")]
public class Receta : BaseAuditableEntity
{
    public int? IdProducto { get; set; }
    public int? IdVariante { get; set; }
    public int? IdOpcionModificador { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    public bool EsSubReceta { get; set; } = false;

    [Column(TypeName = "decimal(18,4)")]
    public decimal Rendimiento { get; set; } = 1m;

    public int IdUnidadMedidaRendimiento { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal CostoEstimadoUnitario { get; set; } = 0m;

    // Navegación
    [ForeignKey(nameof(IdProducto))]
    public virtual Producto? Producto { get; set; }

    [ForeignKey(nameof(IdVariante))]
    public virtual VarianteProducto? Variante { get; set; }

    [ForeignKey(nameof(IdOpcionModificador))]
    public virtual OpcionModificador? OpcionModificador { get; set; }

    [ForeignKey(nameof(IdUnidadMedidaRendimiento))]
    public virtual UnidadMedida? UnidadMedidaRendimiento { get; set; }

    public virtual ICollection<RecetaDetalle> Detalles { get; set; } = new List<RecetaDetalle>();
}

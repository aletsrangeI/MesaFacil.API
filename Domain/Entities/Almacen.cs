using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("Almacenes")]
public class Almacen : BaseAuditableEntity
{
    public int IdSucursal { get; set; }

    [Required]
    [MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string TipoAlmacen { get; set; } = "General"; // General, Cocina, Barra, Bodega

    public bool EsPrincipal { get; set; }

    [ForeignKey(nameof(IdSucursal))]
    public virtual Sucursal? Sucursal { get; set; }

    public virtual ICollection<InventarioExistencia> Existencias { get; set; } = new List<InventarioExistencia>();
}

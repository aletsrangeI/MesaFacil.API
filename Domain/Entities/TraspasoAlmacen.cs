using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("TraspasosAlmacen")]
public class TraspasoAlmacen : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Folio { get; set; } = string.Empty;

    public int IdAlmacenOrigen { get; set; }

    public int IdAlmacenDestino { get; set; }

    [Required]
    [MaxLength(30)]
    public string Estado { get; set; } = "Completado"; // Pendiente, Completado, Cancelado

    public int IdUsuarioSolicita { get; set; }

    public int? IdUsuarioRecibe { get; set; }

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

    public DateTime? FechaRecepcion { get; set; }

    [MaxLength(500)]
    public string? Observaciones { get; set; }

    [ForeignKey(nameof(IdAlmacenOrigen))]
    public virtual Almacen? AlmacenOrigen { get; set; }

    [ForeignKey(nameof(IdAlmacenDestino))]
    public virtual Almacen? AlmacenDestino { get; set; }

    [ForeignKey(nameof(IdUsuarioSolicita))]
    public virtual Usuario? UsuarioSolicita { get; set; }

    [ForeignKey(nameof(IdUsuarioRecibe))]
    public virtual Usuario? UsuarioRecibe { get; set; }

    public virtual ICollection<TraspasoAlmacenDetalle> Detalles { get; set; } = new List<TraspasoAlmacenDetalle>();
}

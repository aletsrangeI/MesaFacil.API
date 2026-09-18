using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("FilaEsperaItems")]
public class FilaEsperaItem : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }

    [Required]
    [MaxLength(100)]
    public string NombreCliente { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string TelefonoCliente { get; set; } = string.Empty;

    public int NumeroPersonas { get; set; } = 2;

    [MaxLength(50)]
    public string? ZonaPreferencia { get; set; }

    public int MinutosEstimados { get; set; } = 15;

    public DateTime RegistradoEn { get; set; } = DateTime.UtcNow;

    public DateTime? NotificadoEn { get; set; }

    [Required]
    [MaxLength(30)]
    public string Estado { get; set; } = "EnEspera"; // EnEspera, Notificado, Sentado, Cancelado

    public int? IdMesaAsignada { get; set; }

    // Navegación
    [ForeignKey(nameof(IdMesaAsignada))]
    public virtual Mesa? MesaAsignada { get; set; }

    [ForeignKey(nameof(IdSucursal))]
    public virtual Sucursal? Sucursal { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("ReservasMesa")]
public class ReservaMesa : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }

    [Required]
    [MaxLength(100)]
    public string NombreCliente { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string TelefonoCliente { get; set; } = string.Empty;

    public DateTime FechaHoraReserva { get; set; }

    public int NumeroPersonas { get; set; } = 2;

    [MaxLength(50)]
    public string? ZonaPreferencia { get; set; }

    [Required]
    [MaxLength(30)]
    public string EstadoReserva { get; set; } = "Confirmada"; // Confirmada, Sentada, Cancelada, NoShow

    [Column(TypeName = "decimal(18,2)")]
    public decimal AnticipoPagado { get; set; } = 0m;

    [MaxLength(500)]
    public string? Notas { get; set; }

    // Navegación
    [ForeignKey(nameof(IdMesa))]
    public virtual Mesa? Mesa { get; set; }

    [ForeignKey(nameof(IdSucursal))]
    public virtual Sucursal? Sucursal { get; set; }
}

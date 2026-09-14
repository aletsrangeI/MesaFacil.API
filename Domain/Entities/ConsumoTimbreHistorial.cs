using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Spec 020: Auditoría transaccional de la bolsa de timbres (consumos por timbrado
/// y recargas por compra de paquete adicional).
/// </summary>
[Table("ConsumosTimbreHistorial")]
public class ConsumoTimbreHistorial : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    public int IdEmpresaBolsaTimbres { get; set; }

    /// <summary>
    /// Consumo | Recarga
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Tipo { get; set; } = TipoMovimientoTimbre.Consumo;

    /// <summary>
    /// Cantidad de timbres movidos (siempre positiva); el signo lo determina Tipo.
    /// </summary>
    public int Cantidad { get; set; }

    public int SaldoResultante { get; set; }

    /// <summary>
    /// Id de la FacturaVenta que originó el consumo (nulo en recargas de paquete).
    /// </summary>
    public int? IdFacturaVenta { get; set; }

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey(nameof(IdEmpresaBolsaTimbres))]
    public virtual EmpresaBolsaTimbres? BolsaTimbres { get; set; }

    [ForeignKey(nameof(IdFacturaVenta))]
    public virtual FacturaVenta? FacturaVenta { get; set; }
}

public static class TipoMovimientoTimbre
{
    public const string Consumo = "Consumo";
    public const string Recarga = "Recarga";
}

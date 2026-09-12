using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("CuentasPorPagar")]
public class CuentaPorPagar : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    public int IdSucursal { get; set; }

    public int IdProveedor { get; set; }

    /// <summary>
    /// ID de la factura de compra en ComprasFactura que originó este pasivo (opcional si es pasivo directo).
    /// </summary>
    public int? IdCompraFactura { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal MontoTotal { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal SaldoInsoluto { get; set; }

    public DateTime FechaEmision { get; set; }

    public DateTime FechaVencimiento { get; set; }

    /// <summary>
    /// Estados: Pendiente, Abonada, Pagada, Cancelada
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Estado { get; set; } = "Pendiente";

    [MaxLength(500)]
    public string? Observaciones { get; set; }

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey(nameof(IdSucursal))]
    public virtual Sucursal? Sucursal { get; set; }

    [ForeignKey(nameof(IdProveedor))]
    public virtual Proveedor? Proveedor { get; set; }

    [ForeignKey(nameof(IdCompraFactura))]
    public virtual CompraFactura? CompraFactura { get; set; }

    public virtual ICollection<PagoCuentaPorPagar> Pagos { get; set; } = new List<PagoCuentaPorPagar>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("ComprasFactura")]
public class CompraFactura : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    public int IdSucursal { get; set; }

    public int IdAlmacen { get; set; }

    public int IdProveedor { get; set; }

    /// <summary>
    /// Folio Fiscal Digital SAT (UUID). 36 caracteres. Único por factura.
    /// Nullable si es remisión o nota de compra manual sin CFDI.
    /// </summary>
    [MaxLength(36)]
    public string? UUID { get; set; }

    [MaxLength(20)]
    public string? Serie { get; set; }

    [Required]
    [MaxLength(50)]
    public string Folio { get; set; } = string.Empty;

    public DateTime FechaEmision { get; set; }

    public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;

    public bool EsCredito { get; set; } = false;

    public int DiasCredito { get; set; } = 0;

    public DateTime? FechaVencimiento { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalDescuento { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalIVA { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalIEPS { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Total { get; set; }

    /// <summary>
    /// Estados posibles: Borrador, Aplicada, Cancelada
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Estado { get; set; } = "Borrador";

    [MaxLength(500)]
    public string? RutaArchivoXML { get; set; }

    [MaxLength(500)]
    public string? RutaArchivoPDF { get; set; }

    [MaxLength(500)]
    public string? Observaciones { get; set; }

    public int? IdUsuario { get; set; }

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey(nameof(IdSucursal))]
    public virtual Sucursal? Sucursal { get; set; }

    [ForeignKey(nameof(IdAlmacen))]
    public virtual Almacen? Almacen { get; set; }

    [ForeignKey(nameof(IdProveedor))]
    public virtual Proveedor? Proveedor { get; set; }

    [ForeignKey(nameof(IdUsuario))]
    public virtual Usuario? Usuario { get; set; }

    public virtual ICollection<CompraFacturaDetalle> Detalles { get; set; } = new List<CompraFacturaDetalle>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Spec 020: Maestro del CFDI 4.0 emitido a un comensal por la venta de un Pedido.
/// Id se mantiene int (no forma parte de las entidades operativas migradas en Spec 019),
/// pero PedidoId es Guid porque Pedido.Id ya es Guid/UUIDv7 desde Spec 019.
/// </summary>
[Table("FacturasVenta")]
public class FacturaVenta : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    public int IdSucursal { get; set; }

    /// <summary>
    /// Spec 019: Pedido.Id es Guid/UUIDv7. Único (no se puede facturar dos veces el mismo pedido
    /// mientras la factura esté vigente/activa).
    /// </summary>
    public Guid PedidoId { get; set; }

    /// <summary>
    /// Folio fiscal digital SAT (UUID v4 del timbrado). Único.
    /// </summary>
    [MaxLength(36)]
    public string? UUID { get; set; }

    [MaxLength(20)]
    public string Serie { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Folio { get; set; } = string.Empty;

    public DateTime? FechaTimbrado { get; set; }

    [Required]
    [MaxLength(13)]
    public string RfcReceptor { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string NombreReceptor { get; set; } = string.Empty;

    [MaxLength(3)]
    public string RegimenFiscalReceptor { get; set; } = string.Empty;

    [MaxLength(5)]
    public string CodigoPostalReceptor { get; set; } = string.Empty;

    /// <summary>
    /// Clave de Uso CFDI del catálogo c_UsoCFDI (ej. G03, S01).
    /// </summary>
    [MaxLength(3)]
    public string UsoCfdi { get; set; } = string.Empty;

    /// <summary>
    /// Clave del catálogo c_FormaPago (ej. 01 Efectivo, 04 Tarjeta de crédito, 28 Tarjeta de débito).
    /// </summary>
    [MaxLength(2)]
    public string FormaPago { get; set; } = string.Empty;

    /// <summary>
    /// PUE (Pago en una sola exhibición) o PPD (Pago en parcialidades o diferido).
    /// </summary>
    [MaxLength(3)]
    public string MetodoPago { get; set; } = "PUE";

    [Column(TypeName = "decimal(18,4)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Descuento { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Iva { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal Total { get; set; }

    public string? CadenaOriginalSat { get; set; }

    public string? SelloDigitalSat { get; set; }

    public string? SelloDigitalEmisor { get; set; }

    [MaxLength(30)]
    public string? NoCertificadoSat { get; set; }

    /// <summary>
    /// XML timbrado completo (Comprobante + TimbreFiscalDigital), tal como quedó sellado por el PAC.
    /// </summary>
    public string? XmlSellado { get; set; }

    /// <summary>
    /// Vigente | Cancelada | Error
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string EstadoFiscal { get; set; } = EstadoFiscalFactura.Pendiente;

    /// <summary>
    /// Clave del catálogo c_MotivoCancelacion del SAT (01, 02, 03, 04).
    /// </summary>
    [MaxLength(2)]
    public string? MotivoCancelacion { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    /// <summary>
    /// GUID entregado al comensal en el ticket térmico (QR) para autofacturación. Único.
    /// </summary>
    public Guid TicketAutofacturaGuid { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Vigencia límite (en días naturales) para que el comensal pueda autofacturar este ticket.
    /// </summary>
    public DateTime? VigenciaAutofactura { get; set; }

    public bool GeneradaPorAutofactura { get; set; } = false;

    public DateTime? CorreoEnviadoEn { get; set; }

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }

    [ForeignKey(nameof(IdSucursal))]
    public virtual Sucursal? Sucursal { get; set; }

    public virtual ICollection<FacturaVentaDetalle> Detalles { get; set; } = new List<FacturaVentaDetalle>();
}

public static class EstadoFiscalFactura
{
    public const string Pendiente = "Pendiente";
    public const string Vigente = "Vigente";
    public const string Cancelada = "Cancelada";
    public const string Error = "Error";
}

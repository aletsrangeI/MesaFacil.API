using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Spec 020: Configuración de facturación fiscal (CFDI 4.0) por Empresa.
/// Guarda el proveedor PAC elegido, credenciales, Certificado de Sello Digital (CSD)
/// y datos del emisor requeridos para timbrar. IdEmpresa sigue siendo int (no migrado a Guid).
/// </summary>
[Table("EmpresaConfiguracionesPAC")]
public class EmpresaConfiguracionPAC : BaseAuditableEntity
{
    public int IdEmpresa { get; set; }

    /// <summary>
    /// Proveedor PAC seleccionado: Mock, Finkok, Facturama, SwSapien.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string ProveedorPAC { get; set; } = ProveedorPacNombres.Mock;

    [MaxLength(200)]
    public string? PacApiKey { get; set; }

    [MaxLength(200)]
    public string? PacApiSecret { get; set; }

    public bool EsProduccion { get; set; } = false;

    /// <summary>
    /// Certificado de Sello Digital (.cer) en Base64. Nulo mientras se use MockPac.
    /// </summary>
    public string? CertificadoCerBase64 { get; set; }

    /// <summary>
    /// Llave privada (.key) en Base64. Nulo mientras se use MockPac.
    /// </summary>
    public string? LlaveKeyBase64 { get; set; }

    /// <summary>
    /// Contraseña de la llave privada, cifrada en reposo (no se almacena en texto plano).
    /// </summary>
    [MaxLength(500)]
    public string? PasswordKeyCifrado { get; set; }

    [Required]
    [MaxLength(20)]
    public string SerieFacturacion { get; set; } = "F";

    public int FolioSiguiente { get; set; } = 1;

    /// <summary>
    /// Código postal del lugar de expedición del comprobante (LugarExpedicion CFDI).
    /// </summary>
    [Required]
    [MaxLength(5)]
    public string LugarExpedicionCP { get; set; } = string.Empty;

    [MaxLength(13)]
    public string RfcEmisor { get; set; } = string.Empty;

    [MaxLength(300)]
    public string RazonSocialEmisor { get; set; } = string.Empty;

    /// <summary>
    /// Clave del catálogo c_RegimenFiscal del SAT (ej. 601, 612, 621).
    /// </summary>
    [MaxLength(3)]
    public string RegimenFiscalEmisor { get; set; } = string.Empty;

    [ForeignKey(nameof(IdEmpresa))]
    public virtual Empresa? Empresa { get; set; }
}

public static class ProveedorPacNombres
{
    public const string Mock = "Mock";
    public const string Finkok = "Finkok";
    public const string Facturama = "Facturama";
    public const string SwSapien = "SwSapien";
}

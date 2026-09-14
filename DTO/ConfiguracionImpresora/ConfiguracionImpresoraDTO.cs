namespace DTO.ConfiguracionImpresora;

/// <summary>
/// Spec 023: DTO plano de configuración de impresora térmica (CRUD).
/// </summary>
public class ConfiguracionImpresoraDTO
{
    public int Id { get; set; }
    public int IdSucursal { get; set; }
    public string? Nombre { get; set; }

    /// <summary>"RedLAN", "USBLocal" o "NavegadorDialogo".</summary>
    public string TipoConexion { get; set; } = "RedLAN";

    /// <summary>58 u 80 (mm).</summary>
    public int AnchoPapel { get; set; } = 80;

    public string? DireccionIp { get; set; }
    public int Puerto { get; set; } = 9100;
    public bool AperturaCajon { get; set; }
    public bool Autocorte { get; set; } = true;
    public string? EstacionAsociada { get; set; }
}

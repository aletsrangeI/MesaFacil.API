namespace DTO.Impresoras;

/// <summary>
/// Spec 023, sección 3 del spec.md: forma EXACTA de la respuesta de
/// POST /api/impresoras/diagnostico/{idImpresora}. Los nombres de propiedad se serializan
/// a camelCase por la política por defecto de System.Text.Json en ASP.NET Core.
/// </summary>
public class DiagnosticoImpresoraResponseDTO
{
    public int IdImpresora { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? DireccionIp { get; set; }
    public int Puerto { get; set; }

    /// <summary>Latencia de conexión TCP en milisegundos. Null cuando no aplica (USB/Navegador) o no fue posible medirla.</summary>
    public int? LatenciaMs { get; set; }

    /// <summary>"Ok", "NoAlcanzable", "TapaAbierta", "SinPapel", "ErrorHardware".</summary>
    public string Estado { get; set; } = string.Empty;

    public bool TapaAbierta { get; set; }
    public bool SinPapel { get; set; }
    public string MensajeDiagnostico { get; set; } = string.Empty;
    public string AccionSugerida { get; set; } = string.Empty;
}

using DTO.Suscripcion;

namespace Interface.Suscripciones;

/// <summary>
/// Spec 033: Servicio para consultar e invalidar el estado operativo y de suscripción de un Tenant (Empresa).
/// Utiliza IMemoryCache para garantizar tiempo de respuesta ultra-rápido (< 1ms) en el pipeline HTTP.
/// </summary>
public interface ITenantStatusService
{
    /// <summary>
    /// Obtiene el estado actual del tenant evaluando caché en memoria y la base de datos si expiró.
    /// </summary>
    Task<TenantStatusResult> ObtenerEstadoTenantAsync(int empresaId);

    /// <summary>
    /// Invalida la entrada en memoria de la empresa para forzar recarga en el siguiente request.
    /// </summary>
    void InvalidarCacheTenant(int empresaId);

    /// <summary>
    /// Aplica una instrucción recibida desde OrionSys Central Hub (Spec 034) o panel administrativo.
    /// Actualiza la base de datos, purga la caché y emite notificaciones en tiempo real si corresponde.
    /// </summary>
    Task<bool> ActualizarEstadoDesdeHubAsync(ActualizarEstadoLicenciaRequestDTO request);
}

public class TenantStatusResult
{
    public int EmpresaId { get; set; }
    public string EstadoSuscripcion { get; set; } = "Activa";
    public bool EstaSuspendido { get; set; }
    public bool EnPeriodoGracia { get; set; }
    public DateTime? FechaFinVigencia { get; set; }
    public string? MotivoSuspension { get; set; }
    public string? ContactoWhatsApp { get; set; }
}

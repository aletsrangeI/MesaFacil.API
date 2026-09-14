using Common;
using DTO.Seguridad;

namespace Interface.UseCases;

/// <summary>
/// Spec 024: candado de supervisor con PIN de 4 dígitos. Cubre configuración del PIN,
/// autorización de acciones protegidas (cancelar platillo en cocina, descuento excesivo,
/// cancelar cuenta) con rate-limiting anti fuerza-bruta, y validación del token efímero
/// (60 segundos) emitido tras una autorización exitosa.
/// </summary>
public interface ISupervisorPinSecurityService
{
    Task<Response<bool>> ConfigurarPinAsync(ConfigurarPinRequestDTO request, int idUsuarioSolicitante, CancellationToken ct = default);

    Task<AutorizarSupervisorPinResponseDTO> AutorizarAsync(AutorizarSupervisorPinRequestDTO request, CancellationToken ct = default);

    /// <summary>
    /// Valida el token de autorización efímero emitido por AutorizarAsync: firma, expiración,
    /// y que el claim "accionProtegida" + "idPedidoDetalle" coincidan con el recurso solicitado.
    /// Usado por el endpoint de borrado de PedidoDetalle (restricción infranqueable, criterio #1).
    /// </summary>
    bool ValidarTokenAutorizacion(string? token, string accionEsperada, Guid idPedidoDetalle, out int? idUsuarioSupervisor);
}

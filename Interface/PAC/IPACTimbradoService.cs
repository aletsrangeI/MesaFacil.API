using DTO.Facturacion;

namespace Interface.PAC;

/// <summary>
/// Spec 020: abstracción agnóstica de proveedor PAC para timbrado de CFDI 4.0.
/// Permite conectar MockPac (desarrollo/tests), Finkok, Facturama o SW Sapien sin cambiar
/// una sola línea de la lógica de negocio de facturación (AutofacturacionComensalService,
/// FacturasVentaController).
/// </summary>
public interface IPACTimbradoService
{
    /// <summary>
    /// Nombre corto del proveedor implementado (debe coincidir con Domain.Entities.ProveedorPacNombres).
    /// </summary>
    string NombreProveedor { get; }

    /// <summary>
    /// Envía el comprobante sellado por el emisor al PAC para su timbrado ante el SAT.
    /// </summary>
    Task<TimbradoResultadoDTO> TimbrarAsync(TimbrarCfdiRequestDTO request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Solicita al PAC/SAT la cancelación de un CFDI previamente timbrado.
    /// </summary>
    Task<CancelarCfdiResultadoDTO> CancelarAsync(CancelarCfdiRequestDTO request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta el saldo de timbres disponible directamente en el PAC (cuando aplique).
    /// </summary>
    Task<ConsultarSaldoResultadoDTO> ConsultarSaldoAsync(CancellationToken cancellationToken = default);
}

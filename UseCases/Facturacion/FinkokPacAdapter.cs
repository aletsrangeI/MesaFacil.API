using Domain.Entities;
using DTO.Facturacion;
using Interface.PAC;

namespace UseCases.Facturacion;

/// <summary>
/// Spec 020: adaptador para el webservice SOAP de Finkok (https://wsdl.finkok.com).
/// Se define la forma real de la integración (endpoints de prueba/producción, autenticación
/// por usuario/password de la cuenta Finkok) pero NO se implementa la llamada SOAP real
/// porque este entorno no tiene credenciales de una cuenta Finkok real ni se puede verificar
/// el contrato WSDL exacto sin ellas. Devuelve un error controlado "credenciales no
/// configuradas" en vez de simular un timbrado real, que sería engañoso para producción.
///
/// Para producción falta:
///  - Alta de cuenta en Finkok (usuario/contraseña de la cuenta, no PacApiKey/PacApiSecret
///    genéricos) y su UUID de servicio "stamp"/"cancel" según su WSDL vigente.
///  - Referenciar el paquete de cliente SOAP (System.ServiceModel / WCF Core) o construir el
///    sobre SOAP manualmente contra https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl
///    (pruebas) y https://facturacion.finkok.com/servicios/soap/stamp.wsdl (producción).
///  - Manejo de respuesta SOAP (UUID, Sello, SelloSAT, NoCertificadoSAT, XML timbrado en Base64).
/// </summary>
public class FinkokPacAdapter : IPACTimbradoService
{
    private readonly HttpClient _httpClient;

    private const string UrlPruebas = "https://demo-facturacion.finkok.com/servicios/soap/stamp.wsdl";
    private const string UrlProduccion = "https://facturacion.finkok.com/servicios/soap/stamp.wsdl";

    public FinkokPacAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string NombreProveedor => ProveedorPacNombres.Finkok;

    public Task<TimbradoResultadoDTO> TimbrarAsync(TimbrarCfdiRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PacApiKey) || string.IsNullOrWhiteSpace(request.PacApiSecret))
        {
            return Task.FromResult(new TimbradoResultadoDTO
            {
                Exitoso = false,
                MensajeError = "Finkok no está configurado: faltan credenciales (usuario/contraseña) de la cuenta Finkok en EmpresaConfiguracionPAC."
            });
        }

        // TODO producción: armar sobre SOAP "stamp" contra UrlProduccion/UrlPruebas según
        // request.EsProduccion, usando _httpClient, y parsear la respuesta (UUID, sellos, XML).
        throw new NotImplementedException(
            "FinkokPacAdapter.TimbrarAsync no está implementado: requiere credenciales reales de Finkok y verificación del contrato SOAP vigente antes de integrarse a producción.");
    }

    public Task<CancelarCfdiResultadoDTO> CancelarAsync(CancelarCfdiRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PacApiKey) || string.IsNullOrWhiteSpace(request.PacApiSecret))
        {
            return Task.FromResult(new CancelarCfdiResultadoDTO
            {
                Exitoso = false,
                MensajeError = "Finkok no está configurado: faltan credenciales de la cuenta Finkok en EmpresaConfiguracionPAC."
            });
        }

        throw new NotImplementedException(
            "FinkokPacAdapter.CancelarAsync no está implementado: requiere credenciales reales de Finkok.");
    }

    public Task<ConsultarSaldoResultadoDTO> ConsultarSaldoAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ConsultarSaldoResultadoDTO
        {
            Exitoso = false,
            MensajeError = "Finkok no está configurado: faltan credenciales de la cuenta Finkok en EmpresaConfiguracionPAC."
        });
    }
}

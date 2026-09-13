using Domain.Entities;
using DTO.Facturacion;
using Interface.PAC;

namespace UseCases.Facturacion;

/// <summary>
/// Spec 020: adaptador para la API REST/JSON de Facturama (https://api.facturama.mx).
/// Se define la forma real de la integración (autenticación Basic con ApiKey/ApiSecret,
/// endpoint /api/3/cfdis) pero NO se implementa la llamada HTTP real porque este entorno no
/// tiene credenciales de una cuenta Facturama real. Devuelve un error controlado en vez de
/// inventar un contrato JSON no verificado.
///
/// Para producción falta:
///  - Alta de cuenta en Facturama y sus credenciales (usuario/password de la API, Basic Auth).
///  - Mapear TimbrarCfdiRequestDTO al esquema JSON de Facturama (CfdiType, Receiver, Items,
///    etc. — su modelo NO es un CFDI XML directo, es un DTO propio que ellos traducen a CFDI).
///  - Endpoint real: POST https://api.facturama.mx/api/3/cfdis (o apisandbox.facturama.mx en pruebas).
///  - Manejo de respuesta (Id, Complement.TaxStamp.Uuid, Complement.TaxStamp.SatSign, etc.).
/// </summary>
public class FacturamaPacAdapter : IPACTimbradoService
{
    private readonly HttpClient _httpClient;

    private const string UrlPruebas = "https://apisandbox.facturama.mx";
    private const string UrlProduccion = "https://api.facturama.mx";

    public FacturamaPacAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string NombreProveedor => ProveedorPacNombres.Facturama;

    public Task<TimbradoResultadoDTO> TimbrarAsync(TimbrarCfdiRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PacApiKey) || string.IsNullOrWhiteSpace(request.PacApiSecret))
        {
            return Task.FromResult(new TimbradoResultadoDTO
            {
                Exitoso = false,
                MensajeError = "Facturama no está configurado: faltan credenciales (usuario/password de API) en EmpresaConfiguracionPAC."
            });
        }

        // TODO producción: construir el payload JSON propio de Facturama (no es CFDI XML directo)
        // y hacer POST autenticado con Basic Auth contra UrlProduccion/UrlPruebas usando _httpClient.
        throw new NotImplementedException(
            "FacturamaPacAdapter.TimbrarAsync no está implementado: requiere credenciales reales de Facturama y mapear el esquema JSON propio de su API antes de integrarse a producción.");
    }

    public Task<CancelarCfdiResultadoDTO> CancelarAsync(CancelarCfdiRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PacApiKey) || string.IsNullOrWhiteSpace(request.PacApiSecret))
        {
            return Task.FromResult(new CancelarCfdiResultadoDTO
            {
                Exitoso = false,
                MensajeError = "Facturama no está configurado: faltan credenciales de API en EmpresaConfiguracionPAC."
            });
        }

        throw new NotImplementedException(
            "FacturamaPacAdapter.CancelarAsync no está implementado: requiere credenciales reales de Facturama.");
    }

    public Task<ConsultarSaldoResultadoDTO> ConsultarSaldoAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ConsultarSaldoResultadoDTO
        {
            Exitoso = false,
            MensajeError = "Facturama no está configurado: faltan credenciales de API en EmpresaConfiguracionPAC."
        });
    }
}

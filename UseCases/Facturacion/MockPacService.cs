using System.Text.RegularExpressions;
using System.Xml.Linq;
using Domain.Entities;
using DTO.Facturacion;
using Interface.PAC;

namespace UseCases.Facturacion;

/// <summary>
/// Spec 020: implementación de IPACTimbradoService completamente funcional sin dependencias
/// externas. Es la ÚNICA implementación que funciona end-to-end sin credenciales reales:
/// desarrollo local, pruebas unitarias y entornos de demostración. Genera UUID v4 y sellos
/// digitales sintéticos con formato de texto plausible, y siempre tiene éxito salvo que el
/// RFC del receptor sea inválido.
/// </summary>
public class MockPacService : IPACTimbradoService
{
    private static readonly Regex RfcRegex = new(
        @"^([A-ZÑ&]{3,4})\d{6}[A-Z0-9]{3}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string NombreProveedor => ProveedorPacNombres.Mock;

    public Task<TimbradoResultadoDTO> TimbrarAsync(TimbrarCfdiRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (!EsRfcValido(request.RfcReceptor))
        {
            return Task.FromResult(new TimbradoResultadoDTO
            {
                Exitoso = false,
                MensajeError = $"MockPac rechazó el timbrado: el RFC receptor '{request.RfcReceptor}' no tiene un formato válido."
            });
        }

        var uuid = Guid.NewGuid().ToString().ToUpperInvariant();
        var fechaTimbrado = DateTime.UtcNow;

        // Sello digital SAT sintético con formato de texto plausible (Base64), no verificable
        // criptográficamente contra la CSD real del SAT: es exclusivo de MockPac para pruebas.
        var selloSat = ConstruirSelloSintetico(uuid, "SAT");
        var noCertificadoSat = "00001000000700000000"; // formato de 20 dígitos, valor sintético fijo de pruebas

        var xmlTimbrado = InsertarTimbreFiscalDigital(
            request.XmlSellado, uuid, fechaTimbrado, selloSat, noCertificadoSat, request.SelloDigitalEmisor ?? string.Empty);

        return Task.FromResult(new TimbradoResultadoDTO
        {
            Exitoso = true,
            UUID = uuid,
            FechaTimbrado = fechaTimbrado,
            SelloDigitalSat = selloSat,
            SelloDigitalEmisor = request.SelloDigitalEmisor,
            NoCertificadoSat = noCertificadoSat,
            CadenaOriginalSat = request.CadenaOriginalSat,
            XmlTimbrado = xmlTimbrado
        });
    }

    public Task<CancelarCfdiResultadoDTO> CancelarAsync(CancelarCfdiRequestDTO request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UUID))
        {
            return Task.FromResult(new CancelarCfdiResultadoDTO
            {
                Exitoso = false,
                MensajeError = "MockPac no pudo cancelar: falta el UUID del comprobante."
            });
        }

        return Task.FromResult(new CancelarCfdiResultadoDTO
        {
            Exitoso = true,
            EstatusCancelacion = "Cancelado"
        });
    }

    public Task<ConsultarSaldoResultadoDTO> ConsultarSaldoAsync(CancellationToken cancellationToken = default)
    {
        // MockPac no tiene bolsa remota propia: el control real de saldo se hace en
        // EmpresaBolsaTimbres (BD local). Se responde éxito sin dato numérico.
        return Task.FromResult(new ConsultarSaldoResultadoDTO
        {
            Exitoso = true,
            TimbresDisponibles = null
        });
    }

    private static bool EsRfcValido(string? rfc)
    {
        if (string.IsNullOrWhiteSpace(rfc)) return false;
        rfc = rfc.Trim().ToUpperInvariant();
        if (rfc.Length != 12 && rfc.Length != 13) return false;
        if (rfc == "XAXX010101000" || rfc == "XEXX010101000") return true;
        return RfcRegex.IsMatch(rfc);
    }

    private static string ConstruirSelloSintetico(string uuid, string sufijo)
    {
        var payload = $"MOCKPAC-{sufijo}-{uuid}-{DateTime.UtcNow.Ticks}";
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload));
    }

    private static string InsertarTimbreFiscalDigital(
        string xmlSellado, string uuid, DateTime fechaTimbrado, string selloSat, string noCertificadoSat, string selloCfd)
    {
        XNamespace cfdiNs = "http://www.sat.gob.mx/cfd/4";
        XNamespace tfdNs = "http://www.sat.gob.mx/TimbreFiscalDigital";

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xmlSellado);
        }
        catch
        {
            // Si el XML de entrada no es parseable (p.ej. pruebas unitarias con XML mínimo),
            // se devuelve tal cual sin timbre insertado; el llamador ya recibió el UUID por
            // separado en TimbradoResultadoDTO.
            return xmlSellado;
        }

        var comprobante = doc.Root;
        if (comprobante == null) return xmlSellado;

        var complemento = comprobante.Element(cfdiNs + "Complemento");
        if (complemento == null)
        {
            complemento = new XElement(cfdiNs + "Complemento");
            comprobante.Add(complemento);
        }

        complemento.Add(new XElement(tfdNs + "TimbreFiscalDigital",
            new XAttribute(XNamespace.Xmlns + "tfd", tfdNs.NamespaceName),
            new XAttribute("Version", "1.1"),
            new XAttribute("UUID", uuid),
            new XAttribute("FechaTimbrado", fechaTimbrado.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XAttribute("RfcProvCertif", "MOCKPAC010101ABC"),
            new XAttribute("SelloCFD", selloCfd),
            new XAttribute("NoCertificadoSAT", noCertificadoSat),
            new XAttribute("SelloSAT", selloSat)));

        return doc.ToString(SaveOptions.DisableFormatting);
    }
}

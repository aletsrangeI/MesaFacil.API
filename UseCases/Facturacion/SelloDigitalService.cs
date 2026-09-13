using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Domain.Entities;

namespace UseCases.Facturacion;

public class SelloResultado
{
    public string CadenaOriginal { get; set; } = string.Empty;
    public string SelloDigital { get; set; } = string.Empty;
    public string? NoCertificado { get; set; }

    /// <summary>
    /// True cuando el sello se generó con un certificado .cer/.key real (RSA-SHA256).
    /// False cuando se usó el sellado simulado (SHA256 Base64) por no haber CSD cargado
    /// (flujo normal con MockPacService).
    /// </summary>
    public bool EsSelloReal { get; set; }

    public string? Advertencia { get; set; }
}

public interface ISelloDigitalService
{
    /// <summary>
    /// Calcula la cadena original del comprobante concatenando los campos relevantes
    /// separados por "|", siguiendo el principio del Anexo 20 (posición fija de campos).
    /// No implementa la plantilla XSLT oficial completa del SAT (no es requerida para
    /// que el flujo con MockPac funcione end-to-end), pero conserva el mismo criterio:
    /// concatenación determinista y estable de los campos fiscales del comprobante.
    /// </summary>
    string CalcularCadenaOriginal(EmpresaConfiguracionPAC emisor, FacturaVenta factura);

    /// <summary>
    /// Sella la cadena original. Si la Empresa tiene CSD real cargado (CertificadoCerBase64 +
    /// LlaveKeyBase64), intenta un sellado real con RSA-SHA256. Si no hay CSD real, o si el
    /// certificado es inválido, cae a un sellado SIMULADO (hash SHA256 en Base64) marcado con
    /// EsSelloReal = false, sin bloquear el resto del flujo (MockPacService no requiere sello real).
    /// </summary>
    SelloResultado Sellar(EmpresaConfiguracionPAC emisor, string cadenaOriginal, string? passwordKeyDescifrada = null);
}

public class SelloDigitalService : ISelloDigitalService
{
    public string CalcularCadenaOriginal(EmpresaConfiguracionPAC emisor, FacturaVenta factura)
    {
        // Concatenación determinista de los campos fiscales principales, en posición fija,
        // delimitados por "|" (criterio general del Anexo 20; no es la plantilla XSLT oficial).
        var campos = new[]
        {
            "||", // marcador de inicio, análogo a "||" del Anexo 20
            factura.Serie,
            factura.Folio,
            (factura.FechaTimbrado ?? DateTime.UtcNow).ToString("yyyy-MM-ddTHH:mm:ss"),
            emisor.RfcEmisor,
            emisor.RegimenFiscalEmisor,
            factura.RfcReceptor,
            factura.UsoCfdi,
            factura.Subtotal.ToString("F2"),
            factura.Descuento.ToString("F2"),
            factura.Iva.ToString("F2"),
            factura.Total.ToString("F2"),
            factura.MetodoPago,
            factura.FormaPago,
            "||"
        };

        return string.Join("|", campos);
    }

    public SelloResultado Sellar(EmpresaConfiguracionPAC emisor, string cadenaOriginal, string? passwordKeyDescifrada = null)
    {
        bool tieneCsdReal = !string.IsNullOrWhiteSpace(emisor.CertificadoCerBase64)
                             && !string.IsNullOrWhiteSpace(emisor.LlaveKeyBase64);

        if (tieneCsdReal)
        {
            try
            {
                return SellarConCsdReal(emisor, cadenaOriginal, passwordKeyDescifrada);
            }
            catch (Exception ex)
            {
                // No bloqueante: el resto del flujo (MockPac) puede continuar con sello simulado.
                var simulado = SellarSimulado(cadenaOriginal);
                simulado.Advertencia = $"No fue posible sellar con el CSD real, se usó sello simulado. Detalle: {ex.Message}";
                return simulado;
            }
        }

        return SellarSimulado(cadenaOriginal);
    }

    private static SelloResultado SellarConCsdReal(EmpresaConfiguracionPAC emisor, string cadenaOriginal, string? password)
    {
        byte[] cerBytes = Convert.FromBase64String(emisor.CertificadoCerBase64!);
        byte[] keyBytes = Convert.FromBase64String(emisor.LlaveKeyBase64!);

        // El .key del SAT viene en PKCS#8 cifrado en DER. Se intenta importar directamente;
        // en producción, aquí se requeriría además convertir el .key del SAT (formato propietario
        // no estándar PKCS#8) usando OpenSSL u otra utilidad previa a esta llamada.
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(keyBytes, out _);

        using var certificado = new X509Certificate2(cerBytes);

        byte[] datos = Encoding.UTF8.GetBytes(cadenaOriginal);
        byte[] firma = rsa.SignData(datos, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return new SelloResultado
        {
            CadenaOriginal = cadenaOriginal,
            SelloDigital = Convert.ToBase64String(firma),
            NoCertificado = certificado.SerialNumber,
            EsSelloReal = true
        };
    }

    private static SelloResultado SellarSimulado(string cadenaOriginal)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(cadenaOriginal));

        return new SelloResultado
        {
            CadenaOriginal = cadenaOriginal,
            SelloDigital = Convert.ToBase64String(hash),
            NoCertificado = "00000000000000000000", // No certificado sintético (20 dígitos), formato SAT
            EsSelloReal = false
        };
    }
}

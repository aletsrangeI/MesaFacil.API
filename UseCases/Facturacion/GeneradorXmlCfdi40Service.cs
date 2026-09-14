using System.Globalization;
using System.Xml.Linq;
using Domain.Entities;

namespace UseCases.Facturacion;

/// <summary>
/// Insumo (renglón) para generar un concepto del CFDI. Se arma a partir de
/// FacturaVentaDetalle antes de persistir, o directamente de PedidoDetalle.
/// </summary>
public class ConceptoCfdiInput
{
    public string ClaveProdServ { get; set; } = string.Empty;
    public string ClaveUnidad { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal ValorUnitario { get; set; }
    public decimal TasaIva { get; set; } = 0.16m;
}

/// <summary>
/// Resultado del cálculo de un concepto ya redondeado a 2 decimales según reglas SAT
/// (el redondeo se hace por concepto, no sobre el total acumulado).
/// </summary>
public class ConceptoCfdiCalculado
{
    public string ClaveProdServ { get; set; } = string.Empty;
    public string ClaveUnidad { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal ValorUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal BaseIva { get; set; }
    public decimal TasaIva { get; set; }
    public decimal ImporteIva { get; set; }
}

public class ComprobanteCfdiTotales
{
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
}

public interface IGeneradorXmlCfdi40Service
{
    /// <summary>
    /// Calcula, con redondeo a 2 decimales por concepto (regla SAT Anexo 20), los importes
    /// base/IVA de cada renglón y los totales del comprobante.
    /// </summary>
    List<ConceptoCfdiCalculado> CalcularConceptos(IEnumerable<ConceptoCfdiInput> conceptos);

    ComprobanteCfdiTotales CalcularTotales(IEnumerable<ConceptoCfdiCalculado> conceptos, decimal descuento = 0);

    /// <summary>
    /// Genera el XML del comprobante CFDI 4.0 (Comprobante, Emisor, Receptor, Conceptos, Impuestos),
    /// SIN timbrar (sin nodo TimbreFiscalDigital). El sellado/timbrado lo agrega SelloDigitalService
    /// y el IPACTimbradoService correspondiente.
    /// </summary>
    string GenerarXmlComprobante(EmpresaConfiguracionPAC emisor, FacturaVenta factura, List<ConceptoCfdiCalculado> conceptos);
}

public class GeneradorXmlCfdi40Service : IGeneradorXmlCfdi40Service
{
    private static readonly XNamespace CfdiNs = "http://www.sat.gob.mx/cfd/4";
    private const string CfdiVersion = "4.0";

    public List<ConceptoCfdiCalculado> CalcularConceptos(IEnumerable<ConceptoCfdiInput> conceptos)
    {
        var resultado = new List<ConceptoCfdiCalculado>();

        foreach (var c in conceptos)
        {
            // Regla SAT: el importe del concepto se calcula y redondea a 2 decimales
            // ANTES de calcular el impuesto trasladado sobre esa base ya redondeada.
            decimal importe = Math.Round(c.Cantidad * c.ValorUnitario, 2, MidpointRounding.AwayFromZero);
            decimal baseIva = importe;
            decimal importeIva = Math.Round(baseIva * c.TasaIva, 2, MidpointRounding.AwayFromZero);

            resultado.Add(new ConceptoCfdiCalculado
            {
                ClaveProdServ = c.ClaveProdServ,
                ClaveUnidad = c.ClaveUnidad,
                Cantidad = c.Cantidad,
                Descripcion = c.Descripcion,
                ValorUnitario = Math.Round(c.ValorUnitario, 2, MidpointRounding.AwayFromZero),
                Importe = importe,
                BaseIva = baseIva,
                TasaIva = c.TasaIva,
                ImporteIva = importeIva
            });
        }

        return resultado;
    }

    public ComprobanteCfdiTotales CalcularTotales(IEnumerable<ConceptoCfdiCalculado> conceptos, decimal descuento = 0)
    {
        var lista = conceptos.ToList();
        decimal subtotal = Math.Round(lista.Sum(c => c.Importe), 2, MidpointRounding.AwayFromZero);
        decimal iva = Math.Round(lista.Sum(c => c.ImporteIva), 2, MidpointRounding.AwayFromZero);
        decimal descuentoRedondeado = Math.Round(descuento, 2, MidpointRounding.AwayFromZero);
        decimal total = Math.Round(subtotal - descuentoRedondeado + iva, 2, MidpointRounding.AwayFromZero);

        return new ComprobanteCfdiTotales
        {
            Subtotal = subtotal,
            Descuento = descuentoRedondeado,
            Iva = iva,
            Total = total
        };
    }

    public string GenerarXmlComprobante(EmpresaConfiguracionPAC emisor, FacturaVenta factura, List<ConceptoCfdiCalculado> conceptos)
    {
        var inv = CultureInfo.InvariantCulture;

        var comprobante = new XElement(CfdiNs + "Comprobante",
            new XAttribute(XNamespace.Xmlns + "cfdi", CfdiNs.NamespaceName),
            new XAttribute("Version", CfdiVersion),
            new XAttribute("Serie", factura.Serie),
            new XAttribute("Folio", factura.Folio),
            new XAttribute("Fecha", (factura.FechaTimbrado ?? DateTime.UtcNow).ToString("yyyy-MM-ddTHH:mm:ss", inv)),
            new XAttribute("FormaPago", factura.FormaPago),
            new XAttribute("SubTotal", factura.Subtotal.ToString("F2", inv)),
            new XAttribute("Descuento", factura.Descuento.ToString("F2", inv)),
            new XAttribute("Moneda", "MXN"),
            new XAttribute("Total", factura.Total.ToString("F2", inv)),
            new XAttribute("TipoDeComprobante", "I"),
            new XAttribute("MetodoPago", factura.MetodoPago),
            new XAttribute("LugarExpedicion", emisor.LugarExpedicionCP),

            new XElement(CfdiNs + "Emisor",
                new XAttribute("Rfc", emisor.RfcEmisor),
                new XAttribute("Nombre", emisor.RazonSocialEmisor),
                new XAttribute("RegimenFiscal", emisor.RegimenFiscalEmisor)),

            new XElement(CfdiNs + "Receptor",
                new XAttribute("Rfc", factura.RfcReceptor),
                new XAttribute("Nombre", factura.NombreReceptor),
                new XAttribute("DomicilioFiscalReceptor", factura.CodigoPostalReceptor),
                new XAttribute("RegimenFiscalReceptor", factura.RegimenFiscalReceptor),
                new XAttribute("UsoCFDI", factura.UsoCfdi)),

            new XElement(CfdiNs + "Conceptos",
                conceptos.Select(c =>
                    new XElement(CfdiNs + "Concepto",
                        new XAttribute("ClaveProdServ", c.ClaveProdServ),
                        new XAttribute("Cantidad", c.Cantidad.ToString("F2", inv)),
                        new XAttribute("ClaveUnidad", c.ClaveUnidad),
                        new XAttribute("Descripcion", c.Descripcion),
                        new XAttribute("ValorUnitario", c.ValorUnitario.ToString("F2", inv)),
                        new XAttribute("Importe", c.Importe.ToString("F2", inv)),
                        new XAttribute("ObjetoImp", "02"),
                        new XElement(CfdiNs + "Impuestos",
                            new XElement(CfdiNs + "Traslados",
                                new XElement(CfdiNs + "Traslado",
                                    new XAttribute("Base", c.BaseIva.ToString("F2", inv)),
                                    new XAttribute("Impuesto", "002"),
                                    new XAttribute("TipoFactor", "Tasa"),
                                    new XAttribute("TasaOCuota", c.TasaIva.ToString("F6", inv)),
                                    new XAttribute("Importe", c.ImporteIva.ToString("F2", inv))))))
                )),

            new XElement(CfdiNs + "Impuestos",
                new XAttribute("TotalImpuestosTrasladados", factura.Iva.ToString("F2", inv)),
                new XElement(CfdiNs + "Traslados",
                    new XElement(CfdiNs + "Traslado",
                        new XAttribute("Base", factura.Subtotal.ToString("F2", inv)),
                        new XAttribute("Impuesto", "002"),
                        new XAttribute("TipoFactor", "Tasa"),
                        new XAttribute("TasaOCuota", "0.160000"),
                        new XAttribute("Importe", factura.Iva.ToString("F2", inv)))))
        );

        var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), comprobante);
        return doc.ToString(SaveOptions.DisableFormatting);
    }
}

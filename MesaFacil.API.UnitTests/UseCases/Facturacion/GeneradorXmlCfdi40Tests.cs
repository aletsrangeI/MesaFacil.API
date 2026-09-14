using Domain.Entities;
using FluentAssertions;
using UseCases.Facturacion;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Facturacion;

public class GeneradorXmlCfdi40Tests
{
    private readonly GeneradorXmlCfdi40Service _sut = new();

    [Fact]
    public void CalcularConceptos_RedondeaImporteYIvaADosDecimales()
    {
        // Cantidad * ValorUnitario = 3 * 33.333 = 99.999 -> debe redondear a 100.00,
        // y el IVA se calcula SOBRE la base ya redondeada: 100.00 * 0.16 = 16.00
        var conceptos = new List<ConceptoCfdiInput>
        {
            new() { ClaveProdServ = "90101501", ClaveUnidad = "E48", Cantidad = 3, ValorUnitario = 33.333m, Descripcion = "Platillo", TasaIva = 0.16m }
        };

        var resultado = _sut.CalcularConceptos(conceptos);

        resultado.Should().HaveCount(1);
        resultado[0].Importe.Should().Be(100.00m);
        resultado[0].BaseIva.Should().Be(100.00m);
        resultado[0].ImporteIva.Should().Be(16.00m);
    }

    [Fact]
    public void CalcularTotales_SumaConceptosYRedondeaTotalADosDecimales()
    {
        var conceptos = _sut.CalcularConceptos(new List<ConceptoCfdiInput>
        {
            new() { ClaveProdServ = "90101501", ClaveUnidad = "E48", Cantidad = 2, ValorUnitario = 150.505m, Descripcion = "A", TasaIva = 0.16m },
            new() { ClaveProdServ = "90101501", ClaveUnidad = "E48", Cantidad = 1, ValorUnitario = 89.995m, Descripcion = "B", TasaIva = 0.16m }
        });

        var totales = _sut.CalcularTotales(conceptos);

        totales.Subtotal.Should().Be(conceptos.Sum(c => c.Importe));
        totales.Iva.Should().Be(conceptos.Sum(c => c.ImporteIva));
        totales.Total.Should().Be(Math.Round(totales.Subtotal - totales.Descuento + totales.Iva, 2));
    }

    [Fact]
    public void GenerarXmlComprobante_ContieneNodosPrincipalesDeCfdi40()
    {
        var emisor = new EmpresaConfiguracionPAC
        {
            RfcEmisor = "BME8808116B1",
            RazonSocialEmisor = "Bistró La Central SA de CV",
            RegimenFiscalEmisor = "601",
            LugarExpedicionCP = "83000"
        };

        var factura = new FacturaVenta
        {
            Serie = "F",
            Folio = "1",
            RfcReceptor = "XAXX010101000",
            NombreReceptor = "PUBLICO EN GENERAL",
            RegimenFiscalReceptor = "616",
            CodigoPostalReceptor = "83000",
            UsoCfdi = "S01",
            FormaPago = "01",
            MetodoPago = "PUE",
            Subtotal = 100.00m,
            Descuento = 0m,
            Iva = 16.00m,
            Total = 116.00m,
            FechaTimbrado = DateTime.UtcNow
        };

        var conceptos = _sut.CalcularConceptos(new List<ConceptoCfdiInput>
        {
            new() { ClaveProdServ = "90101501", ClaveUnidad = "E48", Cantidad = 1, ValorUnitario = 100m, Descripcion = "Consumo restaurante", TasaIva = 0.16m }
        });

        var xml = _sut.GenerarXmlComprobante(emisor, factura, conceptos);

        xml.Should().Contain("Comprobante");
        xml.Should().Contain("Emisor");
        xml.Should().Contain("Receptor");
        xml.Should().Contain("Conceptos");
        xml.Should().Contain("Concepto");
        xml.Should().Contain("Impuestos");
        xml.Should().Contain("Traslado");
        xml.Should().Contain("BME8808116B1");
        xml.Should().Contain("XAXX010101000");
        xml.Should().Contain("Total=\"116.00\"");
    }
}

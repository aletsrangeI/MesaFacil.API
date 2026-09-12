using DTO.Compras;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Compras;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Compras;

public class CfdiXmlParserTests
{
    private ApplicationDbContext CrearContextoEnMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor());
    }


    private const string Cfdi40Ejemplo = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital""
    Version=""4.0"" Serie=""FA"" Folio=""1024"" Fecha=""2026-09-10T14:30:00""
    FormaPago=""03"" MetodoPago=""PUE"" SubTotal=""1000.00"" Descuento=""0.00"" Total=""1160.00"" Moneda=""MXN"">
  <cfdi:Emisor Rfc=""BME8808116B1"" Nombre=""CARNES SELECTAS DE SONORA SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""MRA120304AA1"" Nombre=""MESA FACIL RESTAURANTES SAPI DE CV"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50111500"" NoIdentificacion=""CARNE-01"" Cantidad=""10.00"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO""
                   Descripcion=""CARNE MOLIDA DE RES ESPECIAL 80/20"" ValorUnitario=""100.00"" Importe=""1000.00"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1000.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""160.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Impuestos TotalImpuestosTrasladados=""160.00"">
    <cfdi:Traslados>
      <cfdi:Traslado Base=""1000.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""160.00"" />
    </cfdi:Traslados>
  </cfdi:Impuestos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital UUID=""4B15C87E-97F1-4A56-A6F7-B15DCA7A7799"" FechaTimbrado=""2026-09-10T14:31:00"" />
  </cfdi:Complemento>
</cfdi:Comprobante>";

    private const string Cfdi33Ejemplo = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:cfdi=""http://www.sat.gob.mx/cfd/3"" xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital""
    Version=""3.3"" Serie=""B"" Folio=""552"" Fecha=""2026-08-15T10:00:00""
    SubTotal=""500.00"" Total=""540.00"">
  <cfdi:Emisor Rfc=""ABC680524P76"" Nombre=""DISTRIBUIDORA DE BEBIDAS Y ABARROTES"" />
  <cfdi:Receptor Rfc=""MRA120304AA1"" Nombre=""MESA FACIL"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50202306"" Cantidad=""5.00"" Descripcion=""REFRESCO COLA 600ML CAJA 24"" ValorUnitario=""100.00"" Importe=""500.00"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Impuesto=""003"" TasaOCuota=""0.080000"" Importe=""40.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital UUID=""99887766-5544-3322-1100-AABBCCDDEEFF"" />
  </cfdi:Complemento>
</cfdi:Comprobante>";

    [Fact]
    public async Task ParsearCfdi40_XmlValido_ExtraeDatosFiscalesYConceptosCorrectamente()
    {
        using var context = CrearContextoEnMemoria();
        var parser = new CfdiXmlParserService(context);

        var result = await parser.ParsearCfdiAsync(Cfdi40Ejemplo);

        result.Should().NotBeNull();
        result.UUID.Should().Be("4B15C87E-97F1-4A56-A6F7-B15DCA7A7799");
        result.Serie.Should().Be("FA");
        result.Folio.Should().Be("1024");
        result.RfcEmisor.Should().Be("BME8808116B1");
        result.NombreEmisor.Should().Be("CARNES SELECTAS DE SONORA SA DE CV");
        result.RegimenFiscalEmisor.Should().Be("601");
        result.Subtotal.Should().Be(1000.00m);
        result.TotalIVA.Should().Be(160.00m);
        result.Total.Should().Be(1160.00m);

        result.Conceptos.Should().HaveCount(1);
        var concepto = result.Conceptos[0];
        concepto.ClaveProdServ.Should().Be("50111500");
        concepto.NoIdentificacion.Should().Be("CARNE-01");
        concepto.Cantidad.Should().Be(10.00m);
        concepto.ClaveUnidad.Should().Be("KGM");
        concepto.Descripcion.Should().Be("CARNE MOLIDA DE RES ESPECIAL 80/20");
        concepto.ValorUnitario.Should().Be(100.00m);
        concepto.TasaIVA.Should().Be(0.160000m);
        concepto.ImporteIVA.Should().Be(160.00m);
        concepto.ImporteTotal.Should().Be(1160.00m);
    }

    [Fact]
    public async Task ParsearCfdi33_XmlValido_ExtraeDatosCorrectamente()
    {
        using var context = CrearContextoEnMemoria();
        var parser = new CfdiXmlParserService(context);

        var result = await parser.ParsearCfdiAsync(Cfdi33Ejemplo);

        result.Should().NotBeNull();
        result.UUID.Should().Be("99887766-5544-3322-1100-AABBCCDDEEFF");
        result.Folio.Should().Be("552");
        result.RfcEmisor.Should().Be("ABC680524P76");
        result.TotalIEPS.Should().Be(40.00m);
        result.Total.Should().Be(540.00m);

        result.Conceptos.Should().HaveCount(1);
        result.Conceptos[0].ClaveProdServ.Should().Be("50202306");
        result.Conceptos[0].TasaIEPS.Should().Be(0.080000m);
        result.Conceptos[0].ImporteIEPS.Should().Be(40.00m);
    }

    [Fact]
    public async Task ParsearCfdi_XmlInvalido_LanzaFormatException()
    {
        using var context = CrearContextoEnMemoria();
        var parser = new CfdiXmlParserService(context);

        Func<Task> act = async () => await parser.ParsearCfdiAsync("<esto no es un xml valido>");

        await act.Should().ThrowAsync<FormatException>();
    }

    [Fact]
    public async Task ParsearCfdi_FacturaDuplicada_DetectaUuidExistente()
    {
        using var context = CrearContextoEnMemoria();
        const string testUuid = "4B15C87E-97F1-4A56-A6F7-B15DCA7A7799";

        // Insertar factura previa con ese UUID
        context.ComprasFactura.Add(new Domain.Entities.CompraFactura
        {
            IdEmpresa = 1,
            IdSucursal = 1,
            IdAlmacen = 1,
            IdProveedor = 1,
            UUID = testUuid,
            Folio = "1024",
            Estado = "Aplicada",
            FechaEmision = DateTime.UtcNow,
            Total = 1160.00m
        });
        await context.SaveChangesAsync();

        var parser = new CfdiXmlParserService(context);
        var result = await parser.ParsearCfdiAsync(Cfdi40Ejemplo);

        result.FacturaYaExiste.Should().BeTrue();
        result.FacturaExistenteEstado.Should().Be("Aplicada");
        result.MensajeValidacion.Should().Contain(testUuid);
    }
}

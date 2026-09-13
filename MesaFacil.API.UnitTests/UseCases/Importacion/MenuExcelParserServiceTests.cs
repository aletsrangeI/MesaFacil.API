using System.Text;
using FluentAssertions;
using UseCases.Importacion;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Importacion;

/// <summary>
/// Spec 022: pruebas puras del parser (sin base de datos) para CSV con codificación Latin1
/// y separador punto y coma, verificando el respeto de acentos/ñ y la limitación documentada
/// de que un CSV solo trae la pestaña de Menú y Productos.
/// </summary>
public class MenuExcelParserServiceTests
{
    [Fact]
    public void Parse_CsvConPuntoYComaYLatin1_RespetaAcentosYEnie()
    {
        var csv = "Categoria;CodigoInterno;Producto;Descripcion;PrecioVenta;EstacionCocina;AplicaInventario;GruposModificadores\n" +
                  "Botanas;BOT-01;Niño Envuelto en Toña;Especialidad de la casa;89.50;Cocina Caliente;SI;\n";

        var bytes = Encoding.Latin1.GetBytes(csv);

        var resultado = MenuExcelParserService.Parse(bytes, "menu.csv");

        resultado.Errores.Should().BeEmpty();
        resultado.EsCsvSinModificadores.Should().BeTrue();
        resultado.Productos.Should().ContainSingle();
        resultado.Productos[0].Producto.Should().Be("Niño Envuelto en Toña");
        resultado.Productos[0].Categoria.Should().Be("Botanas");
        resultado.Productos[0].PrecioVenta.Should().Be(89.50m);
    }

    [Fact]
    public void Parse_CsvConFormatoInvalidoDePrecio_GeneraError()
    {
        var csv = "Categoria,Producto,PrecioVenta\nBebidas,Agua Mineral,no-es-un-precio\n";
        var bytes = Encoding.UTF8.GetBytes(csv);

        var resultado = MenuExcelParserService.Parse(bytes, "menu.csv");

        resultado.Errores.Should().Contain(e => e.Columna == "PrecioVenta" && e.Mensaje.Contains("no es un precio"));
    }

    [Fact]
    public void Parse_ExtensionNoSoportada_GeneraErrorGeneral()
    {
        var resultado = MenuExcelParserService.Parse(new byte[] { 1, 2, 3 }, "menu.pdf");

        resultado.Errores.Should().ContainSingle(e => e.Mensaje.Contains("no soportado"));
    }
}

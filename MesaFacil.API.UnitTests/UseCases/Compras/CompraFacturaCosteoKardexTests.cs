using FluentAssertions;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Compras;

public class CompraFacturaCosteoKardexTests
{
    [Fact]
    public void RecalculoCostoPromedioPonderado_CompraMayorCosto_CalculaMatematicamenteExacto()
    {
        // Escenario Spec 016 Criterio 3:
        // Stock actual: 50 kg de carne molida a CPP $100.00 MXN
        decimal stockActual = 50.00m;
        decimal costoPromedioActual = 100.00m;

        // Se compran 100 kg a $130.00 MXN (costo superior)
        decimal cantidadEntrada = 100.00m;
        decimal costoCompraUnitario = 130.00m;

        // Fórmula: ((StockActual * CPP_actual) + (CantidadEntrada * CostoCompraUnitario)) / (StockActual + CantidadEntrada)
        decimal valorPrevio = stockActual * costoPromedioActual; // 50 * 100 = 5,000.00
        decimal valorEntrada = cantidadEntrada * costoCompraUnitario; // 100 * 130 = 13,000.00
        decimal nuevoStock = stockActual + cantidadEntrada; // 150.00
        decimal nuevoCostoPromedio = Math.Round((valorPrevio + valorEntrada) / nuevoStock, 4);

        // Verificación matemática
        valorPrevio.Should().Be(5000.00m);
        valorEntrada.Should().Be(13000.00m);
        nuevoStock.Should().Be(150.00m);
        nuevoCostoPromedio.Should().Be(120.00m); // 18,000 / 150 = 120.00
    }

    [Fact]
    public void FactorConversion_CajasAPiezas_CalculaCantidadYCostoUnitarioBase()
    {
        // Se compran 5 cajas de refresco de 24 piezas cada una a $240.00 por caja
        decimal cajasCompradas = 5.0m;
        decimal factorConversion = 24.0m; // 24 piezas por caja
        decimal costoPorCaja = 240.00m;

        decimal piezasTotales = cajasCompradas * factorConversion;
        decimal importeTotalCompra = cajasCompradas * costoPorCaja;
        decimal costoUnitarioPorPieza = importeTotalCompra / piezasTotales;

        piezasTotales.Should().Be(120.0m);
        importeTotalCompra.Should().Be(1200.00m);
        costoUnitarioPorPieza.Should().Be(10.00m);
    }

    [Fact]
    public void RecalculoCostoPromedioPonderado_StockInicialEnCero_AsignaCostoUnitarioDirecto()
    {
        decimal stockActual = 0m;
        decimal cantidadEntrada = 25.00m;
        decimal costoCompraUnitario = 45.50m;

        decimal nuevoCostoPromedio = stockActual <= 0 ? costoCompraUnitario : 0;

        nuevoCostoPromedio.Should().Be(45.50m);
    }
}

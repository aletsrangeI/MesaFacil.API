using FluentAssertions;
using UseCases.Inventario;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Inventario;

public class RecetaCosteoTests
{
    [Fact]
    public void ConversionUnidades_GramosAKilogramos_CalculaExacto()
    {
        // 180 gramos convertidos a Kilogramos -> 0.18 kg
        decimal cantidadGramos = 180m;
        decimal resultado = ConversionUnidadesHelper.ConvertirCantidad(cantidadGramos, "G", "KG");

        resultado.Should().Be(0.18m);
    }

    [Fact]
    public void ConversionUnidades_KilogramosAGramos_CalculaExacto()
    {
        decimal cantidadKg = 1.5m;
        decimal resultado = ConversionUnidadesHelper.ConvertirCantidad(cantidadKg, "KG", "G");

        resultado.Should().Be(1500m);
    }

    [Fact]
    public void ConversionUnidades_MililitrosALitros_CalculaExacto()
    {
        decimal cantidadMl = 250m;
        decimal resultado = ConversionUnidadesHelper.ConvertirCantidad(cantidadMl, "ML", "L");

        resultado.Should().Be(0.25m);
    }

    [Fact]
    public void CosteoMatematico_HamburguesaClasica_CumpleCriterioAceptacionSpec015()
    {
        // Insumo 1: Carne molida - 180g comprada a $120.00 el kg
        decimal costoKgCarne = 120.00m;
        decimal carneGramos = 180m;
        decimal carneKg = ConversionUnidadesHelper.ConvertirCantidad(carneGramos, "G", "KG");
        decimal costoCarne = carneKg * costoKgCarne; // 0.18 * 120 = 21.60

        // Insumo 2: Pan brioche - 1 pieza a $4.50
        decimal costoPan = 4.50m;

        // Insumo 3: Queso gouda - 30g a $150.00 el kg
        decimal costoKgQueso = 150.00m;
        decimal quesoGramos = 30m;
        decimal quesoKg = ConversionUnidadesHelper.ConvertirCantidad(quesoGramos, "G", "KG");
        decimal costoQueso = quesoKg * costoKgQueso; // 0.03 * 150 = 4.50

        decimal costoTotalPlatillo = costoCarne + costoPan + costoQueso;

        costoCarne.Should().Be(21.60m);
        costoQueso.Should().Be(4.50m);
        costoTotalPlatillo.Should().Be(30.60m);

        // PVP = $120.00 MXN
        decimal pvp = 120.00m;
        decimal margenBruto = pvp - costoTotalPlatillo;
        decimal margenPct = Math.Round((margenBruto / pvp) * 100m, 1);
        decimal foodCostPct = Math.Round((costoTotalPlatillo / pvp) * 100m, 1);

        margenBruto.Should().Be(89.40m);
        margenPct.Should().Be(74.5m);
        foodCostPct.Should().Be(25.5m);
    }

    [Fact]
    public void MermaEsperada_AumentaConsumoEfectivo()
    {
        // 200g con 5% de merma en cocción -> 210g
        decimal cantidad = 200m;
        decimal mermaPct = 5.0m;
        decimal consumoEfectivo = cantidad * (1m + (mermaPct / 100m));

        consumoEfectivo.Should().Be(210.0m);
    }

    [Fact]
    public void SubReceta_CalculoCostoPorcionProporcional()
    {
        // Sub-receta "Salsa Secreta":
        // Rendimiento: 4,000 ML
        // Costo total del lote: $200.00 MXN
        decimal rendimientoLote = 4000m;
        decimal costoTotalLote = 200.00m;
        decimal costoPorMl = costoTotalLote / rendimientoLote; // $0.05 por ML

        // Platillo consume 50 ML
        decimal consumoPlatillo = 50m;
        decimal costoEnPlatillo = consumoPlatillo * costoPorMl;

        costoPorMl.Should().Be(0.05m);
        costoEnPlatillo.Should().Be(2.50m);
    }
}

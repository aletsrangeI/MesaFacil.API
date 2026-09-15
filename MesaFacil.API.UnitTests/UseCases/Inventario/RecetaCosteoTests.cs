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

    [Fact]
    public void ConversionUnidades_MililitrosAKilogramos_CalculaExacto()
    {
        // 10 ML de líquido culinario con base KG (ej. Aceite de Trufa) -> 0.01 KG
        decimal cantidadMl = 10m;
        decimal resultado = ConversionUnidadesHelper.ConvertirCantidad(cantidadMl, "ML", "KG");

        resultado.Should().Be(0.01m);
    }

    [Fact]
    public void ConversionUnidades_LitrosAKilogramos_CalculaExacto()
    {
        // 1.5 L -> 1.5 KG
        decimal cantidadL = 1.5m;
        decimal resultado = ConversionUnidadesHelper.ConvertirCantidad(cantidadL, "L", "KG");

        resultado.Should().Be(1.5m);
    }

    [Fact]
    public void ConversionUnidades_GramosAMililitros_CalculaExacto()
    {
        // 250 G -> 250 ML
        decimal cantidadG = 250m;
        decimal resultado = ConversionUnidadesHelper.ConvertirCantidad(cantidadG, "G", "ML");

        resultado.Should().Be(250m);
    }

    [Fact]
    public void CosteoMatematico_AceiteTrufa_10mlConBaseKg_CalculoCostoCorrecto()
    {
        // Aceite de Trufa: $380.00 / KG
        // Receta pide: 10 ML con 4% de merma
        decimal costoKg = 380.00m;
        decimal cantidadMl = 10m;
        decimal mermaPct = 4m;

        decimal cantBaseKg = ConversionUnidadesHelper.ConvertirCantidad(cantidadMl, "ML", "KG");
        decimal cantConMerma = cantBaseKg * (1m + (mermaPct / 100m));
        decimal costoTotal = Math.Round(cantConMerma * costoKg, 2);

        cantBaseKg.Should().Be(0.01m);
        cantConMerma.Should().Be(0.0104m);
        costoTotal.Should().Be(3.95m); // NO $3,952.00
    }
}

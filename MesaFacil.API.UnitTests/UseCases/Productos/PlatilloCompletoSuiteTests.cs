using DTO.Producto;
using DTO.Receta;
using FluentAssertions;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Productos;

public class PlatilloCompletoSuiteTests
{
    [Fact]
    public void CrearPlatilloCompletoDTO_PlatilloSimple_EstructuraCorrecta()
    {
        var dto = new CrearPlatilloCompletoDTO
        {
            Nombre = "Parrillada Sonora Especial",
            Descripcion = "Rib Eye y Arrachera marinada",
            IdCategoria = 2,
            IdMenu = 1,
            TieneVariantes = false,
            PrecioVenta = 120.00m
        };

        dto.Nombre.Should().Be("Parrillada Sonora Especial");
        dto.TieneVariantes.Should().BeFalse();
        dto.PrecioVenta.Should().Be(120.00m);
    }

    [Fact]
    public void CrearPlatilloCompletoDTO_ConVariantesMultiples_ConservaPreciosYDefault()
    {
        var dto = new CrearPlatilloCompletoDTO
        {
            Nombre = "Pizza Pepperoni",
            IdCategoria = 3,
            IdMenu = 1,
            TieneVariantes = true,
            Variantes = new List<PlatilloVarianteItemDTO>
            {
                new() { Nombre = "Individual", PrecioVenta = 110.00m, EsDefault = false },
                new() { Nombre = "Familiar", PrecioVenta = 230.00m, EsDefault = true }
            }
        };

        dto.TieneVariantes.Should().BeTrue();
        dto.Variantes.Should().HaveCount(2);
        dto.Variantes.First(v => v.EsDefault).Nombre.Should().Be("Familiar");
        dto.Variantes.First(v => v.EsDefault).PrecioVenta.Should().Be(230.00m);
    }

    [Fact]
    public void CrearRecetaDTO_PermiteSincronizarPrecioVentaActualParaPOS()
    {
        var dto = new CrearRecetaDTO
        {
            Nombre = "Receta Parrillada Sonora",
            IdProducto = 5,
            PrecioVentaActual = 135.00m,
            Rendimiento = 1,
            IdUnidadMedidaRendimiento = 1
        };

        dto.PrecioVentaActual.Should().Be(135.00m);
    }
}

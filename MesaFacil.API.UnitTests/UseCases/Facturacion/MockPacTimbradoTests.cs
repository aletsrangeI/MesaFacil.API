using Domain.Entities;
using DTO.Facturacion;
using FluentAssertions;
using Interface.PAC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Facturacion;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Facturacion;

public class MockPacTimbradoTests
{
    private static ApplicationDbContext CrearContextoEnMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    private static IServiceProvider CrearServiceProviderConMockPac()
    {
        var services = new ServiceCollection();
        services.AddScoped<IPACTimbradoService, MockPacService>();
        return services.BuildServiceProvider();
    }

    private static async Task<(ApplicationDbContext context, Pedido pedido, EmpresaBolsaTimbres bolsa)> SembrarPedidoPagadoAsync(ApplicationDbContext context)
    {
        var empresa = new Empresa { Nombre = "Bistró Demo", Rfc = "BME8808116B1" };
        context.Empresas.Add(empresa);
        await context.SaveChangesAsync();

        var sucursal = new Sucursal { IdEmpresa = empresa.Id, Nombre = "Centro" };
        context.Sucursales.Add(sucursal);
        await context.SaveChangesAsync();

        var pedido = new Pedido
        {
            IdEmpresa = empresa.Id,
            IdSucursal = sucursal.Id,
            IdTipoPedido = 1,
            IdEstadoPedido = 1
        };
        context.Pedidos.Add(pedido);
        await context.SaveChangesAsync();

        context.PedidoDetalles.Add(new PedidoDetalle
        {
            IdPedido = pedido.Id,
            IdProducto = 1,
            IdVariante = 1,
            ProductoNombre = "Hamburguesa Brasa",
            VarianteNombre = "Regular",
            Cantidad = 2,
            PrecioUnitario = 100m,
            IdImpuesto = 1,
            TasaImpuesto = 16m,
            IdEstadoPedidoDetalle = 1
        });
        await context.SaveChangesAsync();

        var cuenta = new Cuenta { IdPedido = pedido.Id, Subtotal = 200m, ImpuestoTotal = 32m, Total = 232m, IdEstadoCuenta = 1 };
        context.Cuentas.Add(cuenta);
        await context.SaveChangesAsync();

        context.Pagos.Add(new Pago { IdCuenta = cuenta.Id, Monto = 232m, PagadoEn = DateTime.UtcNow, IdMetodoDePago = 1 });
        await context.SaveChangesAsync();

        var configPac = new EmpresaConfiguracionPAC
        {
            IdEmpresa = empresa.Id,
            ProveedorPAC = ProveedorPacNombres.Mock,
            SerieFacturacion = "F",
            FolioSiguiente = 1,
            LugarExpedicionCP = "83000",
            RfcEmisor = "BME8808116B1",
            RazonSocialEmisor = "Bistró Demo SA de CV",
            RegimenFiscalEmisor = "601"
        };
        context.EmpresaConfiguracionesPAC.Add(configPac);

        var bolsa = new EmpresaBolsaTimbres { IdEmpresa = empresa.Id, TimbresDisponibles = 10, TimbresConsumidos = 0 };
        context.EmpresaBolsasTimbres.Add(bolsa);
        await context.SaveChangesAsync();

        return (context, pedido, bolsa);
    }

    [Fact]
    public async Task TimbrarPedidoAsync_CicloCompleto_DecrementaBolsaDeTimbresEnUno()
    {
        // Arrange
        var context = CrearContextoEnMemoria();
        var (_, pedido, bolsa) = await SembrarPedidoPagadoAsync(context);

        var sut = new FacturaVentaService(
            context,
            new GeneradorXmlCfdi40Service(),
            new SelloDigitalService(),
            CrearServiceProviderConMockPac());

        var request = new TimbrarPedidoRequestDTO
        {
            PedidoId = pedido.Id,
            RfcReceptor = "XAXX010101000",
            NombreReceptor = "PUBLICO EN GENERAL",
            RegimenFiscalReceptor = "616",
            CodigoPostalReceptor = "83000",
            UsoCfdi = "S01",
            FormaPago = "01",
            MetodoPago = "PUE"
        };

        // Act
        var result = await sut.TimbrarPedidoAsync(request);

        // Assert
        result.isSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.UUID.Should().NotBeNullOrWhiteSpace();
        Guid.TryParse(result.Data.UUID, out _).Should().BeTrue();
        result.Data.EstadoFiscal.Should().Be(EstadoFiscalFactura.Vigente);

        var bolsaActualizada = await context.EmpresaBolsasTimbres.FindAsync(bolsa.Id);
        bolsaActualizada!.TimbresDisponibles.Should().Be(9);
        bolsaActualizada.TimbresConsumidos.Should().Be(1);

        var historial = await context.ConsumosTimbreHistorial.ToListAsync();
        historial.Should().ContainSingle(h => h.Tipo == TipoMovimientoTimbre.Consumo && h.Cantidad == 1);
    }

    [Fact]
    public async Task TimbrarPedidoAsync_RfcReceptorInvalido_RetornaErrorSinConsumirTimbre()
    {
        var context = CrearContextoEnMemoria();
        var (_, pedido, bolsa) = await SembrarPedidoPagadoAsync(context);

        var sut = new FacturaVentaService(
            context,
            new GeneradorXmlCfdi40Service(),
            new SelloDigitalService(),
            CrearServiceProviderConMockPac());

        var request = new TimbrarPedidoRequestDTO
        {
            PedidoId = pedido.Id,
            RfcReceptor = "NO-VALIDO",
            NombreReceptor = "Cliente",
            RegimenFiscalReceptor = "616",
            CodigoPostalReceptor = "83000",
            UsoCfdi = "S01",
            FormaPago = "01"
        };

        var result = await sut.TimbrarPedidoAsync(request);

        result.isSuccess.Should().BeFalse();

        var bolsaSinCambios = await context.EmpresaBolsasTimbres.FindAsync(bolsa.Id);
        bolsaSinCambios!.TimbresDisponibles.Should().Be(10);
    }

    [Fact]
    public async Task TimbrarPedidoAsync_SinSaldoDeTimbres_RetornaErrorControlado()
    {
        var context = CrearContextoEnMemoria();
        var (_, pedido, bolsa) = await SembrarPedidoPagadoAsync(context);
        bolsa.TimbresDisponibles = 0;
        await context.SaveChangesAsync();

        var sut = new FacturaVentaService(
            context,
            new GeneradorXmlCfdi40Service(),
            new SelloDigitalService(),
            CrearServiceProviderConMockPac());

        var request = new TimbrarPedidoRequestDTO
        {
            PedidoId = pedido.Id,
            RfcReceptor = "XAXX010101000",
            NombreReceptor = "PUBLICO EN GENERAL",
            RegimenFiscalReceptor = "616",
            CodigoPostalReceptor = "83000",
            UsoCfdi = "S01",
            FormaPago = "01"
        };

        var result = await sut.TimbrarPedidoAsync(request);

        result.isSuccess.Should().BeFalse();
        result.Message.Should().Contain("timbres");
    }
}

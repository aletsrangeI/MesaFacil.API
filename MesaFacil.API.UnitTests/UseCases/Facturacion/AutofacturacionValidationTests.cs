using Domain.Entities;
using DTO.Facturacion;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Facturacion;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Facturacion;

public class AutofacturacionValidationTests
{
    private static ApplicationDbContext CrearContextoEnMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    private static async Task<Pedido> SembrarPedidoPagadoAsync(ApplicationDbContext context, DateTime fechaPago)
    {
        var empresa = new Empresa { Nombre = "Bistró Demo", Rfc = "BME8808116B1" };
        context.Empresas.Add(empresa);
        await context.SaveChangesAsync();

        var sucursal = new Sucursal { IdEmpresa = empresa.Id, Nombre = "Centro" };
        context.Sucursales.Add(sucursal);
        await context.SaveChangesAsync();

        var pedido = new Pedido { IdEmpresa = empresa.Id, IdSucursal = sucursal.Id, IdTipoPedido = 1, IdEstadoPedido = 1 };
        context.Pedidos.Add(pedido);
        await context.SaveChangesAsync();

        var cuenta = new Cuenta { IdPedido = pedido.Id, Subtotal = 200m, ImpuestoTotal = 32m, Total = 232m, IdEstadoCuenta = 1 };
        context.Cuentas.Add(cuenta);
        await context.SaveChangesAsync();

        context.Pagos.Add(new Pago { IdCuenta = cuenta.Id, Monto = 232m, PagadoEn = fechaPago, IdMetodoDePago = 1 });
        await context.SaveChangesAsync();

        return pedido;
    }

    [Fact]
    public async Task ValidarTicketAsync_TicketYaFacturado_MarcaYaFacturadoYNoPermiteGenerar()
    {
        var context = CrearContextoEnMemoria();
        var pedido = await SembrarPedidoPagadoAsync(context, DateTime.UtcNow);

        context.FacturasVenta.Add(new FacturaVenta
        {
            IdEmpresa = pedido.IdEmpresa,
            IdSucursal = pedido.IdSucursal,
            PedidoId = pedido.Id,
            UUID = Guid.NewGuid().ToString(),
            Serie = "F",
            Folio = "1",
            RfcReceptor = "XAXX010101000",
            NombreReceptor = "PUBLICO EN GENERAL",
            EstadoFiscal = EstadoFiscalFactura.Vigente,
            TicketAutofacturaGuid = pedido.Id
        });
        await context.SaveChangesAsync();

        var facturaVentaServiceMock = new Mock<IFacturaVentaService>();
        var sut = new AutofacturacionComensalService(context, facturaVentaServiceMock.Object);

        // Act - validación
        var validacion = await sut.ValidarTicketAsync(pedido.Id);

        // Assert
        validacion.Data!.YaFacturado.Should().BeTrue();

        // Act - intento de generar factura duplicada
        var resultado = await sut.GenerarFacturaAsync(new GenerarFacturaAutofacturaRequestDTO
        {
            TicketGuid = pedido.Id,
            RfcReceptor = "XAXX010101000",
            NombreReceptor = "PUBLICO EN GENERAL",
            RegimenFiscalReceptor = "616",
            CodigoPostalReceptor = "83000",
            UsoCfdi = "S01"
        });

        resultado.isSuccess.Should().BeFalse();
        resultado.Message.Should().Contain("ya fue facturado");
        facturaVentaServiceMock.Verify(s => s.TimbrarPedidoAsync(It.IsAny<TimbrarPedidoRequestDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidarTicketAsync_TicketVencido_NoPermiteGenerarFactura()
    {
        var context = CrearContextoEnMemoria();
        // Pago hace 30 días: vence a los 7 días naturales por defecto.
        var pedido = await SembrarPedidoPagadoAsync(context, DateTime.UtcNow.AddDays(-30));

        var facturaVentaServiceMock = new Mock<IFacturaVentaService>();
        var sut = new AutofacturacionComensalService(context, facturaVentaServiceMock.Object, diasVigenciaAutofactura: 7);

        var validacion = await sut.ValidarTicketAsync(pedido.Id);
        validacion.Data!.Vigente.Should().BeFalse();

        var resultado = await sut.GenerarFacturaAsync(new GenerarFacturaAutofacturaRequestDTO
        {
            TicketGuid = pedido.Id,
            RfcReceptor = "XAXX010101000",
            NombreReceptor = "PUBLICO EN GENERAL",
            RegimenFiscalReceptor = "616",
            CodigoPostalReceptor = "83000",
            UsoCfdi = "S01"
        });

        resultado.isSuccess.Should().BeFalse();
        resultado.Message.Should().Contain("vigencia");
        facturaVentaServiceMock.Verify(s => s.TimbrarPedidoAsync(It.IsAny<TimbrarPedidoRequestDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ValidarTicketAsync_TicketNoExiste_RetornaExistePedidoFalso()
    {
        var context = CrearContextoEnMemoria();
        var sut = new AutofacturacionComensalService(context, new Mock<IFacturaVentaService>().Object);

        var validacion = await sut.ValidarTicketAsync(Guid.NewGuid());

        validacion.Data!.ExistePedido.Should().BeFalse();
    }

    [Fact]
    public async Task GenerarFacturaAsync_TicketVigenteYNoFacturado_DelegaEnFacturaVentaService()
    {
        var context = CrearContextoEnMemoria();
        var pedido = await SembrarPedidoPagadoAsync(context, DateTime.UtcNow);

        var facturaVentaServiceMock = new Mock<IFacturaVentaService>();
        facturaVentaServiceMock
            .Setup(s => s.TimbrarPedidoAsync(It.IsAny<TimbrarPedidoRequestDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Common.Response<FacturaVentaDTO>
            {
                isSuccess = false,
                Message = "PAC no configurado en este test"
            });

        var sut = new AutofacturacionComensalService(context, facturaVentaServiceMock.Object);

        var resultado = await sut.GenerarFacturaAsync(new GenerarFacturaAutofacturaRequestDTO
        {
            TicketGuid = pedido.Id,
            RfcReceptor = "XAXX010101000",
            NombreReceptor = "PUBLICO EN GENERAL",
            RegimenFiscalReceptor = "616",
            CodigoPostalReceptor = "83000",
            UsoCfdi = "S01"
        });

        facturaVentaServiceMock.Verify(s => s.TimbrarPedidoAsync(
            It.Is<TimbrarPedidoRequestDTO>(r => r.PedidoId == pedido.Id && r.RfcReceptor == "XAXX010101000"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

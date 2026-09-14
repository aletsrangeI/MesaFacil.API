using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Pago;
using FluentAssertions;
using Interface.Persistence;
using Interface.UseCases;
using Moq;
using UseCases.Pagos;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Pagos;

public class PagoApplicationTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAppLogger<PagoApplication>> _loggerMock;
    private readonly Mock<IDescuentoInventarioService> _descuentoInventarioServiceMock;
    private readonly PagoApplication _sut;

    public PagoApplicationTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<IAppLogger<PagoApplication>>();
        _descuentoInventarioServiceMock = new Mock<IDescuentoInventarioService>();

        _sut = new PagoApplication(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            new PagoDTOValidator(),
            _loggerMock.Object,
            _descuentoInventarioServiceMock.Object
        );
    }

    [Fact]
    public async Task RegistrarPagoAsync_PagoParcial_RegistraPagoYNoCierraCuenta()
    {
        // Arrange
        int cuentaId = 10;
        Guid pedidoId = Guid.NewGuid();

        var cuenta = new Cuenta
        {
            Id = cuentaId,
            IdPedido = pedidoId,
            Total = 200m,
            IdEstadoCuenta = 2 // Abierta
        };

        var pagoDto = new RegistrarPagoDTO
        {
            IdCuenta = cuentaId,
            Monto = 100m,
            Propina = 10m,
            IdMetodoDePago = 1,
            Referencia = "EFECTIVO-100"
        };

        _unitOfWorkMock.Setup(u => u.Cuentas.GetAsync(cuentaId)).ReturnsAsync(cuenta);
        _unitOfWorkMock.Setup(u => u.Pagos.InsertAsync(It.IsAny<Pago>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Pagos.GetAllAsync()).ReturnsAsync(new List<Pago>()); // Sin pagos previos
        _unitOfWorkMock.Setup(u => u.EventosPedido.InsertAsync(It.IsAny<EventoPedido>())).ReturnsAsync(true);

        // Act
        var result = await _sut.RegistrarPagoAsync(pagoDto);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Pago registrado exitosamente");

        // Verificamos que se insertó el pago
        _unitOfWorkMock.Verify(u => u.Pagos.InsertAsync(It.Is<Pago>(p =>
            p.IdCuenta == cuentaId &&
            p.Monto == 100m &&
            p.Propina == 10m)), Times.Once);

        // Evento de pago registrado
        _unitOfWorkMock.Verify(u => u.EventosPedido.InsertAsync(It.Is<EventoPedido>(e =>
            e.IdPedido == pedidoId &&
            e.TipoEvento == "PagoRegistrado")), Times.Once);

        // Al ser pago parcial (100 < 200), NO debe cerrar la cuenta ni el pedido
        _unitOfWorkMock.Verify(u => u.Cuentas.UpdateAsync(It.IsAny<Cuenta>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Pedidos.UpdateAsync(It.IsAny<Pedido>()), Times.Never);
        _descuentoInventarioServiceMock.Verify(d => d.DescontarPorCuentaPagadaAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarPagoAsync_PagoCompleto_LiquidaCuentaYCierraPedidoYMarcaMesaSucia()
    {
        // Arrange
        int cuentaId = 15;
        Guid pedidoId = Guid.NewGuid();
        int mesaId = 4;

        var cuenta = new Cuenta
        {
            Id = cuentaId,
            IdPedido = pedidoId,
            Total = 250m,
            IdEstadoCuenta = 2 // Abierta
        };

        var pedido = new Pedido
        {
            Id = pedidoId,
            IdMesa = mesaId,
            IdEstadoPedido = 2 // En proceso
        };

        var mesa = new Mesa
        {
            Id = mesaId,
            IdEstadoMesa = 2 // Ocupada
        };

        var pagosPrevios = new List<Pago>
        {
            new Pago { IdCuenta = cuentaId, Monto = 100m }
        };

        var pagoDto = new RegistrarPagoDTO
        {
            IdCuenta = cuentaId,
            Monto = 150m, // 100 + 150 = 250 (Liquidación exacta)
            Propina = 25m,
            IdMetodoDePago = 2,
            Referencia = "TARJETA-AUTH-987"
        };

        _unitOfWorkMock.Setup(u => u.Cuentas.GetAsync(cuentaId)).ReturnsAsync(cuenta);
        _unitOfWorkMock.Setup(u => u.Pagos.InsertAsync(It.IsAny<Pago>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Pagos.GetAllAsync()).ReturnsAsync(pagosPrevios);
        _unitOfWorkMock.Setup(u => u.EventosPedido.InsertAsync(It.IsAny<EventoPedido>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Cuentas.UpdateAsync(It.IsAny<Cuenta>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Pedidos.GetAsync(pedidoId)).ReturnsAsync(pedido);
        _unitOfWorkMock.Setup(u => u.Pedidos.UpdateAsync(It.IsAny<Pedido>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.Mesas.GetAsync(mesaId)).ReturnsAsync(mesa);
        _unitOfWorkMock.Setup(u => u.Mesas.UpdateAsync(It.IsAny<Mesa>())).ReturnsAsync(true);

        // Act
        var result = await _sut.RegistrarPagoAsync(pagoDto);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();

        // 1. Cuenta liquidada -> Estado 1 (Pagada)
        _unitOfWorkMock.Verify(u => u.Cuentas.UpdateAsync(It.Is<Cuenta>(c => c.Id == cuentaId && c.IdEstadoCuenta == 1)), Times.Once);

        // 2. Pedido cerrado -> Estado 5 (Cerrado)
        _unitOfWorkMock.Verify(u => u.Pedidos.UpdateAsync(It.Is<Pedido>(p => p.Id == pedidoId && p.IdEstadoPedido == 5)), Times.Once);

        // 3. Mesa marcada sucia -> Estado 4 (Sucia)
        _unitOfWorkMock.Verify(u => u.Mesas.UpdateAsync(It.Is<Mesa>(m => m.Id == mesaId && m.IdEstadoMesa == 4)), Times.Once);

        // 4. Evento de cierre registrado
        _unitOfWorkMock.Verify(u => u.EventosPedido.InsertAsync(It.Is<EventoPedido>(e =>
            e.IdPedido == pedidoId &&
            e.TipoEvento == "PedidoCerrado")), Times.Once);

        // 5. Descuento de inventario invocado
        _descuentoInventarioServiceMock.Verify(d => d.DescontarPorCuentaPagadaAsync(cuentaId), Times.Once);
    }

    [Fact]
    public async Task RegistrarPagoAsync_CuentaNoExiste_RetornaError()
    {
        // Arrange
        int cuentaIdInexistente = 999;
        var pagoDto = new RegistrarPagoDTO
        {
            IdCuenta = cuentaIdInexistente,
            Monto = 50m
        };

        _unitOfWorkMock.Setup(u => u.Cuentas.GetAsync(cuentaIdInexistente)).ReturnsAsync((Cuenta?)null);

        // Act
        var result = await _sut.RegistrarPagoAsync(pagoDto);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeFalse();
        result.Message.Should().Contain("Cuenta no encontrada");
        _loggerMock.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Cuenta no encontrada"))), Times.Once);
        _unitOfWorkMock.Verify(u => u.Pagos.InsertAsync(It.IsAny<Pago>()), Times.Never);
    }
}

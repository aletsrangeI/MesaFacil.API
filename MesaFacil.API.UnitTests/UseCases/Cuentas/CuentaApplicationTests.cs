using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Cuenta;
using FluentAssertions;
using Interface.Persistence;
using Moq;
using UseCases.Cuentas;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Cuentas;

public class CuentaApplicationTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAppLogger<CuentaApplication>> _loggerMock;
    private readonly CuentaApplication _sut;

    public CuentaApplicationTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<IAppLogger<CuentaApplication>>();

        _sut = new CuentaApplication(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            new CuentaDTOValidator(),
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GenerarCuentaAsync_ConPedidoYDetallesActivos_CalculaSubtotalImpuestoYTotal()
    {
        // Arrange
        Guid pedidoId = Guid.NewGuid();
        var pedido = new Pedido
        {
            Id = pedidoId,
            IdSucursal = 1,
            IdMesa = 3
        };

        var mesa = new Mesa { Id = 3, Codigo = "Mesa 3" };
        var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Central" };

        var detalles = new List<PedidoDetalle>
        {
            new PedidoDetalle
            {
                IdPedido = pedidoId,
                ProductoNombre = "Hamburguesa Doble",
                Cantidad = 2m,
                PrecioUnitario = 100m,
                MontoImpuesto = 32m,
                Cancelado = false
            },
            new PedidoDetalle
            {
                IdPedido = pedidoId,
                ProductoNombre = "Refresco Cola",
                Cantidad = 1m,
                PrecioUnitario = 50m,
                MontoImpuesto = 8m,
                Cancelado = false
            },
            new PedidoDetalle
            {
                IdPedido = pedidoId,
                ProductoNombre = "Cerveza Cancelada",
                Cantidad = 1m,
                PrecioUnitario = 70m,
                MontoImpuesto = 11.2m,
                Cancelado = true // Cancelado no debe computar
            }
        };

        _unitOfWorkMock.Setup(u => u.Pedidos.GetAsync(pedidoId)).ReturnsAsync(pedido);
        _unitOfWorkMock.Setup(u => u.PedidoDetalles.GetAllAsync()).ReturnsAsync(detalles);
        _unitOfWorkMock.Setup(u => u.Cuentas.GetAllAsync()).ReturnsAsync(new List<Cuenta>());
        _unitOfWorkMock.Setup(u => u.Pagos.GetAllAsync()).ReturnsAsync(new List<Pago>());
        _unitOfWorkMock.Setup(u => u.Mesas.GetAsync(3)).ReturnsAsync(mesa);
        _unitOfWorkMock.Setup(u => u.Sucursales.GetAsync(1)).ReturnsAsync(sucursal);
        _unitOfWorkMock.Setup(u => u.PedidoModificadores.GetAllAsync()).ReturnsAsync(new List<PedidoModificador>());

        _mapperMock.Setup(m => m.Map<CuentaDTO>(It.IsAny<Cuenta>()))
            .Returns((Cuenta c) => new CuentaDTO
            {
                Id = c.Id,
                IdPedido = c.IdPedido,
                Subtotal = c.Subtotal,
                ImpuestoTotal = c.ImpuestoTotal,
                Total = c.Total,
                IdEstadoCuenta = c.IdEstadoCuenta
            });

        // Act
        var result = await _sut.GenerarCuentaAsync(pedidoId);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        // 200 + 50 = 250 total
        // 32 + 8 = 40 impuestos
        // 250 - 40 = 210 subtotal
        result.Data.Total.Should().Be(250m);
        result.Data.ImpuestoTotal.Should().Be(40m);
        result.Data.Subtotal.Should().Be(210m);
        result.Data.SaldoRestante.Should().Be(250m);
        result.Data.TotalPagado.Should().Be(0m);
        result.Data.MesaNombre.Should().Be("Mesa 3");
        result.Data.SucursalNombre.Should().Be("Sucursal Central");
        result.Data.Items.Should().HaveCount(2);

        _unitOfWorkMock.Verify(u => u.Cuentas.InsertAsync(It.Is<Cuenta>(c => c.IdPedido == pedidoId && c.Total == 250m)), Times.Once);
    }

    [Fact]
    public async Task GenerarCuentaAsync_ConPagosPrevios_CalculaTotalPagadoYSaldoRestante()
    {
        // Arrange
        Guid pedidoId = Guid.NewGuid();
        var pedido = new Pedido { Id = pedidoId, IdSucursal = 1 };

        var detalles = new List<PedidoDetalle>
        {
            new PedidoDetalle
            {
                IdPedido = pedidoId,
                ProductoNombre = "Pizza Familiar",
                Cantidad = 1m,
                PrecioUnitario = 300m,
                MontoImpuesto = 41.38m,
                Cancelado = false
            }
        };

        var cuentaExistente = new Cuenta
        {
            Id = 55,
            IdPedido = pedidoId,
            IdEstadoCuenta = 2, // Abierta
            IsActive = true,
            Total = 300m
        };

        var pagos = new List<Pago>
        {
            new Pago
            {
                IdCuenta = 55,
                Monto = 100m,
                Propina = 20m,
                IdMetodoDePago = 1,
                IsActive = true,
                PagadoEn = DateTime.UtcNow.AddMinutes(-10)
            },
            new Pago
            {
                IdCuenta = 55,
                Monto = 50m,
                Propina = 10m,
                IdMetodoDePago = 2,
                IsActive = true,
                PagadoEn = DateTime.UtcNow.AddMinutes(-5)
            }
        };

        _unitOfWorkMock.Setup(u => u.Pedidos.GetAsync(pedidoId)).ReturnsAsync(pedido);
        _unitOfWorkMock.Setup(u => u.PedidoDetalles.GetAllAsync()).ReturnsAsync(detalles);
        _unitOfWorkMock.Setup(u => u.Cuentas.GetAllAsync()).ReturnsAsync(new List<Cuenta> { cuentaExistente });
        _unitOfWorkMock.Setup(u => u.Pagos.GetAllAsync()).ReturnsAsync(pagos);
        _unitOfWorkMock.Setup(u => u.Sucursales.GetAsync(1)).ReturnsAsync(new Sucursal { Id = 1, Nombre = "Sucursal Norte" });
        _unitOfWorkMock.Setup(u => u.PedidoModificadores.GetAllAsync()).ReturnsAsync(new List<PedidoModificador>());

        _mapperMock.Setup(m => m.Map<CuentaDTO>(It.IsAny<Cuenta>()))
            .Returns((Cuenta c) => new CuentaDTO
            {
                Id = c.Id,
                IdPedido = c.IdPedido,
                Total = c.Total
            });

        // Act
        var result = await _sut.GenerarCuentaAsync(pedidoId);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Data.TotalPagado.Should().Be(150m); // 100 + 50
        result.Data.SaldoRestante.Should().Be(150m); // 300 - 150
        result.Data.PagosRealizados.Should().HaveCount(2);

        _unitOfWorkMock.Verify(u => u.Cuentas.UpdateAsync(It.Is<Cuenta>(c => c.Id == 55 && c.Total == 300m)), Times.Once);
        _unitOfWorkMock.Verify(u => u.Cuentas.InsertAsync(It.IsAny<Cuenta>()), Times.Never);
    }

    [Fact]
    public async Task GenerarCuentaAsync_PedidoNoExiste_RetornaError()
    {
        // Arrange
        Guid pedidoIdInexistente = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Pedidos.GetAsync(pedidoIdInexistente)).ReturnsAsync((Pedido?)null);

        // Act
        var result = await _sut.GenerarCuentaAsync(pedidoIdInexistente);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeFalse();
        result.Message.Should().Contain("Pedido no encontrado");
        result.Data.Should().BeNull();
        _loggerMock.Verify(l => l.LogError(It.Is<string>(s => s.Contains("Pedido no encontrado"))), Times.Once);
    }
}

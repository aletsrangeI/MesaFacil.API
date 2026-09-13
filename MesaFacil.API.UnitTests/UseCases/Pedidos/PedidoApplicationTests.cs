using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Pedido;
using FluentAssertions;
using Interface.Persistence;
using Moq;
using UseCases.Pedidos;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Pedidos;

public class PedidoApplicationTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAppLogger<PedidoApplication>> _loggerMock;
    private readonly Mock<IFoliadorSucursalService> _foliadorMock;
    private readonly PedidoApplication _sut;

    public PedidoApplicationTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<IAppLogger<PedidoApplication>>();
        _foliadorMock = new Mock<IFoliadorSucursalService>();
        _foliadorMock.Setup(f => f.ObtenerSiguienteFolioAsync(It.IsAny<int>(), It.IsAny<DateOnly?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new PedidoApplication(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            new PedidoDTOValidator(),
            _loggerMock.Object,
            _foliadorMock.Object
        );
    }

    [Fact]
    public async Task GetAsync_CuandoPedidoExiste_RetornaExitoYDto()
    {
        // Arrange
        Guid pedidoId = Guid.NewGuid();
        var pedidoEntity = new Pedido
        {
            Id = pedidoId,
            IdEmpresa = 1,
            IdSucursal = 2,
            IdMesa = 5,
            IdEstadoPedido = 1
        };

        var pedidoDto = new PedidoDTO
        {
            Id = pedidoId,
            IdEmpresa = 1,
            IdSucursal = 2,
            IdMesa = 5,
            IdEstadoPedido = 1
        };

        _unitOfWorkMock.Setup(u => u.Pedidos.GetAsync(pedidoId)).ReturnsAsync(pedidoEntity);
        _mapperMock.Setup(m => m.Map<PedidoDTO>(pedidoEntity)).Returns(pedidoDto);

        // Act
        var result = await _sut.GetAsync(pedidoId);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Message.Should().Be("Pedido encontrado");
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(pedidoId);
        result.Data.IdMesa.Should().Be(5);

        _unitOfWorkMock.Verify(u => u.Pedidos.GetAsync(pedidoId), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_DatosValidos_InsertaYRetornaTrue()
    {
        // Arrange
        var pedidoDto = new PedidoDTO
        {
            IdEmpresa = 1,
            IdSucursal = 1,
            IdMesa = 3,
            IdTipoPedido = 1,
            IdEstadoPedido = 1,
            AbiertoEn = DateTime.UtcNow
        };

        var pedidoEntity = new Pedido
        {
            IdEmpresa = 1,
            IdSucursal = 1,
            IdMesa = 3,
            IdTipoPedido = 1,
            IdEstadoPedido = 1
        };

        _mapperMock.Setup(m => m.Map<Pedido>(pedidoDto)).Returns(pedidoEntity);
        _unitOfWorkMock.Setup(u => u.Pedidos.InsertAsync(pedidoEntity)).ReturnsAsync(true);

        // Act
        var result = await _sut.InsertAsync(pedidoDto);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Pedido creado correctamente");

        _unitOfWorkMock.Verify(u => u.Pedidos.InsertAsync(pedidoEntity), Times.Once);
    }

    [Fact]
    public async Task InsertConDetallesAsync_Exitoso_RetornaIdGenerado()
    {
        // Arrange
        var requestDto = new CrearPedidoRequestDTO
        {
            IdEmpresa = 1,
            IdSucursal = 1,
            IdMesa = 2,
            IdTipoPedido = 1,
            IdEstadoPedido = 1
        };

        var pedidoEntity = new Pedido
        {
            IdEmpresa = 1,
            IdSucursal = 1,
            IdMesa = 2
        };
        var expectedId = pedidoEntity.Id;

        _mapperMock.Setup(m => m.Map<Pedido>(requestDto)).Returns(pedidoEntity);
        _unitOfWorkMock.Setup(u => u.Pedidos.InsertAsync(pedidoEntity)).ReturnsAsync(true);

        // Act
        var result = await _sut.InsertConDetallesAsync(requestDto);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Data.Should().Be(expectedId);
        result.Message.Should().Be("Pedido con detalles creado correctamente");
    }
}

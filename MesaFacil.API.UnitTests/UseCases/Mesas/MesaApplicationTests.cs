using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Mesa;
using FluentAssertions;
using Interface.Persistence;
using Moq;
using UseCases.Mesas;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Mesas;

public class MesaApplicationTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAppLogger<MesaApplication>> _loggerMock;
    
    // Si la validación estuviera mockeada mediante interfaz, la agregaríamos aquí. 
    // Como en el UseCase se usa la clase concreta 'MesaDTOValidator', 
    // pasaremos null o una instancia según convenga si es necesario para la instanciación.
    
    private readonly MesaApplication _sut; // System Under Test

    public MesaApplicationTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<IAppLogger<MesaApplication>>();

        _sut = new MesaApplication(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            null, // Pasamos null si el validador no se usa en GetAsync
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetAsync_WhenMesaExists_ReturnsSuccessAndMesaDTO()
    {
        // Arrange
        int mesaId = 1;
        var mesaEntity = new Mesa { Id = mesaId, Codigo = "M1" };
        var mesaDTO = new MesaDTO { Id = mesaId, Codigo = "M1" };

        // Configuramos el mock del repositorio Mesas
        _unitOfWorkMock.Setup(uow => uow.Mesas.GetAsync(mesaId))
                       .ReturnsAsync(mesaEntity);

        // Configuramos el mock del Mapper
        _mapperMock.Setup(m => m.Map<MesaDTO>(mesaEntity))
                   .Returns(mesaDTO);

        // Act
        var result = await _sut.GetAsync(mesaId);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeTrue();
        result.Message.Should().Be("Mesa encontrado");
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(mesaId);
        result.Data.Codigo.Should().Be("M1");
        
        // Verificamos que el repositorio fue llamado exactamente una vez con ese id
        _unitOfWorkMock.Verify(uow => uow.Mesas.GetAsync(mesaId), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenMesaDoesNotExist_ReturnsResponseWithSuccessFalse()
    {
        // Arrange
        int mesaId = 99;

        // Simulamos que el repositorio no encuentra la mesa (retorna null)
        _unitOfWorkMock.Setup(uow => uow.Mesas.GetAsync(mesaId))
                       .ReturnsAsync((Mesa?)null);

        // Configuramos el mapper para retornar null (comportamiento usual)
        _mapperMock.Setup(m => m.Map<MesaDTO>(null))
                   .Returns((MesaDTO)null);

        // Act
        var result = await _sut.GetAsync(mesaId);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeFalse(); // Según la lógica actual de MesaApplication, si Data == null, isSuccess no se asigna a true, su valor por defecto es false.
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WhenExceptionIsThrown_ReturnsResponseWithErrorAndLogs()
    {
        // Arrange
        int mesaId = 1;
        var errorMessage = "Database connection error";

        // Simulamos que el repositorio lanza una excepción
        _unitOfWorkMock.Setup(uow => uow.Mesas.GetAsync(mesaId))
                       .ThrowsAsync(new Exception(errorMessage));

        // Act
        var result = await _sut.GetAsync(mesaId);

        // Assert
        result.Should().NotBeNull();
        result.isSuccess.Should().BeFalse();
        result.Message.Should().Be(errorMessage);
        result.Data.Should().BeNull();

        // Verificamos que el error fue registrado en el logger
        _loggerMock.Verify(l => l.LogError(errorMessage), Times.Once);
    }
}

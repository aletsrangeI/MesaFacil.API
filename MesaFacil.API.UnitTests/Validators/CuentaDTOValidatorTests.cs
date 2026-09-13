using DTO.Cuenta;
using FluentAssertions;
using FluentValidation.TestHelper;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.Validators;

public class CuentaDTOValidatorTests
{
    private readonly CuentaDTOValidator _validator;

    public CuentaDTOValidatorTests()
    {
        _validator = new CuentaDTOValidator();
    }

    [Fact]
    public void Validate_CuandoTotalNoCoincideConFormula_DebeTenerError()
    {
        // Total debe ser Subtotal - DescuentoTotal + CargoServicio + ImpuestoTotal
        // Subtotal: 100, Descuento: 10, CargoServicio: 0, Impuestos: 16 -> Total esperado = 106
        var model = new CuentaDTO
        {
            IdPedido = Guid.NewGuid(),
            IdEstadoCuenta = 1,
            Subtotal = 100m,
            DescuentoTotal = 10m,
            CargoServicio = 0m,
            ImpuestoTotal = 16m,
            Total = 150m // Valor erróneo
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void Validate_CuandoImportesTienenMasDeDosDecimales_DebeTenerError()
    {
        // Arrange
        var model = new CuentaDTO
        {
            IdPedido = Guid.NewGuid(),
            IdEstadoCuenta = 1,
            Subtotal = 100.123m, // 3 decimales
            DescuentoTotal = 0m,
            CargoServicio = 0m,
            ImpuestoTotal = 0m,
            Total = 100.123m
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_CuandoCuentaEsConsistente_NoDebeTenerErrores()
    {
        // Arrange: 100 - 10 + 5 + 16 = 111.00
        var model = new CuentaDTO
        {
            IdPedido = Guid.NewGuid(),
            IdEstadoCuenta = 2,
            Subtotal = 100.00m,
            DescuentoTotal = 10.00m,
            CargoServicio = 5.00m,
            ImpuestoTotal = 16.00m,
            Total = 111.00m
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

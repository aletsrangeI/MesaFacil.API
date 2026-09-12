using DTO.Pago;
using FluentValidation.TestHelper;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.Validators;

public class PagoDTOValidatorTests
{
    private readonly PagoDTOValidator _validator;

    public PagoDTOValidatorTests()
    {
        _validator = new PagoDTOValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-0.01)]
    public void Validate_CuandoMontoEsMenorOIgualACero_DebeTenerError(decimal montoInvalido)
    {
        // Arrange
        var model = new PagoDTO { Monto = montoInvalido };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Monto);
    }

    [Fact]
    public void Validate_CuandoPropinaEsNegativa_DebeTenerError()
    {
        // Arrange
        var model = new PagoDTO { Propina = -5m };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Propina);
    }

    [Theory]
    [InlineData("")]
    [InlineData("MX")]
    [InlineData("PESOS")]
    public void Validate_CuandoMonedaNoTiene3Caracteres_DebeTenerError(string monedaInvalida)
    {
        // Arrange
        var model = new PagoDTO { Moneda = monedaInvalida };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Moneda);
    }

    [Fact]
    public void Validate_CuandoFechaEnFuturo_DebeTenerError()
    {
        // Arrange
        var model = new PagoDTO { PagadoEn = DateTime.UtcNow.AddDays(1) };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PagadoEn);
    }

    [Fact]
    public void Validate_CuandoPagoEsValido_NoDebeTenerErrores()
    {
        // Arrange
        var model = new PagoDTO
        {
            IdCuenta = 1,
            Monto = 150.00m,
            Moneda = "MXN",
            Propina = 15.00m,
            PagadoEn = DateTime.UtcNow.AddMinutes(-1),
            IdMetodoDePago = 1,
            Referencia = "REF12345"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

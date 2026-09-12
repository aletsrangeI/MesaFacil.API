using DTO.Compras;
using FluentAssertions;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Compras;

public class RfcValidationTests
{
    [Theory]
    [InlineData("BME8808116B1")] // Persona Moral (12 caracteres)
    [InlineData("ABC680524P76")] // Persona Moral
    [InlineData("MORA8501019K8")] // Persona Física (13 caracteres)
    [InlineData("GODE561231GR8")] // Persona Física
    [InlineData("XAXX010101000")] // Genérico nacional
    [InlineData("XEXX010101000")] // Genérico extranjero
    public void ValidarRfc_RfcsValidos_RetornaTrue(string rfc)
    {
        bool valido = RfcHelper.EsRfcValido(rfc, out string error);

        valido.Should().BeTrue();
        error.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("ABC")] // Demasiado corto
    [InlineData("ABC12345678901234")] // Demasiado largo
    [InlineData("123456789012")] // Solo números
    [InlineData("ABCD999999XYZ")] // Mes 99 inválido
    public void ValidarRfc_RfcsInvalidos_RetornaFalse(string? rfc)
    {
        bool valido = RfcHelper.EsRfcValido(rfc, out string error);

        valido.Should().BeFalse();
        error.Should().NotBeNullOrWhiteSpace();
    }
}

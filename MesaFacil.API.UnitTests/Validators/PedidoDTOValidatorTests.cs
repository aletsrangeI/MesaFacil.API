using DTO.Pedido;
using FluentValidation.TestHelper;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.Validators;

public class PedidoDTOValidatorTests
{
    private readonly PedidoDTOValidator _validator;

    public PedidoDTOValidatorTests()
    {
        _validator = new PedidoDTOValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_CuandoIdEmpresaInvalido_DebeTenerError(int idEmpresaInvalido)
    {
        var model = new PedidoDTO { IdEmpresa = idEmpresaInvalido };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.IdEmpresa);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_CuandoIdSucursalInvalido_DebeTenerError(int idSucursalInvalido)
    {
        var model = new PedidoDTO { IdSucursal = idSucursalInvalido };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.IdSucursal);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_CuandoCargoServicioPctFueraDeRango_DebeTenerError(decimal pctInvalido)
    {
        var model = new PedidoDTO { CargoServicioPct = pctInvalido };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CargoServicioPct);
    }

    [Fact]
    public void Validate_CuandoCerradoEnEsAnteriorAAbiertoEn_DebeTenerError()
    {
        var ahora = DateTime.UtcNow;
        var model = new PedidoDTO
        {
            AbiertoEn = ahora,
            CerradoEn = ahora.AddHours(-1), // Anterior a la apertura
            CerradoPor = 1
        };

        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CerradoEn);
    }

    [Fact]
    public void Validate_CuandoPedidoEsValido_NoDebeTenerErrores()
    {
        var ahora = DateTime.UtcNow;
        var model = new PedidoDTO
        {
            IdEmpresa = 1,
            IdSucursal = 1,
            IdTipoPedido = 1,
            IdEstadoPedido = 1,
            AbiertoEn = ahora,
            CargoServicioPct = 10m
        };

        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

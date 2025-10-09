// Validator/DescuentoAplicadoDTOValidator.cs
using DTO.DescuentoAplicado;
using FluentValidation;

namespace Validator;

public class DescuentoAplicadoDTOValidator : AbstractValidator<DescuentoAplicadoDTO>
{
    public DescuentoAplicadoDTOValidator()
    {
        // FK requeridas
        RuleFor(x => x.IdCuenta)
            .GreaterThan(0).WithMessage("El campo IdCuenta es requerido y debe ser mayor a 0.");

        RuleFor(x => x.TipoCatalogId)
            .GreaterThan(0).WithMessage("El campo TipoCatalogId es requerido y debe ser mayor a 0.");

        RuleFor(x => x.TipoItemId)
            .GreaterThan(0).WithMessage("El campo TipoItemId es requerido y debe ser mayor a 0.");

        // Valor no negativo
        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0).WithMessage("El campo Valor no puede ser negativo.");

        // Alcance opcional pero con límite de longitud
        RuleFor(x => x.Alcance)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Alcance))
            .WithMessage("El campo Alcance no debe exceder los 100 caracteres.");

        // Condiciones opcional pero con límite de longitud
        RuleFor(x => x.Condiciones)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Condiciones))
            .WithMessage("El campo Condiciones no debe exceder los 250 caracteres.");
    }
}
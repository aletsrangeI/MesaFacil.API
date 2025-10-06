using DTO.Area;
using FluentValidation;

namespace Validator;

public class AreaDTOValidator : AbstractValidator<AreaDTO>
{
    public AreaDTOValidator()
    {
        RuleFor(x => x.IdSucursal)
            .GreaterThan(0)
            .WithMessage("El campo IdSucursal debe ser mayor que 0.");

        // Nombre obligatorio (no nulo/ni vacío/ni solo espacios), con tamaño máximo
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(100).WithMessage("El campo Nombre no debe exceder 100 caracteres.");

        // Orden no negativo
        RuleFor(x => x.Orden)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El campo Orden no puede ser negativo.");
    }
}
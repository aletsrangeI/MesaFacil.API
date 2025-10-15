// Validator/CategoriaMenuDTOValidator.cs
using DTO.CategoriaMenu;
using FluentValidation;

namespace Validator;

public class CategoriaMenuDTOValidator : AbstractValidator<CategoriaMenuDTO>
{
    public CategoriaMenuDTOValidator()
    {
        // IdMenu requerido y válido (> 0)
        RuleFor(x => x.IdMenu)
            .GreaterThan(0)
            .WithMessage("El campo IdMenu es requerido y debe ser mayor a 0.");

        // Nombre requerido, sin espacios en blanco, máximo 100 caracteres
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(100).WithMessage("El campo Nombre no debe exceder los 100 caracteres.");

        // Orden no negativo
        RuleFor(x => x.Orden)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El campo Orden no puede ser negativo.");
    }
}
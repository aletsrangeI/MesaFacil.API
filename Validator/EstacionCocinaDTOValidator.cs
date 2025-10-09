using DTO.EstacionCocina;
using FluentValidation;

namespace Validator;

public class EstacionCocinaDTOValidator : AbstractValidator<EstacionCocinaDTO>
{
    public EstacionCocinaDTOValidator()
    {
        // TODO: Agrega las reglas de validación específicas para EstacionCocinaDTO.
        // Ejemplo:
        // RuleFor(x => x.Code).NotEmpty().WithMessage("El campo Code es requerido");
        // RuleFor(x => x.Name).NotEmpty().WithMessage("El campo Name es requerido");
    }
}
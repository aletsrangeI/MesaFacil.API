using DTO.CategoriaMenu;
using FluentValidation;

namespace Validator;

public class CategoriaMenuDTOValidator : AbstractValidator<CategoriaMenuDTO>
{
    public CategoriaMenuDTOValidator()
    {
        // TODO: Agrega las reglas de validación específicas para CategoriaMenuDTO.
        // Ejemplo:
        // RuleFor(x => x.Code).NotEmpty().WithMessage("El campo Code es requerido");
        // RuleFor(x => x.Name).NotEmpty().WithMessage("El campo Name es requerido");
    }
}
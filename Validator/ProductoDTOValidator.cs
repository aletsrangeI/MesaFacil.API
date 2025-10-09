using DTO.Producto;
using FluentValidation;

namespace Validator;

public class ProductoDTOValidator : AbstractValidator<ProductoDTO>
{
    public ProductoDTOValidator()
    {
        // TODO: Agrega las reglas de validación específicas para ProductoDTO.
        // Ejemplo:
        // RuleFor(x => x.Code).NotEmpty().WithMessage("El campo Code es requerido");
        // RuleFor(x => x.Name).NotEmpty().WithMessage("El campo Name es requerido");
    }
}
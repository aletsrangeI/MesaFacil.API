using DTO.CatalogItem;
using FluentValidation;

namespace Validator;

public class CatalogItemDTOValidator : AbstractValidator<CatalogItemDTO>
{
    public CatalogItemDTOValidator()
    {
        // TODO: Agrega las reglas de validación específicas para CatalogItemDTO.
        // Ejemplo:
        // RuleFor(x => x.Code).NotEmpty().WithMessage("El campo Code es requerido");
        // RuleFor(x => x.Name).NotEmpty().WithMessage("El campo Name es requerido");
        
        RuleFor(x => x.CatalogId).NotNull();
    }
}
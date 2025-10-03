using DTO.Catalog;
using FluentValidation;

namespace Validator;

public class CatalogDTOValidator : AbstractValidator<CatalogDTO>
{
    public CatalogDTOValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("El campo es requerido");
        RuleFor(x => x.Name).NotEmpty().WithMessage("El campo es requerido");
    }
}
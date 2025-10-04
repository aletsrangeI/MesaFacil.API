using DTO.CatalogItem;
using FluentValidation;

namespace Validator;

public class CatalogItemDTOValidator : AbstractValidator<CatalogItemDTO>
{
    public CatalogItemDTOValidator()
    {
        RuleFor(x => x.CatalogId).NotNull();
    }
}
using System.Data;
using DTO.CatCredencial;
using FluentValidation;

namespace Validator;

public class CatCredencialDTOValidator : AbstractValidator<CatCredencialDTO>
{
    public CatCredencialDTOValidator()
    {
        RuleFor(x => x.Id).NotNull();
        RuleFor(x => x.Descripcion).NotEmpty();
    }
}
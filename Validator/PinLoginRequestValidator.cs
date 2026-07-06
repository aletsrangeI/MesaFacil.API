using FluentValidation;
using Interface.UseCases;

namespace Validator;

public class PinLoginRequestValidator : AbstractValidator<PinLoginRequest>
{
    public PinLoginRequestValidator()
    {
        RuleFor(x => x.UserOrEmail).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Pin).NotEmpty().MaximumLength(32);
        RuleFor(x => x.EmpresaId).NotEqual(0);
        RuleFor(x => x.SucursalId).NotEqual(0);
    }
}

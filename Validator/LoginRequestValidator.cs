using FluentValidation;
using Interface.UseCases;

namespace Validator;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UserOrEmail).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.EmpresaId).NotEqual(0);
        RuleFor(x => x.SucursalId).NotEqual(0);
    }
}
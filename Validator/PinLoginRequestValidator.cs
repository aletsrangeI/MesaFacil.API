using FluentValidation;
using Interface.UseCases;

namespace Validator;

public class PinLoginRequestValidator : AbstractValidator<PinLoginRequest>
{
    public PinLoginRequestValidator()
    {
        RuleFor(x => x.UserOrEmail).MaximumLength(256);
        RuleFor(x => x.Pin).NotEmpty().MaximumLength(32);
        RuleFor(x => x.EmpresaId).NotEqual(0);
        RuleFor(x => x.SucursalId).NotEqual(0);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.UserOrEmail) || (x.UsuarioId.HasValue && x.UsuarioId.Value > 0))
            .WithMessage("Debe proporcionar el UserOrEmail o el UsuarioId para iniciar sesión.");
    }
}

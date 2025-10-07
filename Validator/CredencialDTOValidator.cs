// Validator/CredencialDTOValidator.cs

using DTO.Credencial;
using FluentValidation;

namespace Validator;

public class CredencialDTOValidator : AbstractValidator<CredencialDTO>
{
    public CredencialDTOValidator()
    {
        // Usuario requerido
        RuleFor(x => x.IdUsuario)
            .GreaterThan(0)
            .WithMessage("El campo IdUsuario es requerido y debe ser mayor a 0.");

        // TipoCatalogId requerido
        RuleFor(x => x.TipoCatalogId)
            .GreaterThan(0)
            .WithMessage("El campo TipoCatalogId es requerido y debe ser mayor a 0.");

        // TipoItemId requerido
        RuleFor(x => x.TipoItemId)
            .GreaterThan(0)
            .WithMessage("El campo TipoItemId es requerido y debe ser mayor a 0.");

        // Hash requerido
        RuleFor(x => x.Hash)
            .NotEmpty().WithMessage("El campo Hash es requerido.")
            .MaximumLength(512).WithMessage("El campo Hash no debe exceder los 512 caracteres.");

        // Salt opcional, pero con máximo de caracteres si se proporciona
        RuleFor(x => x.Salt)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Salt))
            .WithMessage("El campo Salt no debe exceder los 256 caracteres.");
    }
}
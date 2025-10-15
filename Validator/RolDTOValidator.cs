using DTO.Rol;
using FluentValidation;

namespace Validator;

public class RolDTOValidator : AbstractValidator<RolDTO>
{
    public RolDTOValidator()
    {
        // Nombre requerido
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(100).WithMessage("El campo Nombre no debe exceder los 100 caracteres.");

        // ConcurrencyStamp opcional con límite
        RuleFor(x => x.ConcurrencyStamp)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.ConcurrencyStamp))
            .WithMessage("El campo ConcurrencyStamp no debe exceder los 200 caracteres.");
    }
}
using DTO.Usuario;
using FluentValidation;

namespace Validator;

public class UsuarioDTOValidator : AbstractValidator<UsuarioDTO>
{
    public UsuarioDTOValidator()
    {
        // Empresa requerida
        RuleFor(x => x.IdEmpresa)
            .GreaterThan(0)
            .WithMessage("El campo IdEmpresa es requerido y debe ser mayor a 0.");

        // NombreCompleto requerido, con trimming y tamaño máximo
        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El campo NombreCompleto es requerido.")
            .MaximumLength(150).WithMessage("El campo NombreCompleto no debe exceder 150 caracteres.");

        // Correo opcional pero válido si se proporciona
        RuleFor(x => x.Correo)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Correo))
            .WithMessage("El campo Correo debe tener un formato válido.")
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Correo))
            .WithMessage("El campo Correo no debe exceder 200 caracteres.");

        // Pin opcional pero con restricciones de longitud si se proporciona
        RuleFor(x => x.Pin)
            .MaximumLength(32).WithMessage("El PIN no debe exceder los 32 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Pin));
    }
}
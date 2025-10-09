using DTO.Empresa;
using FluentValidation;

namespace Validator;

public class EmpresaDTOValidator : AbstractValidator<EmpresaDTO>
{
    public EmpresaDTOValidator()
    {
        // Nombre requerido
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(150).WithMessage("El campo Nombre no debe exceder los 150 caracteres.");

        // RFC opcional pero con formato válido si se proporciona
        RuleFor(x => x.Rfc)
            .Matches(@"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Rfc))
            .WithMessage("El campo RFC no tiene un formato válido.");
    }
}
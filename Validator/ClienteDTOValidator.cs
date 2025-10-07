// Validator/ClienteDTOValidator.cs

using DTO.Cliente;
using FluentValidation;

namespace Validator;

public class ClienteDTOValidator : AbstractValidator<ClienteDTO>
{
    public ClienteDTOValidator()
    {
        // IdEmpresa obligatorio
        RuleFor(x => x.IdEmpresa)
            .GreaterThan(0)
            .WithMessage("El campo IdEmpresa es requerido y debe ser mayor a 0.");

        // Nombre obligatorio con máximo de caracteres
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(150).WithMessage("El campo Nombre no debe exceder los 150 caracteres.");

        // Teléfono opcional, pero si viene debe coincidir con un patrón (10 dígitos en este ejemplo)
        RuleFor(x => x.Telefono)
            .Matches(@"^\d{10}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Telefono))
            .WithMessage("El campo Teléfono debe contener 10 dígitos numéricos.");

        // Correo opcional, pero válido si se proporciona
        RuleFor(x => x.Correo)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Correo))
            .WithMessage("El campo Correo debe tener un formato válido.");
    }
}
using DTO.AccesoRuta;
using FluentValidation;

using DTO.AccesoRuta;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Validator;

public class AccesoRutaDTOValidator : AbstractValidator<AccesoRutaDTO>
{
    public AccesoRutaDTOValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(100).WithMessage("Nombre no debe exceder 100 caracteres.");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("El campo Path es requerido.")
            .MaximumLength(200).WithMessage("Path no debe exceder 200 caracteres.")
            // Acepta rutas tipo /admin, /users/:id, /mesero/pedidos, etc.
            .Matches(new Regex(@"^\/[A-Za-z0-9\-\/:_]*$"))
            .WithMessage("Path debe iniciar con '/' y solo puede contener letras, números, '-', '/', ':', '_'.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.Descripcion))
            .WithMessage("Descripcion no debe exceder 300 caracteres.");
    }
}

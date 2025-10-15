using DTO.RolAccesoRuta;
using FluentValidation;

namespace Validator;

public class RolAccesoRutaDTOValidator : AbstractValidator<RolAccesoRutaDTO>
{
    public RolAccesoRutaDTOValidator()
    {
        RuleFor(x => x.IdRol)
            .GreaterThan(0)
            .WithMessage("El campo IdRol es requerido y debe ser mayor a 0.");

        RuleFor(x => x.IdAccesoRuta)
            .GreaterThan(0)
            .WithMessage("El campo IdAccesoRuta es requerido y debe ser mayor a 0.");
    }
}
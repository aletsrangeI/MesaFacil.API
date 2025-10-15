using DTO.Turno;
using FluentValidation;

namespace Validator;

public class TurnoDTOValidator : AbstractValidator<TurnoDTO>
{
    public TurnoDTOValidator()
    {
        RuleFor(x => x.IdUsuario)
            .GreaterThan(0)
            .WithMessage("El campo IdUsuario es requerido y debe ser mayor a 0.");

        // Sucursal requerida
        RuleFor(x => x.IdSucursal)
            .GreaterThan(0)
            .WithMessage("El campo IdSucursal es requerido y debe ser mayor a 0.");

        // Apertura obligatoria y no futura
        RuleFor(x => x.Apertura)
            .NotEmpty().WithMessage("El campo Apertura es requerido.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("La fecha de Apertura no puede estar en el futuro.");

        // Cierre opcional, pero válido si existe
        RuleFor(x => x.Cierre)
            .GreaterThanOrEqualTo(x => x.Apertura)
            .When(x => x.Cierre.HasValue)
            .WithMessage("La fecha de Cierre debe ser mayor o igual a la de Apertura.");

        // CajaInicial requerida y no negativa
        RuleFor(x => x.CajaInicial)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El campo CajaInicial no puede ser negativo.");

        // CajaFinal opcional, pero no negativa y consistente
        RuleFor(x => x.CajaFinal)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CajaFinal.HasValue)
            .WithMessage("El campo CajaFinal no puede ser negativo.");

        RuleFor(x => x.CajaFinal)
            .GreaterThanOrEqualTo(x => x.CajaInicial)
            .When(x => x.CajaFinal.HasValue)
            .WithMessage("El campo CajaFinal no puede ser menor que CajaInicial.");
    }
}
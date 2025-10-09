using DTO.MovimientoCaja;
using FluentValidation;

namespace Validator;

public class MovimientoCajaDTOValidator : AbstractValidator<MovimientoCajaDTO>
{
    public MovimientoCajaDTOValidator()
    {
        RuleFor(x => x.IdTurno)
            .GreaterThan(0)
            .WithMessage("El campo IdTurno es requerido y debe ser mayor a 0.");

        // Tipo obligatorio, con lista de valores permitidos
        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("El campo Tipo es requerido.")
            .MaximumLength(50).WithMessage("El campo Tipo no debe exceder los 50 caracteres.")
            .Must(t => new[] { "Egreso", "Ingreso", "Deposito" }.Contains(t))
            .WithMessage("El campo Tipo debe ser uno de los valores permitidos: Egreso, Ingreso o Deposito.");

        // Monto obligatorio y positivo
        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El campo Monto debe ser mayor a 0.");

        // Nota opcional con límite de longitud
        RuleFor(x => x.Nota)
            .MaximumLength(250)
            .When(x => !string.IsNullOrWhiteSpace(x.Nota))
            .WithMessage("El campo Nota no debe exceder los 250 caracteres.");
    }
}
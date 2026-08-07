using DTO.Mesa;
using FluentValidation;

namespace Validator;

public class MesaDTOValidator : AbstractValidator<MesaDTO>
{
    public MesaDTOValidator()
    {
        RuleFor(x => x.IdSucursal)
            .GreaterThan(0).WithMessage("IdSucursal es requerido y debe ser mayor a 0.");

        // IdArea opcional pero válido si se especifica
        RuleFor(x => x.IdArea)
            .GreaterThan(0)
            .When(x => x.IdArea.HasValue)
            .WithMessage("IdArea debe ser mayor a 0 cuando se especifique.");

        // Codigo requerido, trim y con patrón/longitud
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Codigo es requerido.")
            .MaximumLength(20).WithMessage("Codigo no debe exceder 20 caracteres.")
            .Matches(@"^[A-Za-z0-9\-_.]+$")
            .WithMessage("Codigo solo puede contener letras, números y - _ .");

        // Asientos mínimo 1 (ajusta el máximo si lo necesitas)
        RuleFor(x => x.Asientos)
            .GreaterThan(0).WithMessage("Asientos debe ser al menos 1.")
            .LessThanOrEqualTo(20).WithMessage("Asientos no debe exceder 20.");

        // Estado requerido (par cat/item)
        RuleFor(x => x.IdEstadoMesa)
            .GreaterThan(0).WithMessage("IdEstadoMesa es requerido y debe ser mayor a 0.");
    }
}
using DTO.DetalleCuenta;
using FluentValidation;

namespace Validator;

public class DetalleCuentaDTOValidator : AbstractValidator<DetalleCuentaDTO>
{
    public DetalleCuentaDTOValidator()
    {
        // FK obligatoria
        RuleFor(x => x.IdCuenta)
            .GreaterThan(0).WithMessage("El campo IdCuenta es requerido y debe ser mayor a 0.");

        // TipoOrigen requerido y con límite
        RuleFor(x => x.TipoOrigen)
            .NotEmpty().WithMessage("El campo TipoOrigen es requerido.")
            .MaximumLength(50).WithMessage("El campo TipoOrigen no debe exceder los 50 caracteres.");

        // IdOrigen opcional, pero si existe debe ser > 0
        RuleFor(x => x.IdOrigen)
            .GreaterThan(0)
            .When(x => x.IdOrigen.HasValue)
            .WithMessage("El campo IdOrigen debe ser mayor a 0 cuando se especifique.");

        // Descripción opcional con límite
        RuleFor(x => x.Descripcion)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Descripcion))
            .WithMessage("El campo Descripcion no debe exceder los 200 caracteres.");

        // Monto requerido, no negativo
        RuleFor(x => x.Monto)
            .GreaterThanOrEqualTo(0).WithMessage("El campo Monto no puede ser negativo.");
    }
}
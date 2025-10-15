using DTO.Pago;
using FluentValidation;

namespace Validator;

public class PagoDTOValidator : AbstractValidator<PagoDTO>
{
    public PagoDTOValidator()
    {
        RuleFor(x => x.IdCuenta)
            .GreaterThan(0).WithMessage("El campo IdCuenta es requerido y debe ser mayor a 0.");

        // Monto mayor que cero
        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El campo Monto debe ser mayor a 0.");

        // Moneda requerida, con longitud de 3 caracteres (ISO 4217, ej. MXN, USD, EUR)
        RuleFor(x => x.Moneda)
            .NotEmpty().WithMessage("El campo Moneda es requerido.")
            .Length(3).WithMessage("El campo Moneda debe tener exactamente 3 caracteres.");

        // Propina no negativa
        RuleFor(x => x.Propina)
            .GreaterThanOrEqualTo(0).WithMessage("El campo Propina no puede ser negativo.");

        // PagadoEn no puede estar en el futuro
        RuleFor(x => x.PagadoEn)
            .NotEmpty().WithMessage("El campo PagadoEn es requerido.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("La fecha PagadoEn no puede estar en el futuro.");

        // Referencia opcional con límite de longitud
        RuleFor(x => x.Referencia)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Referencia))
            .WithMessage("El campo Referencia no debe exceder los 100 caracteres.");

        // RecibidoPor opcional, pero si se especifica debe ser mayor a 0
        RuleFor(x => x.RecibidoPor)
            .GreaterThan(0)
            .When(x => x.RecibidoPor.HasValue)
            .WithMessage("El campo RecibidoPor debe ser mayor a 0 cuando se especifique.");

        // Método de pago requerido
        RuleFor(x => x.MetodoCatalogId)
            .GreaterThan(0).WithMessage("El campo MetodoCatalogId es requerido y debe ser mayor a 0.");

        RuleFor(x => x.MetodoItemId)
            .GreaterThan(0).WithMessage("El campo MetodoItemId es requerido y debe ser mayor a 0.");
    }
}
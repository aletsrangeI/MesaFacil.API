using DTO.Precio;
using FluentValidation;

namespace Validator;

public class PrecioDTOValidator : AbstractValidator<PrecioDTO>
{
    public PrecioDTOValidator()
    {
// Variante requerida
        RuleFor(x => x.IdVariante)
            .GreaterThan(0).WithMessage("El campo IdVariante es requerido y debe ser mayor a 0.");

        // Monto positivo
        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El campo Monto debe ser mayor a 0.");

        // Moneda requerida (ISO 4217 - 3 caracteres)
        RuleFor(x => x.Moneda)
            .NotEmpty().WithMessage("El campo Moneda es requerido.")
            .Length(3).WithMessage("El campo Moneda debe tener exactamente 3 caracteres.")
            .Matches("^[A-Z]{3}$")
            .WithMessage("El campo Moneda debe contener solo letras mayúsculas (ej: MXN, USD, EUR).");

        // Impuesto requerido
        RuleFor(x => x.ImpuestoCatalogId)
            .GreaterThan(0).WithMessage("El campo ImpuestoCatalogId es requerido y debe ser mayor a 0.");
        RuleFor(x => x.ImpuestoItemId)
            .GreaterThan(0).WithMessage("El campo ImpuestoItemId es requerido y debe ser mayor a 0.");

        // Validaciones de fechas
        RuleFor(x => x.ValidoHasta)
            .GreaterThanOrEqualTo(x => x.ValidoDesde)
            .When(x => x.ValidoHasta.HasValue && x.ValidoDesde.HasValue)
            .WithMessage("El campo ValidoHasta debe ser mayor o igual a ValidoDesde.");

        // Dias opcional con límite de longitud
        RuleFor(x => x.Dias)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Dias))
            .WithMessage("El campo Dias no debe exceder los 50 caracteres.");

        // Horario opcional con límite de longitud
        RuleFor(x => x.Horario)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Horario))
            .WithMessage("El campo Horario no debe exceder los 50 caracteres.");
    }
}
// Validator/CuentaDTOValidator.cs

using DTO.Cuenta;
using FluentValidation;

namespace Validator;

public class CuentaDTOValidator : AbstractValidator<CuentaDTO>
{
    public CuentaDTOValidator()
    {
        // FK requeridas
        RuleFor(x => x.IdPedido)
            .GreaterThan(0).WithMessage("IdPedido es requerido y debe ser mayor a 0.");

        RuleFor(x => x.EstadoCatalogId)
            .GreaterThan(0).WithMessage("EstadoCatalogId es requerido y debe ser mayor a 0.");

        RuleFor(x => x.EstadoItemId)
            .GreaterThan(0).WithMessage("EstadoItemId es requerido y debe ser mayor a 0.");

        // Montos no negativos
        RuleFor(x => x.Subtotal)
            .GreaterThanOrEqualTo(0).WithMessage("Subtotal no puede ser negativo.");

        RuleFor(x => x.DescuentoTotal)
            .GreaterThanOrEqualTo(0).WithMessage("DescuentoTotal no puede ser negativo.");

        RuleFor(x => x.CargoServicio)
            .GreaterThanOrEqualTo(0).WithMessage("CargoServicio no puede ser negativo.");

        RuleFor(x => x.ImpuestoTotal)
            .GreaterThanOrEqualTo(0).WithMessage("ImpuestoTotal no puede ser negativo.");

        RuleFor(x => x.Total)
            .GreaterThanOrEqualTo(0).WithMessage("Total no puede ser negativo.");

        // Consistencia de totales: Total = Subtotal - DescuentoTotal + CargoServicio + ImpuestoTotal
        // Elimina esta regla si el Total lo calculas en backend.
        RuleFor(x => x)
            .Must(x => x.Total == x.Subtotal - x.DescuentoTotal + x.CargoServicio + x.ImpuestoTotal)
            .WithMessage("Total debe ser igual a Subtotal - DescuentoTotal + CargoServicio + ImpuestoTotal.");

        // (Opcional) Enforzar 2 decimales máximo
        // Descomenta si quieres restringir escala a 2 decimales.

        RuleForEach(x => new[] { x.Subtotal, x.DescuentoTotal, x.CargoServicio, x.ImpuestoTotal, x.Total })
            .Must(v => HasMaxTwoDecimals(v))
            .WithMessage("Los importes deben tener como máximo 2 decimales.");
    }

    private static bool HasMaxTwoDecimals(decimal value)
    {
        // Hasta 2 decimales
        return decimal.Round(value, 2) == value;
    }
}
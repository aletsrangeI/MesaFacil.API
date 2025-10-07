// Validator/CorteCajaDTOValidator.cs

using DTO.CorteCaja;
using FluentValidation;

namespace Validator;

public class CorteCajaDTOValidator : AbstractValidator<CorteCajaDTO>
{
    public CorteCajaDTOValidator()
    {
        // Turno y Sucursal opcionales, pero si vienen deben ser > 0
        RuleFor(x => x.IdTurno)
            .GreaterThan(0)
            .When(x => x.IdTurno.HasValue)
            .WithMessage("El campo IdTurno debe ser mayor a 0 cuando se especifique.");

        RuleFor(x => x.IdSucursal)
            .GreaterThan(0)
            .When(x => x.IdSucursal.HasValue)
            .WithMessage("El campo IdSucursal debe ser mayor a 0 cuando se especifique.");

        // Rango de fechas válido
        RuleFor(x => x.FechaInicio)
            .NotEmpty().WithMessage("El campo FechaInicio es requerido.");

        RuleFor(x => x.FechaFin)
            .NotEmpty().WithMessage("El campo FechaFin es requerido.")
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("FechaFin debe ser mayor o igual que FechaInicio.");

        // Montos no negativos
        RuleFor(x => x.TotalVentas)
            .GreaterThanOrEqualTo(0).WithMessage("TotalVentas no puede ser negativo.");

        RuleFor(x => x.TotalPagos)
            .GreaterThanOrEqualTo(0).WithMessage("TotalPagos no puede ser negativo.");

        RuleFor(x => x.TotalEfectivo)
            .GreaterThanOrEqualTo(0).WithMessage("TotalEfectivo no puede ser negativo.");

        RuleFor(x => x.TotalTarjeta)
            .GreaterThanOrEqualTo(0).WithMessage("TotalTarjeta no puede ser negativo.");

        RuleFor(x => x.TotalEgresos)
            .GreaterThanOrEqualTo(0).WithMessage("TotalEgresos no puede ser negativo.");

        RuleFor(x => x.CajaEsperada)
            .GreaterThanOrEqualTo(0).WithMessage("CajaEsperada no puede ser negativa.");

        RuleFor(x => x.Declarado)
            .GreaterThanOrEqualTo(0).WithMessage("Declarado no puede ser negativo.");

        // Consistencia de diferencia (quita esta regla si la calculas en el backend)
        RuleFor(x => x.Diferencia)
            .Must((dto, dif) => dif == dto.Declarado - dto.CajaEsperada)
            .WithMessage("Diferencia debe ser igual a Declarado - CajaEsperada.");
    }
}
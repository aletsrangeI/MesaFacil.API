using DTO.PedidoModificador;
using FluentValidation;

namespace Validator;

public class PedidoModificadorDTOValidator : AbstractValidator<PedidoModificadorDTO>
{
    public PedidoModificadorDTOValidator()
    {
        RuleFor(x => x.IdDetalle)
            .GreaterThan(0).WithMessage("IdDetalle es requerido y debe ser mayor a 0.");

        RuleFor(x => x.IdOpcion)
            .GreaterThan(0).WithMessage("IdOpcion es requerido y debe ser mayor a 0.");

        // Precio extra no negativo (ajusta si permites descuentos negativos)
        RuleFor(x => x.PrecioExtra)
            .GreaterThanOrEqualTo(0).WithMessage("PrecioExtra no puede ser negativo.");

        // (Opcional) Forzar hasta 2 decimales:
        RuleFor(x => x.PrecioExtra)
            .Must(v => decimal.Round(v, 2) == v)
            .WithMessage("PrecioExtra debe tener como máximo 2 decimales.");
    }
}
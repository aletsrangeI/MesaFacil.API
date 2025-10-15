using DTO.TicketCocina;
using FluentValidation;

namespace Validator;

public class TicketCocinaDTOValidator : AbstractValidator<TicketCocinaDTO>
{
    public TicketCocinaDTOValidator()
    {
        RuleFor(x => x.IdEstacion)
            .GreaterThan(0).WithMessage("El campo IdEstacion es requerido y debe ser mayor a 0.");

        RuleFor(x => x.IdPedido)
            .GreaterThan(0).WithMessage("El campo IdPedido es requerido y debe ser mayor a 0.");

        // Estado requerido (par cat/item)
        RuleFor(x => x.EstadoCatalogId)
            .GreaterThan(0).WithMessage("El campo EstadoCatalogId es requerido y debe ser mayor a 0.");

        RuleFor(x => x.EstadoItemId)
            .GreaterThan(0).WithMessage("El campo EstadoItemId es requerido y debe ser mayor a 0.");

        // CompletadoEn opcional, pero si existe debe ser válido (no en el futuro)
        RuleFor(x => x.CompletadoEn)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.CompletadoEn.HasValue)
            .WithMessage("El campo CompletadoEn no puede estar en el futuro.");
    }
}
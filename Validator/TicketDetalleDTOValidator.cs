using DTO.TicketDetalle;
using FluentValidation;

namespace Validator;

public class TicketDetalleDTOValidator : AbstractValidator<TicketDetalleDTO>
{
    public TicketDetalleDTOValidator()
    {
        RuleFor(x => x.IdTicket)
            .GreaterThan(0)
            .WithMessage("El campo IdTicket es requerido y debe ser mayor a 0.");

        // Detalle requerido
        RuleFor(x => x.IdDetalle)
            .GreaterThan(0)
            .WithMessage("El campo IdDetalle es requerido y debe ser mayor a 0.");

        // Estado requerido
        RuleFor(x => x.IdEstadoItemKDS)
            .GreaterThan(0)
            .WithMessage("El campo IdEstadoItemKDS es requerido y debe ser mayor a 0.");
    }
}
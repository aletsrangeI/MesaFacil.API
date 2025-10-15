using DTO.EventoPedido;
using FluentValidation;

namespace Validator;

public class EventoPedidoDTOValidator : AbstractValidator<EventoPedidoDTO>
{
    public EventoPedidoDTOValidator()
    {
        // Pedido obligatorio
        RuleFor(x => x.IdPedido)
            .GreaterThan(0)
            .WithMessage("El campo IdPedido es requerido y debe ser mayor a 0.");

        // Usuario opcional pero válido si se especifica
        RuleFor(x => x.IdUsuario)
            .GreaterThan(0)
            .When(x => x.IdUsuario.HasValue)
            .WithMessage("El campo IdUsuario debe ser mayor a 0 cuando se especifique.");

        // TipoEvento opcional, con máximo de caracteres
        RuleFor(x => x.TipoEvento)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.TipoEvento))
            .WithMessage("El campo TipoEvento no debe exceder los 100 caracteres.");

        // Payload opcional, con límite de tamaño
        RuleFor(x => x.Payload)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Payload))
            .WithMessage("El campo Payload no debe exceder los 2000 caracteres.");
    }
}
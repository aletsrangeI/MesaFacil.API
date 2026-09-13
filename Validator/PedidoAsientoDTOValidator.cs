using DTO.PedidoAsiento;
using FluentValidation;

namespace Validator;

public class PedidoAsientoDTOValidator : AbstractValidator<PedidoAsientoDTO>
{
    public PedidoAsientoDTOValidator()
    {
        // Pedido requerido
        RuleFor(x => x.IdPedido)
            .NotEqual(Guid.Empty)
            .WithMessage("El campo IdPedido es requerido.");

        // Número de asiento requerido y mayor a 0
        RuleFor(x => x.NumeroAsiento)
            .GreaterThan(0)
            .WithMessage("El campo NumeroAsiento es requerido y debe ser mayor a 0.");
    }
}
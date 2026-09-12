using DTO.TiposPedido;
using FluentValidation;

namespace Validator.TiposPedido;

public class TipoPedidoDTOValidator : AbstractValidator<TipoPedidoDTO>
{
    public TipoPedidoDTOValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().WithMessage("La descripción es requerida.");
    }
}

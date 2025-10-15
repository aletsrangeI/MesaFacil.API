using DTO.PedidoDetalle;
using FluentValidation;

namespace Validator;

public class PedidoDetalleDTOValidator : AbstractValidator<PedidoDetalleDTO>
{
    public PedidoDetalleDTOValidator()
    {
        // FKs requeridas / opcionales válidas
        RuleFor(x => x.IdPedido)
            .GreaterThan(0).WithMessage("IdPedido es requerido y debe ser mayor a 0.");

        RuleFor(x => x.IdAsiento)
            .GreaterThan(0)
            .When(x => x.IdAsiento.HasValue)
            .WithMessage("IdAsiento debe ser mayor a 0 cuando se especifique.");

        RuleFor(x => x.IdProducto)
            .GreaterThan(0).WithMessage("IdProducto es requerido y debe ser mayor a 0.");

        RuleFor(x => x.IdVariante)
            .GreaterThan(0)
            .When(x => x.IdVariante.HasValue)
            .WithMessage("IdVariante debe ser mayor a 0 cuando se especifique.");

        // Cantidad y precio
        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("Cantidad debe ser mayor a 0.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThanOrEqualTo(0).WithMessage("PrecioUnitario no puede ser negativo.");

        // Notas opcional con límite
        RuleFor(x => x.Notas)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notas))
            .WithMessage("Notas no debe exceder los 500 caracteres.");

        // Estado requerido (par cat/item)
        RuleFor(x => x.EstadoCatalogId)
            .GreaterThan(0).WithMessage("EstadoCatalogId es requerido y debe ser mayor a 0.");
        RuleFor(x => x.EstadoItemId)
            .GreaterThan(0).WithMessage("EstadoItemId es requerido y debe ser mayor a 0.");

        // Impuesto requerido (par cat/item)
        RuleFor(x => x.ImpuestoCatalogId)
            .GreaterThan(0).WithMessage("ImpuestoCatalogId es requerido y debe ser mayor a 0.");
        RuleFor(x => x.ImpuestoItemId)
            .GreaterThan(0).WithMessage("ImpuestoItemId es requerido y debe ser mayor a 0.");

        // (Opcional) Escala de decimales: hasta 2
        // Descomenta si quieres forzar 2 decimales máximos:

        RuleFor(x => x.Cantidad)
            .Must(HasMaxTwoDecimals).WithMessage("Cantidad debe tener como máximo 2 decimales.");
        RuleFor(x => x.PrecioUnitario)
            .Must(HasMaxTwoDecimals).WithMessage("PrecioUnitario debe tener como máximo 2 decimales.");
    }

    private static bool HasMaxTwoDecimals(decimal v) => decimal.Round(v, 2) == v;
}
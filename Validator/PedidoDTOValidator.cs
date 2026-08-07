using DTO.Pedido;
using FluentValidation;

namespace Validator;

public class PedidoDTOValidator : AbstractValidator<PedidoDTO>
{
public PedidoDTOValidator()
    {
        // Empresa y Sucursal requeridas
        RuleFor(x => x.IdEmpresa)
            .GreaterThan(0).WithMessage("IdEmpresa es requerido y debe ser mayor a 0.");

        RuleFor(x => x.IdSucursal)
            .GreaterThan(0).WithMessage("IdSucursal es requerido y debe ser mayor a 0.");

        // Mesa y Cliente opcionales pero válidos si se especifican
        RuleFor(x => x.IdMesa)
            .GreaterThan(0)
            .When(x => x.IdMesa.HasValue)
            .WithMessage("IdMesa debe ser mayor a 0 cuando se especifique.");

        RuleFor(x => x.IdCliente)
            .GreaterThan(0)
            .When(x => x.IdCliente.HasValue)
            .WithMessage("IdCliente debe ser mayor a 0 cuando se especifique.");

        // Usuarios opcionales pero válidos si se especifican
        RuleFor(x => x.AbiertoPor)
            .GreaterThan(0)
            .When(x => x.AbiertoPor.HasValue)
            .WithMessage("AbiertoPor debe ser mayor a 0 cuando se especifique.");

        RuleFor(x => x.CerradoPor)
            .GreaterThan(0)
            .When(x => x.CerradoPor.HasValue)
            .WithMessage("CerradoPor debe ser mayor a 0 cuando se especifique.");

        // Fechas
        RuleFor(x => x.AbiertoEn)
            .NotEmpty().WithMessage("AbiertoEn es requerido.");

        RuleFor(x => x.CerradoEn)
            .GreaterThanOrEqualTo(x => x.AbiertoEn)
            .When(x => x.CerradoPor.HasValue || x.CerradoEn.HasValue)
            .WithMessage("CerradoEn debe ser mayor o igual que AbiertoEn.");

        // Notas opcional con límite
        RuleFor(x => x.Notas)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Notas))
            .WithMessage("Notas no debe exceder los 500 caracteres.");

        // Tipo y Estado requeridos
        RuleFor(x => x.IdTipoPedido)
            .GreaterThan(0).WithMessage("IdTipoPedido es requerido y debe ser mayor a 0.");

        RuleFor(x => x.IdEstadoPedido)
            .GreaterThan(0).WithMessage("IdEstadoPedido es requerido y debe ser mayor a 0.");

        // Cargo de servicio como porcentaje 0–100
        RuleFor(x => x.CargoServicioPct)
            .GreaterThanOrEqualTo(0).WithMessage("CargoServicioPct no puede ser negativo.")
            .LessThanOrEqualTo(100).WithMessage("CargoServicioPct no puede exceder 100.");
    }
}
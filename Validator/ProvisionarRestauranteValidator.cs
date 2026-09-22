using DTO.Onboarding;
using FluentValidation;

namespace Validator;

public class ProvisionarRestauranteValidator : AbstractValidator<ProvisionarRestauranteRequestDTO>
{
    public ProvisionarRestauranteValidator()
    {
        RuleFor(x => x.DatosEmpresa).NotNull().WithMessage("Los datos de la empresa son requeridos.");
        RuleFor(x => x.DatosEmpresa.Nombre)
            .NotEmpty().WithMessage("El nombre del restaurante es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres.");

        RuleFor(x => x.DatosEmpresa.NombreSucursal)
            .NotEmpty().WithMessage("El nombre de la sucursal es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre de sucursal no puede superar 100 caracteres.");

        RuleFor(x => x.AreasYMesas)
            .NotEmpty().WithMessage("Debes configurar al menos una zona o área en el restaurante.");

        RuleForEach(x => x.AreasYMesas).ChildRules(area =>
        {
            area.RuleFor(a => a.NombreArea).NotEmpty().WithMessage("El nombre del área es requerido.");
            area.RuleFor(a => a.CantidadMesas).GreaterThan(0).WithMessage("Cada área debe tener al menos 1 mesa.");
            area.RuleFor(a => a.AsientosPorMesa).GreaterThan(0).WithMessage("Los asientos por mesa deben ser mayor a cero.");
        });

        RuleForEach(x => x.Menu.Productos).ChildRules(prod =>
        {
            prod.RuleFor(p => p.Nombre).NotEmpty().WithMessage("El nombre del producto no puede estar vacío.");
            prod.RuleFor(p => p.Precio).GreaterThanOrEqualTo(0).WithMessage("El precio no puede ser negativo.");
        });

        RuleForEach(x => x.Personal).ChildRules(p =>
        {
            p.RuleFor(u => u.NombreCompleto).NotEmpty().WithMessage("El nombre del empleado es obligatorio.");
            p.RuleFor(u => u.Pin)
                .NotEmpty().WithMessage("El PIN es obligatorio.")
                .Matches("^[0-9]{4}$").WithMessage("El PIN debe constar de exactamente 4 dígitos numéricos.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.PinSupervisorAdmin), () =>
        {
            RuleFor(x => x.PinSupervisorAdmin)
                .Matches("^[0-9]{4}$").WithMessage("El PIN de supervisor debe constar de exactamente 4 dígitos numéricos.");
        });

        When(x => x.AbrirTurnoInicial, () =>
        {
            RuleFor(x => x.FondoCajaInicial)
                .GreaterThanOrEqualTo(0).WithMessage("El fondo de caja inicial no puede ser negativo.");
        });
    }
}

using DTO.GrupoModificador;
using FluentValidation;

namespace Validator;

public class GrupoModificadorDTOValidator : AbstractValidator<GrupoModificadorDTO>
{
    public GrupoModificadorDTOValidator()
    {
        // FK obligatoria
        RuleFor(x => x.IdProducto)
            .GreaterThan(0)
            .WithMessage("El campo IdProducto es requerido y debe ser mayor a 0.");

        // Nombre obligatorio, con límite
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(100).WithMessage("El campo Nombre no debe exceder los 100 caracteres.");

        // Selección mínima no negativa
        RuleFor(x => x.MinSeleccion)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El campo MinSeleccion no puede ser negativo.");

        // Selección máxima mayor o igual a la mínima
        RuleFor(x => x.MaxSeleccion)
            .GreaterThanOrEqualTo(x => x.MinSeleccion)
            .WithMessage("El campo MaxSeleccion debe ser mayor o igual que MinSeleccion.");

        // Consistencia: si es obligatorio, al menos una selección mínima
        RuleFor(x => x.MinSeleccion)
            .GreaterThan(0)
            .When(x => x.Obligatorio)
            .WithMessage("Si el grupo es obligatorio, MinSeleccion debe ser mayor a 0.");
    }
}
using DTO.OpcionModificador;
using FluentValidation;

namespace Validator;

public class OpcionModificadorDTOValidator : AbstractValidator<OpcionModificadorDTO>
{
    public OpcionModificadorDTOValidator()
    {
        RuleFor(x => x.IdGrupo)
            .GreaterThan(0)
            .WithMessage("El campo IdGrupo es requerido y debe ser mayor a 0.");

        // Nombre obligatorio y con límite
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(100).WithMessage("El campo Nombre no debe exceder los 100 caracteres.");

        // PrecioExtra no negativo
        RuleFor(x => x.PrecioExtra)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El campo PrecioExtra no puede ser negativo.");
    }
}
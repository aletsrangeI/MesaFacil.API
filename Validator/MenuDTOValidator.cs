using DTO.Menu;
using FluentValidation;

namespace Validator;

public class MenuDTOValidator : AbstractValidator<MenuDTO>
{
    public MenuDTOValidator()
    {
        RuleFor(x => x.IdSucursal)
            .GreaterThan(0)
            .WithMessage("El campo IdSucursal es requerido y debe ser mayor a 0.");

        // Nombre requerido, con límite de caracteres
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(100).WithMessage("El campo Nombre no debe exceder los 100 caracteres.");
    }
}
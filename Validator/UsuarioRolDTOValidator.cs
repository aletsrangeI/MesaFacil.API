using DTO.UsuarioRol;
using FluentValidation;

namespace Validator;

public class UsuarioRolDTOValidator : AbstractValidator<UsuarioRolDTO>
{
    public UsuarioRolDTOValidator()
    {
// Usuario requerido
        RuleFor(x => x.IdUsuario)
            .GreaterThan(0)
            .WithMessage("El campo IdUsuario es requerido y debe ser mayor a 0.");

        // Rol requerido
        RuleFor(x => x.IdRol)
            .GreaterThan(0)
            .WithMessage("El campo IdRol es requerido y debe ser mayor a 0.");
    }
}
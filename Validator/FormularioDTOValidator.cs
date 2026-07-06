using DTO.Formulario;
using FluentValidation;

namespace Validator;

public class FormularioDTOValidator : AbstractValidator<FormularioDTO>
{
    public FormularioDTOValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty();
    }
}
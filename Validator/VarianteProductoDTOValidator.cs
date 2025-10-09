using DTO.VarianteProducto;
using FluentValidation;

namespace Validator;

public class VarianteProductoDTOValidator : AbstractValidator<VarianteProductoDTO>
{
    public VarianteProductoDTOValidator()
    {
        RuleFor(x => x.IdProducto)
            .GreaterThan(0)
            .WithMessage("El campo IdProducto es requerido y debe ser mayor a 0.");

        // Nombre requerido, con límite
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El campo Nombre es requerido.")
            .MaximumLength(150).WithMessage("El campo Nombre no debe exceder los 150 caracteres.");

        // Código opcional, con validación de formato y límite
        RuleFor(x => x.Codigo)
            .MaximumLength(30).WithMessage("El campo Codigo no debe exceder los 30 caracteres.")
            .Matches(@"^[A-Za-z0-9\-\._]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Codigo))
            .WithMessage("El campo Codigo solo puede contener letras, números y los caracteres - . _");
    }
}
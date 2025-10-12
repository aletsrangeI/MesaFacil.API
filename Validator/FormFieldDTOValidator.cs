using System.Text.RegularExpressions;
using DTO.FormField;
using FluentValidation;

namespace Validator;

public class FormFieldDTOValidator : AbstractValidator<FormFieldDTO>
{
    // Ajusta/expande la lista según tu UI kit
    private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "email", "password", "number", "tel",
        "textarea", "date", "datetime", "time",
        "select", "radio", "checkbox", "switch",
        "hidden"
    };

    public FormFieldDTOValidator()
    {
        // Type requerido y dentro del set permitido
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("El campo Type es requerido.")
            .Must(t => TiposPermitidos.Contains(t!))
            .WithMessage("El campo Type no es válido. Usa uno de: " + string.Join(", ", TiposPermitidos))
            .MaximumLength(50).WithMessage("Type no debe exceder 50 caracteres.");

        // Name requerido, patrón tipo identificador y longitud
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El campo Name es requerido.")
            .MaximumLength(100).WithMessage("Name no debe exceder 100 caracteres.")
            .Matches(new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$"))
            .WithMessage("Name solo puede contener letras, números y _, y no puede iniciar con número.");

        // Label / Placeholder / Value (opcionales con límites)
        RuleFor(x => x.Label)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Label))
            .WithMessage("Label no debe exceder 150 caracteres.");

        RuleFor(x => x.Placeholder)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Placeholder))
            .WithMessage("Placeholder no debe exceder 150 caracteres.");

        RuleFor(x => x.Value)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Value))
            .WithMessage("Value no debe exceder 500 caracteres.");

        // Formulario (en tu DTO viene el objeto en lugar del Id): que exista
        RuleFor(x => x.Formulario)
            .NotNull().WithMessage("El campo Formulario es requerido.");

        // CatalogId opcional pero válido si viene
        RuleFor(x => x.CatalogId)
            .GreaterThan(0)
            .When(x => x.CatalogId.HasValue)
            .WithMessage("CatalogId debe ser mayor a 0 cuando se especifique.");

        // Order >= 0
        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order no puede ser negativo.");

        // Si es un campo de selección, debe tener Options o bien estar ligado a un catálogo
        // (permitimos cualquiera de las dos fuentes de datos)
        RuleFor(x => x)
            .Must(x =>
            {
                if (!string.Equals(x.Type, "select", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(x.Type, "radio", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(x.Type, "checkbox", StringComparison.OrdinalIgnoreCase))
                {
                    return true; // no aplica
                }

                bool tieneCatalogo = x.CatalogId.HasValue && x.CatalogId > 0;
                bool tieneOptions = x.Options is { Count: > 0 };
                return tieneCatalogo || tieneOptions;
            })
            .WithMessage("Para Type select/radio/checkbox debes proporcionar Options o un CatalogId válido.");

        // (Opcional) Si es input de email, valida que el Name o Type sugiera email => placeholder/value no necesarios, solo ejemplo
        // if necesitas reglas por tipo específico, puedes agregar bloques When(...) por Type:
        // When(x => string.Equals(x.Type, "email", StringComparison.OrdinalIgnoreCase), () => {
        //     RuleFor(x => x.Value).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Value));
        // });
    }
}
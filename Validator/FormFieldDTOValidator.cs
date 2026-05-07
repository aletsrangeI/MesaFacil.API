using System.Text.RegularExpressions;
using DTO.FormField;
using FluentValidation;

namespace Validator;

public class FormFieldDTOValidator : AbstractValidator<FormFieldDTO>
{
    private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "email", "password", "number", "tel",
        "textarea", "date", "datetime", "time",
        "select", "radio", "checkbox", "switch",
        "hidden"
    };

    public FormFieldDTOValidator()
    {
        // Type: Requerido y dentro del set permitido
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("El campo Type es requerido.")
            .Must(t => TiposPermitidos.Contains(t!))
            .WithMessage("El campo Type no es válido. Usa uno de: " + string.Join(", ", TiposPermitidos))
            .MaximumLength(50).WithMessage("Type no debe exceder 50 caracteres.");

        // Name: Identificador válido para el frontend (ej: nombreAtributo)
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El campo Name es requerido.")
            .MaximumLength(100).WithMessage("Name no debe exceder 100 caracteres.")
            .Matches(new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$"))
            .WithMessage("Name solo puede contener letras, números y '_', y no puede iniciar con número.");

        // Label: Requerido según el nuevo esquema de UI
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("El campo Label es requerido.")
            .MaximumLength(200).WithMessage("Label no debe exceder 200 caracteres.");

        // Placeholder y Value (opcionales)
        RuleFor(x => x.Placeholder)
            .MaximumLength(200).WithMessage("Placeholder no debe exceder 200 caracteres.");

        RuleFor(x => x.Value)
            .MaximumLength(500).WithMessage("Value no debe exceder 500 caracteres.");

        // [CORREGIDO] IdFormulario: Debe ser una referencia válida
        RuleFor(x => x.IdFormulario)
            .GreaterThan(0).WithMessage("Debe especificar un Id de Formulario válido.");

        // [CORREGIDO] DataSource: Opcional, pero con límite de longitud
        RuleFor(x => x.DataSource)
            .MaximumLength(100).WithMessage("DataSource no debe exceder 100 caracteres.");

        // [CORREGIDO] Orden: Alineado con la entidad (no puede ser negativo)
        RuleFor(x => x.Orden)
            .GreaterThanOrEqualTo(0).WithMessage("El orden no puede ser un número negativo.");

        // [CORREGIDO] Lógica para campos de selección (Select, Radio, Checkbox)
        RuleFor(x => x)
            .Must(x =>
            {
                bool esSeleccion = string.Equals(x.Type, "select", StringComparison.OrdinalIgnoreCase) ||
                                   string.Equals(x.Type, "radio", StringComparison.OrdinalIgnoreCase) ||
                                   string.Equals(x.Type, "checkbox", StringComparison.OrdinalIgnoreCase);

                if (!esSeleccion) return true;

                // Debe tener opciones estáticas (JSON) o un origen de datos dinámico (DataSource)
                bool tieneDataSource = !string.IsNullOrWhiteSpace(x.DataSource);
                bool tieneOptions = x.Options is { Count: > 0 };

                return tieneDataSource || tieneOptions;
            })
            .WithMessage("Para campos de selección (select/radio/checkbox) debe definir un DataSource o proporcionar Options.");
    }
}
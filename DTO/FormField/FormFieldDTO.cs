using Domain.Entities;
using System.Collections.Generic;

namespace DTO.FormField;

public class FormFieldDTO
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Placeholder { get; set; }
    public string Label { get; set; } = null!;
    public string? Value { get; set; }

    // [Mantenemos] Listas tipadas para el Frontend
    public List<FormValidation> Validations { get; set; } = new();
    public List<SelectFormOption> Options { get; set; } = new();

    // [CORREGIDO] Referencia simple al Formulario cabecera
    public int IdFormulario { get; set; }
    public string? FormularioNombre { get; set; }

    // [CORREGIDO] Reemplazo de CatalogId por DataSource
    public string? DataSource { get; set; }

    public int Orden { get; set; }
    public bool IsActive { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class FormField : BaseAuditableEntity
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Placeholder { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public List<FormValidation> Validations { get; set; } = new();
    public List<SelectFormOption>? Options { get; set; } = new();


    public int FormularioCatalogId { get; set; }
    public int FormularioItemId { get; set; }
    public CatalogItem Formulario { get; set; } = null!;

    public int? CatalogId { get; set; }
    public Catalog? Catalog { get; set; }

    public int Order { get; set; }
}
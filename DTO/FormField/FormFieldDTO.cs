using Domain.Entities;

namespace DTO.FormField;

public class FormFieldDTO
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Placeholder { get; set; }
    public string Label { get; set; }
    public string Value { get; set; }
    public List<FormValidation> Validations { get; set; } = new();
    public List<SelectFormOption>? Options { get; set; } = new();
    public Domain.Entities.CatalogItem Formulario { get; set; }
    public int? CatalogId { get; set; }
    public Domain.Entities.Catalog Catalog { get; set; }
    public int Order { get; set; }
}

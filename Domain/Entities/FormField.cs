using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class FormField : BaseAuditableEntity // Asume que hereda IdFormField y auditoría
{
    // [CORREGIDO] - Relación directa a la cabecera, fuera catálogos
    public int IdFormulario { get; set; }

    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Placeholder { get; set; }
    public string Label { get; set; } = null!;
    public string? Value { get; set; }

    // ==========================================
    // MANEJO DE JSON (Base de Datos)
    // ==========================================
    public string? ValidationsJson { get; set; }
    public string? OptionsJson { get; set; }

    // ==========================================
    // PROPIEDADES AUXILIARES (Ignoradas en BD, útiles en código)
    // ==========================================
    public List<FormValidation> Validations { get; set; } = new();
    
    public List<SelectFormOption> Options { get; set; } = new();

    // ==========================================
    // EL REEMPLAZO DE CatalogId
    // ==========================================
    public string? DataSource { get; set; }

    public int Orden { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Formulario Formulario { get; set; } = null!;
}
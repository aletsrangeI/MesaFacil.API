using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class FormFieldConfiguration : IEntityTypeConfiguration<FormField>
{
    public void Configure(EntityTypeBuilder<FormField> e)
    {
        e.ToTable("FormField");
        
        // Asumiendo que BaseAuditableEntity tiene la propiedad 'Id'
        e.HasKey(x => x.Id); 

        e.Property(x => x.Type).IsRequired().HasMaxLength(50);
        e.Property(x => x.Name).IsRequired().HasMaxLength(100);
        e.Property(x => x.Label).IsRequired().HasMaxLength(200);
        e.Property(x => x.Placeholder).HasMaxLength(200);
        e.Property(x => x.Orden).HasDefaultValue(0);
        
        // [CORREGIDO] - El reemplazo de CatalogId
        e.Property(x => x.DataSource).HasMaxLength(100);

        // ==========================================
        // [CORREGIDO] - Relación directa con Formulario
        // ==========================================
        e.HasOne(x => x.Formulario)
            .WithMany(f => f.Campos) // Asegúrate de que Formulario tenga ICollection<FormField> Campos
            .HasForeignKey(x => x.IdFormulario)
            .OnDelete(DeleteBehavior.Cascade);

        // Index para búsquedas rápidas por formulario
        e.HasIndex(x => x.IdFormulario)
            .HasDatabaseName("IX_FormField_IdFormulario");

        // Índice único: No puede haber dos campos con el mismo nombre en el mismo formulario
        e.HasIndex(x => new { x.IdFormulario, x.Name })
            .IsUnique()
            .HasDatabaseName("UX_FormField_Formulario_Name");

        // ==========================================
        // CONFIGURACIÓN DE JSON
        // ==========================================
        // Nota: Si prefieres que EF se encargue de la serialización automática 
        // podrías quitar el [NotMapped] de la entidad y usar HasConversion aquí.
        // De lo contrario, configuramos las columnas de texto:
        
        e.Ignore(x => x.ValidationsJson);
        e.Ignore(x => x.OptionsJson);

        e.Property(x => x.Validations)
            .HasColumnName("ValidationsJson")
            .HasColumnType("jsonb")
            .IsRequired(false)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<FormValidation>>(v ?? "[]", (JsonSerializerOptions)null) ?? new List<FormValidation>()
            );

        e.Property(x => x.Options)
            .HasColumnName("OptionsJson")
            .HasColumnType("jsonb")
            .IsRequired(false)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<SelectFormOption>>(v ?? "[]", (JsonSerializerOptions)null) ?? new List<SelectFormOption>()
            );
    }
}
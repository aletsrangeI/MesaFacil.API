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
        e.HasKey(x => x.Id);

        e.Property(x => x.Type).HasConversion<string>(); // guarda como texto
        e.Property(x => x.Type).IsRequired().HasMaxLength(50);
        e.Property(x => x.Name).IsRequired().HasMaxLength(100);
        e.Property(x => x.Label).HasMaxLength(150);
        e.Property(x => x.Placeholder).HasMaxLength(150);
        e.Property(x => x.Value).HasMaxLength(500);
        e.Property(x => x.Order).HasDefaultValue(0);

        // FK compuesta hacia CatalogItem (Formulario)
        e.HasOne(x => x.Formulario)
            .WithMany() // o .WithMany(ci => ci.FormFields) si existe
            .HasForeignKey(x => new { x.FormularioCatalogId, x.FormularioItemId })
            .HasPrincipalKey(nameof(CatalogItem.CatalogId), nameof(CatalogItem.Id))
            .OnDelete(DeleteBehavior.Restrict);

        // Relación opcional a Catalog
        e.HasOne(x => x.Catalog)
            .WithMany()
            .HasForeignKey(x => x.CatalogId)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasIndex(x => new { x.FormularioCatalogId, x.FormularioItemId })
            .HasDatabaseName("IX_FormField_Formulario");

        e.HasIndex(x => x.CatalogId)
            .HasDatabaseName("IX_FormField_CatalogId");

        e.HasIndex(x => new { x.FormularioCatalogId, x.FormularioItemId, x.Name })
            .IsUnique()
            .HasDatabaseName("UX_FormField_Formulario_Name");

        
        e.Property(x => x.Validations)
            .HasColumnType("jsonb")
            .HasColumnName("ValidationsJson")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<FormValidation>>(v, (JsonSerializerOptions?)null) ?? new()
            );

        e.Property(x => x.Options)
            .HasColumnType("jsonb")
            .HasColumnName("OptionsJson")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<SelectFormOption>>(v, (JsonSerializerOptions?)null) ?? new()
            );
    }
}

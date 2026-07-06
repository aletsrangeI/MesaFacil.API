using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class FormularioConfiguration : IEntityTypeConfiguration<Formulario>
{
    public void Configure(EntityTypeBuilder<Formulario> builder)
    {
        builder.ToTable("Formulario");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Codigo)
            .IsRequired()
            .HasMaxLength(100); // Sincronizado con DBML

        builder.HasIndex(x => x.Codigo)
            .IsUnique();

        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(200); // Sincronizado con DBML

        builder.Property(x => x.Descripcion)
            .HasMaxLength(400);

        // ==========================================
        // [CORREGIDO] - Configuración de Relaciones
        // ==========================================
        builder.HasMany(x => x.Campos)
            .WithOne(f => f.Formulario)
            .HasForeignKey(f => f.IdFormulario) // <--- Apuntando a la propiedad real en FormField
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
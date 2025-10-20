using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AccesoRutaConfiguration : IEntityTypeConfiguration<AccesoRuta>
{
    public void Configure(EntityTypeBuilder<AccesoRuta> e)
    {
        e.ToTable("AccesoRuta");
        e.HasKey(x => x.Id);

        e.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        e.Property(x => x.Path)
            .IsRequired()
            .HasMaxLength(200);

        e.Property(x => x.Descripcion)
            .HasMaxLength(300);

        // NUEVO: metadata para permisos/menú
        e.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(120);

        e.Property(x => x.Group)
            .HasMaxLength(60);

        e.Property(x => x.IsMenu)
            .HasDefaultValue(false);

        // Índices (evitar duplicados de Path y Key)
        e.HasIndex(x => x.Key)
            .IsUnique()
            .HasDatabaseName("UX_AccesoRuta_Key");

        e.HasIndex(x => x.Path)
            .IsUnique()
            .HasDatabaseName("UX_AccesoRuta_Path");
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> e)
    {
        e.ToTable("Rol");
        e.HasKey(x => x.Id);

        e.Property(x => x.Nombre)
            .HasMaxLength(100)
            .IsRequired();
        e.HasIndex(x => x.Nombre).IsUnique();

        // Defaults a nivel de base de datos — protegen contra INSERTs directos sin valor
        e.Property(x => x.IsSystem).HasDefaultValue(false);
        e.Property(x => x.IsAssignable).HasDefaultValue(true);
        e.Property<bool>("IsActive").HasDefaultValue(true);

        // ConcurrencyStamp: campo de rastreo, siempre generado en la capa de aplicación.
        // NO se usa como concurrency token de EF porque el repositorio trabaja en modo
        // desconectado (entidad construida desde DTO, sin valor original disponible).
        e.Property(x => x.ConcurrencyStamp)
            .HasMaxLength(64)
            .IsRequired();

        e.HasMany(x => x.AccesosRuta)
            .WithOne(x => x.Rol)
            .HasForeignKey(x => x.IdRol)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

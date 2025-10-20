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

        e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        e.HasIndex(x => x.Nombre).IsUnique();

        e.Property(x => x.IsSystem).HasDefaultValue(false);
        e.Property(x => x.IsAssignable).HasDefaultValue(true);

        e.Property(x => x.ConcurrencyStamp)
            .HasMaxLength(64)
            .IsConcurrencyToken(); // opcional pero útil

        e.HasMany(x => x.AccesosRuta)
            .WithOne(x => x.Rol)
            .HasForeignKey(x => x.IdRol)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

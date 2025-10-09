using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class GrupoModificadorConfiguration : IEntityTypeConfiguration<GrupoModificador>
{
    public void Configure(EntityTypeBuilder<GrupoModificador> e)
    {
        e.ToTable("GrupoModificador");
        e.HasKey(x => x.Id);
        e.Property(x => x.Nombre).HasMaxLength(150);
        e.Property(x => x.MinSeleccion).HasDefaultValue(0);
        e.Property(x => x.MaxSeleccion).HasDefaultValue(1);
        e.Property(x => x.Obligatorio).HasDefaultValue(false);

        e.HasOne(x => x.Producto)
            .WithMany(x => x.GruposModificador)
            .HasForeignKey(x => x.IdProducto)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
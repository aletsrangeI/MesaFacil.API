using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> e)
    {
        e.ToTable("Producto");
        e.HasKey(x => x.Id);
        e.Property(x => x.Codigo).HasMaxLength(64);
        e.Property(x => x.Nombre).HasMaxLength(200);
        e.Property(x => x.Descripcion).HasMaxLength(400);
        e.Property(x => x.Activo).HasDefaultValue(true);

        e.HasOne(x => x.Menu)
            .WithMany(x => x.Productos)
            .HasForeignKey(x => x.IdMenu)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Categoria)
            .WithMany(x => x.Productos)
            .HasForeignKey(x => x.IdCategoria)
            .OnDelete(DeleteBehavior.Restrict);

        // Estación cocina (nullable)
        e.HasOne(x => x.EstacionItem)
            .WithMany()
            .HasForeignKey(x => new { x.EstacionCatalogId, x.EstacionItemId })
            .OnDelete(DeleteBehavior.SetNull);
    }
}

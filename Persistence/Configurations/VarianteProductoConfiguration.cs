using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class VarianteProductoConfiguration : IEntityTypeConfiguration<VarianteProducto>
{
    public void Configure(EntityTypeBuilder<VarianteProducto> e)
    {
        e.ToTable("VarianteProducto");
        e.HasKey(x => x.Id);
        e.Property(x => x.Nombre).HasMaxLength(120);
        e.Property(x => x.Codigo).HasMaxLength(64);
        e.Property(x => x.EsDefault).HasDefaultValue(false);
        e.HasOne(x => x.Producto)
            .WithMany(x => x.Variantes)
            .HasForeignKey(x => x.IdProducto)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

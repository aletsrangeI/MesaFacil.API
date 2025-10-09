using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MesaConfiguration : IEntityTypeConfiguration<Mesa>
{
    public void Configure(EntityTypeBuilder<Mesa> e)
    {
        e.ToTable("Mesa");
        e.HasKey(x => x.Id);
        e.Property(x => x.Codigo).HasMaxLength(32).IsRequired();
        e.Property(x => x.Asientos).HasDefaultValue(2);
        e.HasIndex(x => new { x.IdSucursal, x.Codigo }).IsUnique();

        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.Mesas)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Area)
            .WithMany(x => x.Mesas)
            .HasForeignKey(x => x.IdArea)
            .OnDelete(DeleteBehavior.SetNull);

        // Estado: FK compuesta a CatalogItem (EstadoCatalogId, EstadoItemId)
        e.HasOne(x => x.EstadoItem)
            .WithMany()
            .HasForeignKey(x => new { x.EstadoCatalogId, x.EstadoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

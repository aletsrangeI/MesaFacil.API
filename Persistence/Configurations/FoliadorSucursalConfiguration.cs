using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class FoliadorSucursalConfiguration : IEntityTypeConfiguration<FoliadorSucursal>
{
    public void Configure(EntityTypeBuilder<FoliadorSucursal> e)
    {
        e.ToTable("FoliadorSucursal");
        e.HasKey(x => x.Id);

        e.Property(x => x.UltimoFolio).HasDefaultValue(0);

        e.HasIndex(x => new { x.IdSucursal, x.Fecha }).IsUnique();

        e.HasOne(x => x.Sucursal)
            .WithMany()
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

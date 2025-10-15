using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable("Areas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre).HasMaxLength(120);
        builder.Property(x => x.Orden).HasDefaultValue(0);
        builder.HasOne(x => x.Sucursal)
            .WithMany(x => x.Areas)
            .HasForeignKey(x => x.IdSucursal);
    }
}
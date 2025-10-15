using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> e)
    {
        e.ToTable("Menu");
        e.HasKey(x => x.Id);
        e.Property(x => x.Nombre).HasMaxLength(200);
        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.Menus)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
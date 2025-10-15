using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CategoriaMenuConfiguration : IEntityTypeConfiguration<CategoriaMenu>
{
    public void Configure(EntityTypeBuilder<CategoriaMenu> builder)
    {
        builder.ToTable("CategoriaMenus");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre).HasMaxLength(200);
        builder.Property(x => x.Orden).HasDefaultValue(0);
        builder.HasOne(x => x.Menu)
            .WithMany(x => x.Categorias)
            .HasForeignKey(x => x.IdMenu)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatTipoDescuentoConfiguration : IEntityTypeConfiguration<CatTipoDescuento>
{
    public void Configure(EntityTypeBuilder<CatTipoDescuento> builder)
    {
        builder.ToTable("CatTipoDescuento");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

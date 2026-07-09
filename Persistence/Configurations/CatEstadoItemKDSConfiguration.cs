using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatEstadoItemKDSConfiguration : IEntityTypeConfiguration<CatEstadoItemKDS>
{
    public void Configure(EntityTypeBuilder<CatEstadoItemKDS> builder)
    {
        builder.ToTable("CatEstadoItemKDS");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

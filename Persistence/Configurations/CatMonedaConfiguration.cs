using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatMonedaConfiguration : IEntityTypeConfiguration<CatMoneda>
{
    public void Configure(EntityTypeBuilder<CatMoneda> builder)
    {
        builder.ToTable("CatMoneda");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

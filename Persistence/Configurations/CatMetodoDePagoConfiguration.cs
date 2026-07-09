using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatMetodoDePagoConfiguration : IEntityTypeConfiguration<CatMetodoDePago>
{
    public void Configure(EntityTypeBuilder<CatMetodoDePago> builder)
    {
        builder.ToTable("CatMetodoDePago");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

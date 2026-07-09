using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatImpuestoConfiguration : IEntityTypeConfiguration<CatImpuesto>
{
    public void Configure(EntityTypeBuilder<CatImpuesto> builder)
    {
        builder.ToTable("CatImpuesto");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

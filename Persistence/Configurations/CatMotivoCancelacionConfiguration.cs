using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatMotivoCancelacionConfiguration : IEntityTypeConfiguration<CatMotivoCancelacion>
{
    public void Configure(EntityTypeBuilder<CatMotivoCancelacion> builder)
    {
        builder.ToTable("CatMotivoCancelacion");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(150);
    }
}

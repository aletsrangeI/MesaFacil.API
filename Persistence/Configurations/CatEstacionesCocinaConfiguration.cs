using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatEstacionesCocinaConfiguration : IEntityTypeConfiguration<CatEstacionesCocina>
{
    public void Configure(EntityTypeBuilder<CatEstacionesCocina> builder)
    {
        builder.ToTable("CatEstacionesCocina");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatEstadoMesaConfiguration : IEntityTypeConfiguration<CatEstadoMesa>
{
    public void Configure(EntityTypeBuilder<CatEstadoMesa> builder)
    {
        builder.ToTable("CatEstadoMesa");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

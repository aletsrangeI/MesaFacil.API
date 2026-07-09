using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatEstadoCuentaConfiguration : IEntityTypeConfiguration<CatEstadoCuenta>
{
    public void Configure(EntityTypeBuilder<CatEstadoCuenta> builder)
    {
        builder.ToTable("CatEstadoCuenta");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatCredencialConfiguration : IEntityTypeConfiguration<CatCredencial>
{
    public void Configure(EntityTypeBuilder<CatCredencial> builder)
    {
        builder.ToTable("CatCredencial");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(100);
    }
}
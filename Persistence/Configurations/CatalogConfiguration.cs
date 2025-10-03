using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
{
    public void Configure(EntityTypeBuilder<Catalog> builder)
    {
        builder.ToTable("Catalogs");
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Code).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Catalog)
            .HasForeignKey(x => x.CatalogId);

        builder.Property(x => x.CreatedBy).HasMaxLength(128);
        builder.Property(x => x.UpdatedBy).HasMaxLength(128);
    }
}
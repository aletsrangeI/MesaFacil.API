using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> e)
    {
        e.ToTable("CatalogItem");
        e.HasKey(x => new { x.CatalogId, x.Id });
        e.Property(x => x.Id); // Identity en DB
        e.Property(x => x.Code).HasMaxLength(64).IsRequired();
        e.Property(x => x.Name).HasMaxLength(256).IsRequired();
        e.Property(x => x.SortOrder).HasDefaultValue(0);
        e.Property(x => x.IsActive).HasDefaultValue(true);
        e.Property(x => x.ValidFrom).HasColumnType("date");
        e.Property(x => x.ValidTo).HasColumnType("date");
        e.HasIndex(x => new { x.CatalogId, x.Code }).IsUnique();
        e.HasIndex(x => new { x.CatalogId, x.IsActive, x.SortOrder });
        e.HasOne(x => x.Catalog)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
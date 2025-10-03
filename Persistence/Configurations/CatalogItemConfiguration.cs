using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("CatalogItems");

        builder.HasKey(x => x.Id);

        // Propiedades
        builder.Property<int>(x => x.CatalogId);

        builder.Property<string>(x => x.Code)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property<string>(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property<int>(x => x.SortOrder)
            .HasDefaultValue(0);

        builder.Property<DateTime?>(x => x.ValidFrom);
        builder.Property<DateTime?>(x => x.ValidTo);

        // jsonb en PostgreSQL (si usas Npgsql)
        builder.Property<string?>(x => x.ExtraJson)
            .HasColumnType("jsonb"); // quita esta línea si NO usas Postgres

        // Índices/Únicos
        builder.HasIndex(x => new { x.CatalogId, x.Code })
            .IsUnique();

        // Opcional: evita nombres duplicados dentro del mismo catálogo
        builder.HasIndex(x => new { x.CatalogId, x.Name })
            .IsUnique();

        // Relación
        builder.HasOne(x => x.Catalog)
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.CatalogId)
            .OnDelete(DeleteBehavior.Cascade);

        // Check de vigencia
        builder.HasCheckConstraint("CK_CatalogItems_ValidRange",
            "(\"ValidFrom\" IS NULL OR \"ValidTo\" IS NULL OR \"ValidFrom\" <= \"ValidTo\")");

        // Auditoría heredada (si quieres límites)
        builder.Property<string>(x => x.CreatedBy).HasMaxLength(128);
        builder.Property<string?>(x => x.UpdatedBy).HasMaxLength(128);
    }
}
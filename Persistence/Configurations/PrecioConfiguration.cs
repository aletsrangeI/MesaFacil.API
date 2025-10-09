using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PrecioConfiguration : IEntityTypeConfiguration<Precio>
{
    public void Configure(EntityTypeBuilder<Precio> e)
    {
        e.ToTable("Precio");
        e.HasKey(x => x.Id);
        e.Property(x => x.Monto).HasColumnType("numeric(12,2)").IsRequired();
        e.Property(x => x.Moneda).HasMaxLength(8).HasDefaultValue("MXN");
        e.Property(x => x.ValidoDesde).HasColumnType("date");
        e.Property(x => x.ValidoHasta).HasColumnType("date");
        e.Property(x => x.Dias).HasMaxLength(14);
        e.Property(x => x.Horario).HasMaxLength(32);

        e.HasOne(x => x.Variante)
            .WithMany(x => x.Precios)
            .HasForeignKey(x => x.IdVariante)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.ImpuestoItem)
            .WithMany()
            .HasForeignKey(x => new { x.ImpuestoCatalogId, x.ImpuestoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
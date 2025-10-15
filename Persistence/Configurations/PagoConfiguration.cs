using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> e)
    {
        const string tsTz = "timestamptz";
        e.ToTable("Pago");
        e.HasKey(x => x.Id);
        e.Property(x => x.Monto).HasColumnType("numeric(12,2)").IsRequired();
        e.Property(x => x.Moneda).HasMaxLength(8).HasDefaultValue("MXN");
        e.Property(x => x.Propina).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.PagadoEn).HasColumnType(tsTz).HasDefaultValueSql("now()");
        e.Property(x => x.Referencia).HasMaxLength(100);

        e.HasOne(x => x.Cuenta)
            .WithMany(x => x.Pagos)
            .HasForeignKey(x => x.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.RecibidoPorUsuario)
            .WithMany(x => x.PagosRecibidos)
            .HasForeignKey(x => x.RecibidoPor)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.MetodoItem)
            .WithMany()
            .HasForeignKey(x => new { x.MetodoCatalogId, x.MetodoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

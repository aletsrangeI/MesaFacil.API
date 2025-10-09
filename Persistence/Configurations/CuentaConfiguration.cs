using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
{
    public void Configure(EntityTypeBuilder<Cuenta> e)
    {
        e.ToTable("Cuenta");
        e.HasKey(x => x.Id);
        e.Property(x => x.Subtotal).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.DescuentoTotal).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.CargoServicio).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.ImpuestoTotal).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.Total).HasColumnType("numeric(12,2)").HasDefaultValue(0m);

        e.HasOne(x => x.Pedido)
            .WithMany(x => x.Cuentas)
            .HasForeignKey(x => x.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.EstadoItem)
            .WithMany()
            .HasForeignKey(x => new { x.EstadoCatalogId, x.EstadoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
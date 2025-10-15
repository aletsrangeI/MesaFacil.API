using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PedidoDetalleConfiguration : IEntityTypeConfiguration<PedidoDetalle>
{
    public void Configure(EntityTypeBuilder<PedidoDetalle> e)
    {
        e.ToTable("PedidoDetalle");
        e.HasKey(x => x.Id);
        e.Property(x => x.Cantidad).HasColumnType("numeric(9,2)").HasDefaultValue(1m);
        e.Property(x => x.PrecioUnitario).HasColumnType("numeric(12,2)").IsRequired();
        e.Property(x => x.Notas).HasMaxLength(300);

        e.HasOne(x => x.Pedido)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Asiento)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.IdAsiento)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.Producto)
            .WithMany(x => x.PedidoDetalles)
            .HasForeignKey(x => x.IdProducto)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Variante)
            .WithMany(x => x.PedidoDetalles)
            .HasForeignKey(x => x.IdVariante)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.EstadoItem)
            .WithMany()
            .HasForeignKey(x => new { x.EstadoCatalogId, x.EstadoItemId })
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.ImpuestoItem)
            .WithMany()
            .HasForeignKey(x => new { x.ImpuestoCatalogId, x.ImpuestoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
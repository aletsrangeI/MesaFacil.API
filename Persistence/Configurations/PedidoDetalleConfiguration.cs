using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PedidoDetalleConfiguration : IEntityTypeConfiguration<PedidoDetalle>
{
    public void Configure(EntityTypeBuilder<PedidoDetalle> e)
    {
        e.ToTable("PedidoDetalle");
        
        // Asumiendo que BaseAuditableEntity provee el Id
        e.HasKey(x => x.Id);

        // ==========================================
        // SNAPSHOTS E INMUTABILIDAD
        // ==========================================
        e.Property(x => x.ProductoNombre)
            .IsRequired()
            .HasMaxLength(200);

        e.Property(x => x.VarianteNombre)
            .IsRequired()
            .HasMaxLength(120);

        e.Property(x => x.Cantidad)
            .HasColumnType("decimal(9,2)")
            .HasDefaultValue(1m);

        e.Property(x => x.PrecioUnitario)
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        // ==========================================
        // IMPUESTOS CONGELADOS
        // ==========================================
        e.Property(x => x.TasaImpuesto)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        e.Property(x => x.MontoImpuesto)
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        e.Property(x => x.Notas)
            .HasMaxLength(300);

        // ==========================================
        // MANEJO DE CANCELACIONES
        // ==========================================
        e.Property(x => x.Cancelado)
            .HasDefaultValue(false);

        e.Property(x => x.MotivoCancelacion)
            .HasMaxLength(200);

        // ==========================================
        // RELACIONES
        // ==========================================
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

        // [CORREGIDO] IdVariante ahora es NOT NULL según la regla de negocio
        e.HasOne(x => x.Variante)
            .WithMany(x => x.PedidoDetalles)
            .HasForeignKey(x => x.IdVariante)
            .OnDelete(DeleteBehavior.Restrict);

        // [CORREGIDO] Relaciones con Catálogos Tipados
        e.HasOne(x => x.EstadoPedidoDetalle)
            .WithMany()
            .HasForeignKey(x => x.IdEstadoPedidoDetalle)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Impuesto)
            .WithMany()
            .HasForeignKey(x => x.IdImpuesto)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.UsuarioCancela)
            .WithMany()
            .HasForeignKey(x => x.CanceladoPor)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
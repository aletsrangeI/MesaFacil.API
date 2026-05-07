using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PrecioConfiguration : IEntityTypeConfiguration<Precio>
{
    public void Configure(EntityTypeBuilder<Precio> e)
    {
        e.ToTable("Precio");
        
        // Asumiendo que BaseAuditableEntity provee el Id (IdPrecio)
        e.HasKey(x => x.Id);

        e.Property(x => x.Monto)
            .HasColumnType("decimal(12,2)")
            .IsRequired();

        // Se mantiene como valor rápido, pero ahora tenemos relación con CatMoneda
        e.Property(x => x.Moneda)
            .HasMaxLength(8)
            .HasDefaultValue("MXN");

        e.Property(x => x.ValidoDesde)
            .HasColumnType("date");

        e.Property(x => x.ValidoHasta)
            .HasColumnType("date");

        e.Property(x => x.Dias)
            .HasMaxLength(14)
            .HasComment("Ej: L-M-M-J-V-S-D");

        e.Property(x => x.Horario)
            .HasMaxLength(32)
            .HasComment("Ej: 12:00-16:00");

        // Relación con la Variante del Producto
        e.HasOne(x => x.Variante)
            .WithMany(x => x.Precios)
            .HasForeignKey(x => x.IdVariante)
            .OnDelete(DeleteBehavior.Cascade);

        // ==========================================
        // [CORREGIDO] - Relaciones con Catálogos Tipados
        // ==========================================
        
        // Relación directa con Impuestos
        e.HasOne(x => x.Impuesto)
            .WithMany()
            .HasForeignKey(x => x.IdImpuesto)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación directa con Monedas
        e.HasOne(x => x.CatMoneda)
            .WithMany()
            .HasForeignKey(x => x.IdMoneda)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
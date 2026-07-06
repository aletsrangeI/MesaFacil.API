using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> e)
    {
        e.ToTable("Pago");
        e.HasKey(x => x.Id);
        
        e.Property(x => x.Monto)
            .HasColumnType("numeric(12,2)")
            .IsRequired();
            
        e.Property(x => x.Moneda)
            .HasMaxLength(8)
            .HasDefaultValue("MXN");
            
        e.Property(x => x.Propina)
            .HasColumnType("numeric(12,2)")
            .HasDefaultValue(0m);
            
        e.Property(x => x.PagadoEn)
            .HasDefaultValueSql("now()"); // Cambiar 'sysutcdatetime()' por 'now()'
            
        e.Property(x => x.Referencia)
            .HasMaxLength(100);

        // Relación con la Cuenta
        e.HasOne(x => x.Cuenta)
            .WithMany(x => x.Pagos)
            .HasForeignKey(x => x.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con el Usuario (Cajero/Mesero que recibe el pago)
        e.HasOne(x => x.RecibidoPorUsuario)
            .WithMany() // Ajustar si el Usuario requiere la colección PagosRecibidos
            .HasForeignKey(x => x.RecibidoPor)
            .OnDelete(DeleteBehavior.SetNull);

        // [CORREGIDO] - Relación directa con el catálogo de métodos de pago
        e.HasOne(x => x.MetodoDePago)
            .WithMany()
            .HasForeignKey(x => x.IdMetodoDePago)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
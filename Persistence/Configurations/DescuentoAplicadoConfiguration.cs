using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class DescuentoAplicadoConfiguration : IEntityTypeConfiguration<DescuentoAplicado>
{
    public void Configure(EntityTypeBuilder<DescuentoAplicado> e)
    {
        e.ToTable("DescuentoAplicado");
        
        e.HasKey(x => x.Id);
        
        e.Property(x => x.Valor)
            .HasColumnType("numeric(12,4)")
            .IsRequired();
            
        e.Property(x => x.Alcance)
            .HasMaxLength(32);
            
        e.Property(x => x.Condiciones); // Por defecto nvarchar(max) en SQL Server

        // Relación con la Cuenta
        e.HasOne(x => x.Cuenta)
            .WithMany(x => x.Descuentos)
            .HasForeignKey(x => x.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);

        // [CORREGIDO] - Relación directa con el catálogo tipado
        e.HasOne(x => x.TipoDescuento)
            .WithMany()
            .HasForeignKey(x => x.IdTipoDescuento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
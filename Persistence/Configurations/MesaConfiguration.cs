using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MesaConfiguration : IEntityTypeConfiguration<Mesa>
{
    public void Configure(EntityTypeBuilder<Mesa> e)
    {
        e.ToTable("Mesa");
        e.HasKey(x => x.Id);
        e.Property(x => x.Codigo).HasMaxLength(32).IsRequired();
        e.Property(x => x.Asientos).HasDefaultValue(2);
        
        // Mantenemos la restricción de que el código sea único por sucursal
        e.HasIndex(x => new { x.IdSucursal, x.Codigo }).IsUnique();

        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.Mesas)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Area)
            .WithMany(x => x.Mesas)
            .HasForeignKey(x => x.IdArea)
            .OnDelete(DeleteBehavior.SetNull);

        // [CORREGIDO] - Relación directa con el catálogo de estados de mesa
        e.HasOne(x => x.EstadoMesa)
            .WithMany()
            .HasForeignKey(x => x.IdEstadoMesa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
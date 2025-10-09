using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class DetalleCuentaConfiguration : IEntityTypeConfiguration<DetalleCuenta>
{
    public void Configure(EntityTypeBuilder<DetalleCuenta> e)
    {
        e.ToTable("DetalleCuenta");
        e.HasKey(x => x.Id);
        e.Property(x => x.TipoOrigen).HasMaxLength(32).IsRequired();
        e.Property(x => x.Descripcion).HasMaxLength(300);
        e.Property(x => x.Monto).HasColumnType("numeric(12,2)").IsRequired();

        e.HasOne(x => x.Cuenta)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
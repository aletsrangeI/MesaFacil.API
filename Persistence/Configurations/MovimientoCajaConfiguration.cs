using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MovimientoCajaConfiguration : IEntityTypeConfiguration<MovimientoCaja>
{
    public void Configure(EntityTypeBuilder<MovimientoCaja> e)
    {
        e.ToTable("MovimientoCaja");
        e.HasKey(x => x.Id);
        e.Property(x => x.Tipo).HasMaxLength(32).IsRequired();
        e.Property(x => x.Monto).HasColumnType("numeric(12,2)").IsRequired();
        e.Property(x => x.Nota).HasMaxLength(300);

        e.HasOne(x => x.Turno)
            .WithMany(x => x.MovimientosCaja)
            .HasForeignKey(x => x.IdTurno)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TurnoConfiguration : IEntityTypeConfiguration<Turno>
{
    public void Configure(EntityTypeBuilder<Turno> e)
    {
        const string tsTz = "timestamptz";
        
        e.ToTable("Turno");
        e.HasKey(x => x.Id);
        e.Property(x => x.Apertura).HasColumnType(tsTz).IsRequired();
        e.Property(x => x.Cierre).HasColumnType(tsTz);
        e.Property(x => x.CajaInicial).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.CajaFinal).HasColumnType("numeric(12,2)");

        e.HasOne(x => x.Usuario)
            .WithMany(x => x.Turnos)
            .HasForeignKey(x => x.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.Turnos)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

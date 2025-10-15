using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CorteCajaConfiguration : IEntityTypeConfiguration<CorteCaja>
{
    public void Configure(EntityTypeBuilder<CorteCaja> e)
    {
        const string tsTz = "timestamptz";

        e.ToTable("CorteCaja");
        e.HasKey(x => x.Id);
        e.Property(x => x.FechaInicio).HasColumnType(tsTz).IsRequired();
        e.Property(x => x.FechaFin).HasColumnType(tsTz).IsRequired();

        e.Property(x => x.TotalVentas).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.TotalPagos).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.TotalEfectivo).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.TotalTarjeta).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.TotalEgresos).HasColumnType("numeric(12,2)").HasDefaultValue(0m);

        e.Property(x => x.CajaEsperada).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.Declarado).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.Diferencia).HasColumnType("numeric(12,2)").HasDefaultValue(0m);

        e.Property(x => x.CreadoEn).HasColumnType(tsTz).HasDefaultValueSql("now()");

        e.HasOne(x => x.Turno)
            .WithMany(x => x.CortesCaja)
            .HasForeignKey(x => x.IdTurno)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.CortesCaja)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.CreadoPorUsuario)
            .WithMany(x => x.CortesCajaCreados)
            .HasForeignKey(x => x.CreadoPor)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasIndex(x => x.IdTurno);
        e.HasIndex(x => new { x.IdSucursal, x.FechaInicio, x.FechaFin });
    }
}
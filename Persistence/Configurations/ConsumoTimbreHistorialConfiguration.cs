using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ConsumoTimbreHistorialConfiguration : IEntityTypeConfiguration<ConsumoTimbreHistorial>
{
    public void Configure(EntityTypeBuilder<ConsumoTimbreHistorial> builder)
    {
        builder.ToTable("ConsumosTimbreHistorial");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.IdEmpresa, x.FechaMovimiento });

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BolsaTimbres)
            .WithMany(x => x.Historial)
            .HasForeignKey(x => x.IdEmpresaBolsaTimbres)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.FacturaVenta)
            .WithMany()
            .HasForeignKey(x => x.IdFacturaVenta)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

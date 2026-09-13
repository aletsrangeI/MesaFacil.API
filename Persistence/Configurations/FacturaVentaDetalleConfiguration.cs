using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class FacturaVentaDetalleConfiguration : IEntityTypeConfiguration<FacturaVentaDetalle>
{
    public void Configure(EntityTypeBuilder<FacturaVentaDetalle> builder)
    {
        builder.ToTable("FacturaVentaDetalles");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.IdFacturaVenta);
        builder.HasIndex(x => x.PedidoDetalleId);
    }
}

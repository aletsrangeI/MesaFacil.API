using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatEstadoPedidoDetalleConfiguration : IEntityTypeConfiguration<CatEstadoPedidoDetalle>
{
    public void Configure(EntityTypeBuilder<CatEstadoPedidoDetalle> builder)
    {
        builder.ToTable("CatEstadoPedidoDetalle");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

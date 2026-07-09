using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatEstadoPedidoConfiguration : IEntityTypeConfiguration<CatEstadoPedido>
{
    public void Configure(EntityTypeBuilder<CatEstadoPedido> builder)
    {
        builder.ToTable("CatEstadoPedido");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

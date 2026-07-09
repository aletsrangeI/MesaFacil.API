using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatTipoPedidoConfiguration : IEntityTypeConfiguration<CatTipoPedido>
{
    public void Configure(EntityTypeBuilder<CatTipoPedido> builder)
    {
        builder.ToTable("CatTipoPedido");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

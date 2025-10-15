using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PedidoAsientoConfiguration : IEntityTypeConfiguration<PedidoAsiento>
{
    public void Configure(EntityTypeBuilder<PedidoAsiento> e)
    {
        e.ToTable("PedidoAsiento");
        e.HasKey(x => x.Id);
        e.HasIndex(x => new { x.IdPedido, x.NumeroAsiento }).IsUnique();
        e.HasOne(x => x.Pedido)
            .WithMany(x => x.Asientos)
            .HasForeignKey(x => x.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EventoPedidoConfiguration : IEntityTypeConfiguration<EventoPedido>
{
    public void Configure(EntityTypeBuilder<EventoPedido> e)
    {
        e.ToTable("EventoPedido");
        e.HasKey(x => x.Id);
        e.Property(x => x.TipoEvento).HasMaxLength(64);

        e.HasOne(x => x.Pedido)
            .WithMany(x => x.Eventos)
            .HasForeignKey(x => x.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Usuario)
            .WithMany(x => x.EventosPedido)
            .HasForeignKey(x => x.IdUsuario)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

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

        // Spec 024: quien autorizó con PIN de supervisor (Gerente/Administrador).
        e.HasOne(x => x.UsuarioSupervisor)
            .WithMany()
            .HasForeignKey(x => x.IdUsuarioSupervisor)
            .OnDelete(DeleteBehavior.SetNull);

        e.Property(x => x.MontoCancelado).HasPrecision(18, 2);
        e.Property(x => x.PorcentajeDescuento).HasPrecision(5, 2);
    }
}

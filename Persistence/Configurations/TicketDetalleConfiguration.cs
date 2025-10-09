using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TicketDetalleConfiguration : IEntityTypeConfiguration<TicketDetalle>
{
    public void Configure(EntityTypeBuilder<TicketDetalle> e)
    {
        e.ToTable("TicketDetalle");
        e.HasKey(x => x.Id);

        e.HasOne(x => x.Ticket)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.IdTicket)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.DetallePedido)
            .WithMany(x => x.TicketsDetalle)
            .HasForeignKey(x => x.IdDetalle)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.EstadoItem)
            .WithMany()
            .HasForeignKey(x => new { x.EstadoCatalogId, x.EstadoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

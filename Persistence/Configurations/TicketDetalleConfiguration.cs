using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TicketDetalleConfiguration : IEntityTypeConfiguration<TicketDetalle>
{
    public void Configure(EntityTypeBuilder<TicketDetalle> e)
    {
        e.ToTable("TicketDetalle");
        
        // Asumiendo que BaseAuditableEntity provee el Id (IdTicketDetalle)
        e.HasKey(x => x.Id);

        // Relación con el Ticket de Cocina (Cabecera)
        e.HasOne(x => x.Ticket)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.IdTicket)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con el detalle original del pedido para trazabilidad
        e.HasOne(x => x.DetallePedido)
            .WithMany(x => x.TicketsDetalle)
            .HasForeignKey(x => x.IdDetalle)
            .OnDelete(DeleteBehavior.Restrict);

        // ==========================================
        // [CORREGIDO] - Relación con Catálogo Tipado
        // ==========================================
        e.HasOne(x => x.EstadoItemKDS)
            .WithMany()
            .HasForeignKey(x => x.IdEstadoItemKDS)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
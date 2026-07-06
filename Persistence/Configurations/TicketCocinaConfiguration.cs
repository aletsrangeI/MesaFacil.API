using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TicketCocinaConfiguration : IEntityTypeConfiguration<TicketCocina>
{
    public void Configure(EntityTypeBuilder<TicketCocina> e)
    {
        e.ToTable("TicketCocina");
        
        // Asumiendo que BaseAuditableEntity provee el Id (IdTicket)
        e.HasKey(x => x.Id);

        // [CORREGIDO] - Ajuste para SQL Server y consistencia con DBML
        e.Property(x => x.CompletadoEn)
            .IsRequired(false);

        // Relación con la Estación de Cocina
        e.HasOne(x => x.Estacion)
            .WithMany(x => x.Tickets) // Asegúrate de que EstacionCocina tenga ICollection<TicketCocina>
            .HasForeignKey(x => x.IdEstacion)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con el Pedido
        e.HasOne(x => x.Pedido)
            .WithMany(x => x.TicketsCocina)
            .HasForeignKey(x => x.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        // ==========================================
        // [CORREGIDO] - Relación con Catálogo Tipado
        // ==========================================
        e.HasOne(x => x.EstadoTicketCocina)
            .WithMany()
            .HasForeignKey(x => x.IdEstadoTicketCocina)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
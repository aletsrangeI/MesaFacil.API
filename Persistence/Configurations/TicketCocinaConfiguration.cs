using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class TicketCocinaConfiguration : IEntityTypeConfiguration<TicketCocina>
{
    public void Configure(EntityTypeBuilder<TicketCocina> e)
    {
        const string tsTz = "timestamptz";
        
        e.ToTable("TicketCocina");
        e.HasKey(x => x.Id);
        e.Property(x => x.CompletadoEn).HasColumnType(tsTz);

        e.HasOne(x => x.Estacion)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.IdEstacion)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Pedido)
            .WithMany(x => x.TicketsCocina)
            .HasForeignKey(x => x.IdPedido)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.EstadoItem)
            .WithMany()
            .HasForeignKey(x => new { x.EstadoCatalogId, x.EstadoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}

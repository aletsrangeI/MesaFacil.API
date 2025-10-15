using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PedidoModificadorConfiguration : IEntityTypeConfiguration<PedidoModificador>
{
    public void Configure(EntityTypeBuilder<PedidoModificador> e)
    {
        e.ToTable("PedidoModificador");
        e.HasKey(x => x.Id);
        e.Property(x => x.PrecioExtra).HasColumnType("numeric(12,2)").HasDefaultValue(0m);

        e.HasOne(x => x.Detalle)
            .WithMany(x => x.Modificadores)
            .HasForeignKey(x => x.IdDetalle)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Opcion)
            .WithMany(x => x.PedidoModificadores)
            .HasForeignKey(x => x.IdOpcion)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

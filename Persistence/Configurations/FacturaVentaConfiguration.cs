using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class FacturaVentaConfiguration : IEntityTypeConfiguration<FacturaVenta>
{
    public void Configure(EntityTypeBuilder<FacturaVenta> builder)
    {
        builder.ToTable("FacturasVenta");
        builder.HasKey(x => x.Id);

        // No duplicidad: un UUID de timbre no se repite (ignorando nulos, permitido por Postgres en índice único).
        builder.HasIndex(x => x.UUID).IsUnique();

        builder.HasIndex(x => x.TicketAutofacturaGuid).IsUnique();

        // Un PedidoId no puede tener más de una factura activa (Vigente/Pendiente).
        builder.HasIndex(x => x.PedidoId);

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Sucursal)
            .WithMany()
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Detalles)
            .WithOne(x => x.FacturaVenta)
            .HasForeignKey(x => x.IdFacturaVenta)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

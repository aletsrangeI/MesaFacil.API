using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class SucursalConfiguration : IEntityTypeConfiguration<Sucursal>
{
    public void Configure(EntityTypeBuilder<Sucursal> e)
    {
        e.ToTable("Sucursal");
        e.HasKey(x => x.Id);
        e.Property(x => x.Nombre).HasMaxLength(200);
        e.Property(x => x.Direccion).HasMaxLength(400);
        e.Property(x => x.ZonaHoraria).HasMaxLength(64);
        e.HasOne(x => x.Empresa)
            .WithMany(x => x.Sucursales)
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

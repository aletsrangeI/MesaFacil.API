using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class RolAccesoRutaConfiguration : IEntityTypeConfiguration<RolAccesoRuta>
{
    public void Configure(EntityTypeBuilder<RolAccesoRuta> e)
    {
        e.ToTable("RolAccesoRuta");
        e.HasKey(x => x.Id);

        e.Property(x => x.IdRol).IsRequired();
        e.Property(x => x.IdAccesoRuta).IsRequired();

        // Evitar rol-ruta duplicados
        e.HasIndex(x => new { x.IdRol, x.IdAccesoRuta })
            .IsUnique()
            .HasDatabaseName("UX_RolAccesoRuta_Rol_Path");

        e.HasOne(x => x.Rol)
            .WithMany(r => r.AccesosRuta)
            .HasForeignKey(x => x.IdRol)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.AccesoRuta)
            .WithMany(x => x.RolesConAcceso)
            .HasForeignKey(x => x.IdAccesoRuta)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

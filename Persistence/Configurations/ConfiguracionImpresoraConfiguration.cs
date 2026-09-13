using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ConfiguracionImpresoraConfiguration : IEntityTypeConfiguration<ConfiguracionImpresora>
{
    public void Configure(EntityTypeBuilder<ConfiguracionImpresora> e)
    {
        e.ToTable("ConfiguracionImpresora");
        e.HasKey(x => x.Id);

        e.Property(x => x.Nombre).HasMaxLength(120);
        e.Property(x => x.DireccionIp).HasMaxLength(45);
        e.Property(x => x.EstacionAsociada).HasMaxLength(80);
        e.Property(x => x.Puerto).HasDefaultValue(9100);

        e.Property(x => x.TipoConexion)
            .HasConversion<string>()
            .HasMaxLength(30);

        e.Property(x => x.AnchoPapel)
            .HasConversion<string>()
            .HasMaxLength(10);

        e.HasOne(x => x.Sucursal)
            .WithMany()
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasIndex(x => x.IdSucursal);
    }
}

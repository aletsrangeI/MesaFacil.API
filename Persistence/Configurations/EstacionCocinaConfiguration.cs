using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EstacionCocinaConfiguration : IEntityTypeConfiguration<EstacionCocina>
{
    public void Configure(EntityTypeBuilder<EstacionCocina> e)
    {
        e.ToTable("EstacionCocina");
        e.HasKey(x => x.Id);
        e.Property(x => x.Nombre).HasMaxLength(120);

        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.EstacionesCocina)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

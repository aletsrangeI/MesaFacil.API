using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class DescuentoAplicadoConfiguration : IEntityTypeConfiguration<DescuentoAplicado>
{
    public void Configure(EntityTypeBuilder<DescuentoAplicado> e)
    {
        e.ToTable("DescuentoAplicado");
        e.HasKey(x => x.Id);
        e.Property(x => x.Valor).HasColumnType("numeric(12,4)").IsRequired();
        e.Property(x => x.Alcance).HasMaxLength(32);
        e.Property(x => x.Condiciones); // nvarchar(max) -> text

        e.HasOne(x => x.Cuenta)
            .WithMany(x => x.Descuentos)
            .HasForeignKey(x => x.IdCuenta)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.TipoItem)
            .WithMany()
            .HasForeignKey(x => new { x.TipoCatalogId, x.TipoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
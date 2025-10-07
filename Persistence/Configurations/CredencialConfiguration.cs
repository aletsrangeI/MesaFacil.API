using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CredencialConfiguration : IEntityTypeConfiguration<Credencial>
{
    public void Configure(EntityTypeBuilder<Credencial> e)
    {
        e.ToTable("Credencial");
        e.HasKey(x => new { x.IdUsuario, x.TipoCatalogId, x.TipoItemId });
        e.Property(x => x.Hash).HasMaxLength(256).IsRequired();
        e.Property(x => x.Salt).HasMaxLength(128);

        e.HasOne(x => x.Usuario)
            .WithMany(x => x.Credenciales)
            .HasForeignKey(x => x.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.TipoItem)
            .WithMany()
            .HasForeignKey(x => new { x.TipoCatalogId, x.TipoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
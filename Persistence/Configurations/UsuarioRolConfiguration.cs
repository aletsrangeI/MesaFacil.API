using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> e)
    {
        e.ToTable("UsuarioRol");
        e.HasKey(x => x.Id);

        e.Property(x => x.UsuarioId).IsRequired();
        e.Property(x => x.IdRol).IsRequired();

        // Evitar duplicados usuario-rol
        e.HasIndex(x => new { x.UsuarioId, x.IdRol })
            .IsUnique()
            .HasDatabaseName("UX_UsuarioRol_Usuario_Rol");

        e.HasOne(x => x.Usuario)
            .WithMany(u => u.UsuarioRoles)
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(x => x.IdRol)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

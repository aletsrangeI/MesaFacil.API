using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CredencialConfiguration : IEntityTypeConfiguration<Credencial>
{
    public void Configure(EntityTypeBuilder<Credencial> e)
    {
        e.ToTable("Credencial");
        
        // [CORREGIDO] - La nueva llave primaria compuesta
        e.HasKey(x => new { x.IdUsuario, x.IdCredencial });
        
        e.Property(x => x.Hash).HasMaxLength(256).IsRequired();
        e.Property(x => x.Salt).HasMaxLength(128);

        // Relación con Usuario
        e.HasOne(x => x.Usuario)
            .WithMany(x => x.Credenciales) // Asegúrate de tener public ICollection<Credencial> Credenciales en Usuario
            .HasForeignKey(x => x.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        // [CORREGIDO] - Relación directa con el catálogo tipado CatCredencial
        e.HasOne(x => x.CatCredencial)
            .WithMany()
            .HasForeignKey(x => x.IdCredencial)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}
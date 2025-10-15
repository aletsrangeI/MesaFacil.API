using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre).HasMaxLength(200);
        builder.Property(x => x.Telefono).HasMaxLength(32);
        builder.Property(x => x.Correo).HasMaxLength(200);

        builder.HasOne(x => x.Empresa)
            .WithMany(x => x.Clientes)
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
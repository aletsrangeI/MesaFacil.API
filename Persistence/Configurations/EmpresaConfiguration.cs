using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> e)
    {
        e.ToTable("Empresa");
        e.HasKey(x => x.Id);
        e.Property(x => x.Nombre).HasMaxLength(200);
        e.Property(x => x.Rfc).HasMaxLength(20);
    }
}
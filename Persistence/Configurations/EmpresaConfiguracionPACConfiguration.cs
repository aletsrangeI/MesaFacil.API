using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EmpresaConfiguracionPACConfiguration : IEntityTypeConfiguration<EmpresaConfiguracionPAC>
{
    public void Configure(EntityTypeBuilder<EmpresaConfiguracionPAC> builder)
    {
        builder.ToTable("EmpresaConfiguracionesPAC");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.IdEmpresa).IsUnique();

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

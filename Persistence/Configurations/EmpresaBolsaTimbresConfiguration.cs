using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EmpresaBolsaTimbresConfiguration : IEntityTypeConfiguration<EmpresaBolsaTimbres>
{
    public void Configure(EntityTypeBuilder<EmpresaBolsaTimbres> builder)
    {
        builder.ToTable("EmpresaBolsasTimbres");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.IdEmpresa).IsUnique();

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EmpresaSuscripcionConfiguration : IEntityTypeConfiguration<EmpresaSuscripcion>
{
    public void Configure(EntityTypeBuilder<EmpresaSuscripcion> builder)
    {
        builder.ToTable("EmpresasSuscripcion");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.IdEmpresa);

        builder.HasOne(x => x.Empresa)
            .WithMany()
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Plan)
            .WithMany(p => p.Suscripciones)
            .HasForeignKey(x => x.IdPlan)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

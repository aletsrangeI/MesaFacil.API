using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class OpcionModificadorConfiguration : IEntityTypeConfiguration<OpcionModificador>
{
    public void Configure(EntityTypeBuilder<OpcionModificador> e)
    {
        e.ToTable("OpcionModificador");
        e.HasKey(x => x.Id);
        e.Property(x => x.Nombre).HasMaxLength(150);
        e.Property(x => x.PrecioExtra).HasColumnType("numeric(12,2)").HasDefaultValue(0m);
        e.Property(x => x.EsDefault).HasDefaultValue(false);

        e.HasOne(x => x.Grupo)
            .WithMany(x => x.Opciones)
            .HasForeignKey(x => x.IdGrupo)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

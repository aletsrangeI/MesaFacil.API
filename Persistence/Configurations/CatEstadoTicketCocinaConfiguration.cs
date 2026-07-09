using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatEstadoTicketCocinaConfiguration : IEntityTypeConfiguration<CatEstadoTicketCocina>
{
    public void Configure(EntityTypeBuilder<CatEstadoTicketCocina> builder)
    {
        builder.ToTable("CatEstadoTicketCocina");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(100);
    }
}

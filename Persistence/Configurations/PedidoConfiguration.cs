using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> e)
    {
        const string tsTz = "timestamptz";
        
        e.ToTable("Pedido");
        e.HasKey(x => x.Id);
        e.Property(x => x.AbiertoEn).HasColumnType(tsTz).HasDefaultValueSql("now()");
        e.Property(x => x.CerradoEn).HasColumnType(tsTz);
        e.Property(x => x.Notas).HasMaxLength(500);
        e.Property(x => x.CargoServicioPct).HasColumnType("numeric(5,2)").HasDefaultValue(0m);

        e.HasOne(x => x.Empresa)
            .WithMany(x => x.Pedidos)
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.Pedidos)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Mesa)
            .WithMany(x => x.Pedidos)
            .HasForeignKey(x => x.IdMesa)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.Cliente)
            .WithMany(x => x.Pedidos)
            .HasForeignKey(x => x.IdCliente)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.AbiertoPorUsuario)
            .WithMany(x => x.PedidosAbiertos)
            .HasForeignKey(x => x.AbiertoPor)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.CerradoPorUsuario)
            .WithMany(x => x.PedidosCerrados)
            .HasForeignKey(x => x.CerradoPor)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.TipoItem)
            .WithMany()
            .HasForeignKey(x => new { x.TipoCatalogId, x.TipoItemId })
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.EstadoItem)
            .WithMany()
            .HasForeignKey(x => new { x.EstadoCatalogId, x.EstadoItemId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
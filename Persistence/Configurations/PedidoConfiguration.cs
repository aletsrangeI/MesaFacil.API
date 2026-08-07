using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> e)
    {
        e.ToTable("Pedido");
        
        // Asumiendo que BaseAuditableEntity provee la propiedad 'Id'
        e.HasKey(x => x.Id); 
        
        // [CORREGIDO] Ajustado a SQL Server (datetime2) y valor por defecto consistente con DBML
        e.Property(x => x.AbiertoEn)
            .IsRequired()
            .HasDefaultValueSql("now()");
            
        e.Property(x => x.CerradoEn);
        
        e.Property(x => x.Personas)
            .IsRequired()
            .HasDefaultValue(1);

        e.Property(x => x.Notas)
            .HasMaxLength(500);

        e.Property(x => x.CargoServicioPct)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0m);

        // Relaciones Base
        e.HasOne(x => x.Empresa)
            .WithMany(x => x.Pedidos) // <-- IMPORTANTE: Indicar la colección de regreso
            .HasForeignKey(x => x.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Mesa)
            .WithMany(x => x.Pedidos)
            .HasForeignKey(x => x.IdMesa)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.Sucursal)
            .WithMany(x => x.Pedidos)
            .HasForeignKey(x => x.IdSucursal)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.Cliente)
            .WithMany(x => x.Pedidos) // <-- IMPORTANTE
            .HasForeignKey(x => x.IdCliente)
            .OnDelete(DeleteBehavior.SetNull);

        // Relaciones con Usuarios (Staff)
        e.HasOne(x => x.AbiertoPorUsuario)
            .WithMany(x => x.PedidosAbiertos) // Asegúrate que Usuario tenga esta colección
            .HasForeignKey(x => x.AbiertoPor)
            .OnDelete(DeleteBehavior.SetNull);

        e.HasOne(x => x.CerradoPorUsuario)
            .WithMany(x => x.PedidosCerrados) // Asegúrate que Usuario tenga esta colección
            .HasForeignKey(x => x.CerradoPor)
            .OnDelete(DeleteBehavior.SetNull);

        // ==========================================
        // [CORREGIDO] - Relaciones con Catálogos Tipados
        // ==========================================
        e.HasOne(x => x.TipoPedido)
            .WithMany()
            .HasForeignKey(x => x.IdTipoPedido)
            .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(x => x.EstadoPedido)
            .WithMany()
            .HasForeignKey(x => x.IdEstadoPedido)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
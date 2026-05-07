using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> e)
    {
        e.ToTable("Producto");
        
        // Asumiendo que BaseAuditableEntity provee el Id (IdProducto)
        e.HasKey(x => x.Id);

        e.Property(x => x.Codigo)
            .HasMaxLength(64);

        e.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        e.Property(x => x.Descripcion)
            .HasMaxLength(400);

        e.Property(x => x.Activo)
            .HasDefaultValue(true);

        // Relación con el Menú
        e.HasOne(x => x.Menu)
            .WithMany(m => m.Productos) // Asegúrate de que la entidad Menu tenga ICollection<Producto>
            .HasForeignKey(x => x.IdMenu)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con la Categoría
        e.HasOne(x => x.Categoria)
            .WithMany(c => c.Productos) // Asegúrate de que CategoriaMenu tenga ICollection<Producto>
            .HasForeignKey(x => x.IdCategoria)
            .OnDelete(DeleteBehavior.Restrict);

        // ==========================================
        // [CORREGIDO] - Estación cocina (Catálogo Tipado)
        // ==========================================
        e.HasOne(x => x.EstacionCocina)
            .WithMany()
            .HasForeignKey(x => x.IdEstacionCocina)
            .OnDelete(DeleteBehavior.SetNull); 
        // Si se borra una estación, el producto simplemente se queda sin estación asignada
    }
}
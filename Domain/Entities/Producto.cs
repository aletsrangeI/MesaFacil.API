using System.Collections.Generic;

namespace Domain.Entities;

public class Producto : BaseAuditableEntity // Asume que hereda IdProducto
{
    public int IdMenu { get; set; }
    public int IdCategoria { get; set; }
    public string? Codigo { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    // [CORREGIDO] - Fuera catálogos genéricos, entra llave foránea específica (es nullable según tu DBML)
    public int? IdEstacionCocina { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Menu Menu { get; set; } = null!;
    public CategoriaMenu Categoria { get; set; } = null!;
    
    // [CORREGIDO] - Navegación a la entidad fuertemente tipada
    public CatEstacionesCocina? EstacionCocina { get; set; }

    // Colecciones hijas
    public ICollection<VarianteProducto> Variantes { get; set; } = new List<VarianteProducto>();

    public ICollection<Precio> PreciosDeprecatedIgnore { get; set; } = new List<Precio>(); // (solo para claridad; precios reales van en Variante)

    public ICollection<GrupoModificador> GruposModificador { get; set; } = new List<GrupoModificador>();
    public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
}
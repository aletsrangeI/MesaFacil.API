using System.Collections.Generic;

namespace Domain.Entities;

public class Cuenta : BaseAuditableEntity // Id propio permanece int; FK a Pedido es Guid (Spec 019)
{
    public Guid IdPedido { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DescuentoTotal { get; set; }
    public decimal CargoServicio { get; set; }
    public decimal ImpuestoTotal { get; set; }
    public decimal Total { get; set; }

    // [CORREGIDO] - La única llave foránea necesaria para el estado
    public int IdEstadoCuenta { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Pedido Pedido { get; set; } = null!;
    
    // [CORREGIDO] - Propiedad de navegación tipada
    public CatEstadoCuenta EstadoCuenta { get; set; } = null!;

    // Colecciones hijas
    public ICollection<DetalleCuenta> Detalles { get; set; } = new List<DetalleCuenta>();
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    public ICollection<DescuentoAplicado> Descuentos { get; set; } = new List<DescuentoAplicado>();
}
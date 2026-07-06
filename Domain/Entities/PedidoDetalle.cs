using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class PedidoDetalle : BaseAuditableEntity // Asume que hereda IdDetalle
{
    public int IdPedido { get; set; }
    public int? IdAsiento { get; set; }
    
    public int IdProducto { get; set; }
    
    // [CORREGIDO] - Ahora es NOT NULL para garantizar la trazabilidad de la variante vendida
    public int IdVariante { get; set; }

    // ==========================================
    // SNAPSHOTS (Inmutabilidad Histórica)
    // ==========================================
    public string ProductoNombre { get; set; } = string.Empty;
    public string VarianteNombre { get; set; } = string.Empty;

    public decimal Cantidad { get; set; } = 1m;
    public decimal PrecioUnitario { get; set; }

    // ==========================================
    // IMPUESTOS CONGELADOS
    // ==========================================
    public int IdImpuesto { get; set; }
    public decimal TasaImpuesto { get; set; }
    public decimal MontoImpuesto { get; set; }

    public string? Notas { get; set; }

    // [CORREGIDO] - Referencia directa al catálogo tipado
    public int IdEstadoPedidoDetalle { get; set; }

    // ==========================================
    // MANEJO DE CANCELACIONES
    // ==========================================
    public bool Cancelado { get; set; } = false;
    public string? MotivoCancelacion { get; set; }
    public int? CanceladoPor { get; set; }
    public DateTime? CanceladoEn { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Pedido Pedido { get; set; } = null!;
    public PedidoAsiento? Asiento { get; set; }
    public Producto Producto { get; set; } = null!;
    
    // [CORREGIDO] - Ya no es nulable
    public VarianteProducto Variante { get; set; } = null!;

    // [NUEVAS] - Navegaciones a los catálogos y entidades fuertes
    public CatEstadoPedidoDetalle EstadoPedidoDetalle { get; set; } = null!;
    public CatImpuesto Impuesto { get; set; } = null!;
    public Usuario? UsuarioCancela { get; set; }

    // Colecciones hijas
    public ICollection<PedidoModificador> Modificadores { get; set; } = new List<PedidoModificador>();
    public ICollection<TicketDetalle> TicketsDetalle { get; set; } = new List<TicketDetalle>();
}
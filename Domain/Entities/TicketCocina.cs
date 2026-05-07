using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class TicketCocina : BaseAuditableEntity // Asume que hereda IdTicket
{
    public int IdEstacion { get; set; }
    public int IdPedido { get; set; }

    // [CORREGIDO] - Eliminamos el catálogo universal y usamos el tipado
    public int IdEstadoTicketCocina { get; set; }
    
    public DateTime? CompletadoEn { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public EstacionCocina Estacion { get; set; } = null!;
    public Pedido Pedido { get; set; } = null!;
    
    // [CORREGIDO] - Navegación directa a la entidad de catálogo específica
    public CatEstadoTicketCocina EstadoTicketCocina { get; set; } = null!;

    // Colecciones hijas
    public ICollection<TicketDetalle> Detalles { get; set; } = new List<TicketDetalle>();
}
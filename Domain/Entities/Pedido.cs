using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Pedido : BaseAuditableEntity // Asumo que aquí heredas el Id (IdPedido) y campos de auditoría si los usas
{
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }
    public int? IdCliente { get; set; }

    // [NUEVO] - Campo para métricas de KPI (Ticket Promedio)
    public int Personas { get; set; } = 1;

    public int? AbiertoPor { get; set; }
    public int? CerradoPor { get; set; }
    public DateTime AbiertoEn { get; set; } = DateTime.UtcNow; // sysutcdatetime() por default
    public DateTime? CerradoEn { get; set; }
    public string? Notas { get; set; }

    // [CORREGIDO] - Referencias directas a los nuevos catálogos tipados
    public int IdTipoPedido { get; set; }
    public int IdEstadoPedido { get; set; }

    public decimal CargoServicioPct { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public Empresa Empresa { get; set; } = null!;
    public Sucursal Sucursal { get; set; } = null!;
    public Mesa? Mesa { get; set; }
    public Cliente? Cliente { get; set; }

    public Usuario? AbiertoPorUsuario { get; set; }
    public Usuario? CerradoPorUsuario { get; set; }
    
    public CatTipoPedido TipoPedido { get; set; } = null!;
    public CatEstadoPedido EstadoPedido { get; set; } = null!;

    // Colecciones hijas
    public ICollection<PedidoAsiento> Asientos { get; set; } = new List<PedidoAsiento>();
    public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    public ICollection<EventoPedido> Eventos { get; set; } = new List<EventoPedido>();
    public ICollection<TicketCocina> TicketsCocina { get; set; } = new List<TicketCocina>();
    public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
}
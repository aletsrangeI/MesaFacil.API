using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Pedido : BaseAuditableGuidEntity // Spec 019: Id (Guid/UUIDv7) generado en servidor/edge, no autoincremental
{
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public int? IdMesa { get; set; }
    public int? IdCliente { get; set; }

    // [NUEVO] - Campo para métricas de KPI (Ticket Promedio)
    public int Personas { get; set; } = 1;

    // Spec 019: folio humano correlativo por sucursal/turno, generado atómicamente por FoliadorSucursal
    public int FolioDiario { get; set; }

    public int? AbiertoPor { get; set; }
    public int? CerradoPor { get; set; }
    public DateTime AbiertoEn { get; set; } = DateTime.UtcNow; // sysutcdatetime() por default
    public DateTime? CerradoEn { get; set; }
    public string? Notas { get; set; }

    // [CORREGIDO] - Referencias directas a los nuevos catálogos tipados
    public int IdTipoPedido { get; set; }
    public int IdEstadoPedido { get; set; }

    public decimal CargoServicioPct { get; set; }
    public string? IdempotencyKey { get; set; }

    // [DELIVERY Y PLATAFORMAS]
    public string CanalOrigen { get; set; } = "POS";
    public string? IdExterno { get; set; }
    public string? NombreClienteDelivery { get; set; }
    public string? TelefonoDelivery { get; set; }
    public string? DireccionEntrega { get; set; }
    public string? NombreRepartidor { get; set; }
    public string? TelefonoRepartidor { get; set; }
    public DateTime? DespachadoEn { get; set; }
    public DateTime? EntregadoEn { get; set; }

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
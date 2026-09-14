using System;
using System.Collections.Generic;

namespace DTO.Delivery;

public class DeliveryQueueItemDTO
{
    public Guid IdPedido { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string TipoPedido { get; set; } = string.Empty; // "Para Llevar", "Delivery"
    public bool IsComedor { get; set; }
    public string CanalOrigen { get; set; } = "POS"; // "POS", "Uber Eats", "Rappi", "Didi Food", "Delivery Propio"
    public string? IdExterno { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? DireccionEntrega { get; set; }
    public string? NombreRepartidor { get; set; }
    public string? TelefonoRepartidor { get; set; }
    public int IdEstadoPedido { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public bool EstaPagado { get; set; }
    public DateTime AbiertoEn { get; set; }
    public DateTime? ListoEn { get; set; }
    public DateTime? DespachadoEn { get; set; }
    public DateTime? EntregadoEn { get; set; }
    public int MinutosEnEstado { get; set; }
    public List<DeliveryItemDetalleDTO> Items { get; set; } = new();
}

public class DeliveryItemDetalleDTO
{
    public Guid IdDetalle { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public string? VarianteNombre { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Notas { get; set; }
    public List<string> Modificadores { get; set; } = new();
}

public class DespacharPedidoRequestDTO
{
    public string? NombreRepartidor { get; set; }
    public string? TelefonoRepartidor { get; set; }
    public string? IdExterno { get; set; }
}

public class RebotarPedidoRequestDTO
{
    public string Motivo { get; set; } = string.Empty;
}

public class EntregarPedidoRequestDTO
{
    public int? IdMetodoDePago { get; set; }
}


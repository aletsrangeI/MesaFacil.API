using System;
using System.Collections.Generic;

namespace DTO.Delivery;

public class DeliveryHistorialItemDTO
{
    public Guid IdPedido { get; set; }
    public string Folio { get; set; } = string.Empty;
    public string TipoPedido { get; set; } = string.Empty;
    public string CanalOrigen { get; set; } = "POS";
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
    public DateTime? CerradoEn { get; set; }
    public int? MinutosTotales { get; set; }
    public string? MotivoCancelacion { get; set; }
    public List<DeliveryItemDetalleDTO> Items { get; set; } = new();
    public List<DeliveryEventoAuditoriaDTO> Eventos { get; set; } = new();
}

public class DeliveryEventoAuditoriaDTO
{
    public int Id { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? Payload { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class CanalVentaResumenDTO
{
    public string Canal { get; set; } = string.Empty;
    public int CantidadPedidos { get; set; }
    public decimal TotalVentas { get; set; }
    public double Porcentaje { get; set; }
}

public class ResumenHistorialDeliveryDTO
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int TotalPedidos { get; set; }
    public decimal TotalVentas { get; set; }
    public int TotalEntregados { get; set; }
    public int TotalRebotados { get; set; }
    public int TotalEnCamino { get; set; }
    public double TasaExitoPorcentaje { get; set; }
    public double TiempoPromedioEntregaMinutos { get; set; }
    public List<CanalVentaResumenDTO> VentasPorCanal { get; set; } = new();
    public List<DeliveryHistorialItemDTO> Pedidos { get; set; } = new();
}

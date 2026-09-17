using System.Text.Json;
using Common;
using Domain.Entities;
using DTO.Auditoria;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Auditoria;

/// <summary>
/// Spec 024, sección 2.4: Monitor de Auditoría y Umbral del 2% en Dashboard.
///
/// Ventana del turno: [Turno.Apertura, Turno.Cierre ?? UtcNow], acotada a Turno.IdSucursal — el
/// mismo criterio que ya usa CorteCajaApplication.ObtenerResumenActualAsync para calcular ventas
/// (suma de Pago.Monto de las Cuentas de Pedidos de esa sucursal en la ventana), evitando
/// duplicar/():reinventar la definición de "venta del turno".
///
/// Detalle del platillo/mesa/mesero en el desglose: EventoPedido sólo ganó 3 campos nuevos en este
/// spec (IdUsuarioSupervisor, MontoCancelado, PorcentajeDescuento); el resto de metadatos
/// (IdPedidoDetalle, producto, motivo) se leen del Payload JSON que ya es el mecanismo de
/// event-sourcing de esta entidad desde spec 003.
/// </summary>
public class AuditoriaCancelacionesService : IAuditoriaCancelacionesService
{
    private const decimal UmbralAlertaPorcentaje = 2.0m;
    private const string TipoEventoPlatilloCancelado = "PlatilloCancelado";
    private const string TipoEventoDescuentoAutorizado = "DescuentoAutorizado";

    private readonly ApplicationDbContext _context;

    public AuditoriaCancelacionesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Response<ResumenCancelacionesTurnoDTO>> ObtenerResumenTurnoAsync(int idTurno, CancellationToken ct = default)
    {
        var response = new Response<ResumenCancelacionesTurnoDTO>();

        var turno = await _context.Turnos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == idTurno, ct);
        if (turno is null)
        {
            response.Message = $"Turno {idTurno} no encontrado.";
            return response;
        }

        var fechaInicio = turno.Apertura;
        var fechaFin = turno.Cierre ?? DateTime.UtcNow;

        // Venta neta del turno (sin propinas), misma definición que CorteCajaApplication.
        var totalVentasTurno = await _context.Pagos
            .Where(p => p.IsActive
                        && p.PagadoEn >= fechaInicio && p.PagadoEn <= fechaFin
                        && p.Cuenta.Pedido.IdSucursal == turno.IdSucursal)
            .SumAsync(p => (decimal?)p.Monto, ct) ?? 0m;

        var eventosCancelacion = await _context.EventosPedido
            .Include(e => e.Pedido).ThenInclude(p => p.Mesa)
            .Include(e => e.Usuario)
            .Include(e => e.UsuarioSupervisor)
            .Where(e => e.IsActive
                        && (e.TipoEvento == TipoEventoPlatilloCancelado || e.TipoEvento == TipoEventoDescuentoAutorizado)
                        && e.CreatedAt >= fechaInicio && e.CreatedAt <= fechaFin
                        && e.Pedido.IdSucursal == turno.IdSucursal)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);

        var totalCancelacionesTurno = eventosCancelacion.Sum(e => e.MontoCancelado ?? 0m);

        var denominador = totalVentasTurno + totalCancelacionesTurno;
        var porcentaje = denominador > 0 ? Math.Round(totalCancelacionesTurno / denominador * 100m, 2) : 0m;

        response.Data = new ResumenCancelacionesTurnoDTO
        {
            TotalVentasTurno = totalVentasTurno,
            TotalCancelacionesTurno = totalCancelacionesTurno,
            PorcentajeCancelaciones = porcentaje,
            SuperaUmbralAlerta = porcentaje > UmbralAlertaPorcentaje,
            TotalEventos = eventosCancelacion.Count,
            Desglose = eventosCancelacion.Select(MapDesglose).ToList()
        };
        response.isSuccess = true;
        response.Message = "Resumen de cancelaciones del turno calculado correctamente.";
        return response;
    }

    public async Task<Response<List<MotivoCancelacionDTO>>> ObtenerMotivosAsync(CancellationToken ct = default)
    {
        var response = new Response<List<MotivoCancelacionDTO>>();

        var motivos = await _context.CatMotivosCancelacion
            .Where(m => m.IsActive)
            .OrderBy(m => m.Descripcion)
            .Select(m => new MotivoCancelacionDTO { Id = m.Id, Descripcion = m.Descripcion })
            .ToListAsync(ct);

        response.Data = motivos;
        response.isSuccess = true;
        response.Message = "Catálogo de motivos de cancelación obtenido correctamente.";
        return response;
    }

    private static DesgloseCancelacionDTO MapDesglose(EventoPedido evento)
    {
        string? platillo = null;
        string? motivo = null;

        if (evento.TipoEvento == TipoEventoDescuentoAutorizado)
        {
            platillo = $"Descuento {evento.PorcentajeDescuento ?? 0:F1}%";
        }

        if (!string.IsNullOrWhiteSpace(evento.Payload))
        {
            try
            {
                using var doc = JsonDocument.Parse(evento.Payload);
                if (platillo == null && doc.RootElement.TryGetProperty("ProductoNombre", out var p)) platillo = p.GetString();
                if (doc.RootElement.TryGetProperty("Motivo", out var m)) motivo = m.GetString();
            }
            catch (JsonException)
            {
                // Payload histórico/no-JSON: se ignora, el desglose queda sin estos campos opcionales.
            }
        }

        return new DesgloseCancelacionDTO
        {
            FechaHora = evento.CreatedAt,
            Mesa = evento.Pedido?.Mesa?.Codigo,
            Platillo = platillo,
            Importe = evento.MontoCancelado ?? 0m,
            Mesero = evento.Usuario?.NombreCompleto,
            Supervisor = evento.UsuarioSupervisor?.NombreCompleto,
            Motivo = motivo
        };
    }
}

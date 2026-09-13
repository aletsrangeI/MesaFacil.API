using Common;
using Domain.Entities;
using DTO.Facturacion;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Facturacion;

public interface IAutofacturacionComensalService
{
    Task<Response<ValidarTicketResultDTO>> ValidarTicketAsync(Guid ticketGuid, CancellationToken cancellationToken = default);
    Task<Response<FacturaVentaDTO>> GenerarFacturaAsync(GenerarFacturaAutofacturaRequestDTO request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Spec 020: Portal Público de Autofacturación Comensal (self-service vía QR del ticket
/// térmico). El "TicketGuid" que se imprime/escanea en el QR es el mismo Pedido.Id
/// (Guid/UUIDv7 desde Spec 019): es un identificador ya globalmente único y no reutilizable,
/// por lo que no se requiere un campo adicional en Pedido para representarlo. Al timbrarse la
/// factura, FacturaVenta.TicketAutofacturaGuid se llena con el mismo valor, sirviendo como
/// bitácora/índice único de qué ticket generó qué factura.
///
/// Valida que el pedido exista, esté pagado, no esté ya facturado (unicidad por PedidoId) y
/// esté dentro de la vigencia parametrizada (por defecto 7 días naturales desde el pago),
/// antes de delegar el timbrado real a IFacturaVentaService (mismo motor que usa el POS).
/// </summary>
public class AutofacturacionComensalService : IAutofacturacionComensalService
{
    private readonly ApplicationDbContext _context;
    private readonly IFacturaVentaService _facturaVentaService;
    private readonly int _diasVigenciaAutofactura;

    public AutofacturacionComensalService(
        ApplicationDbContext context,
        IFacturaVentaService facturaVentaService,
        int diasVigenciaAutofactura = 7)
    {
        _context = context;
        _facturaVentaService = facturaVentaService;
        _diasVigenciaAutofactura = diasVigenciaAutofactura;
    }

    public async Task<Response<ValidarTicketResultDTO>> ValidarTicketAsync(Guid ticketGuid, CancellationToken cancellationToken = default)
    {
        var response = new Response<ValidarTicketResultDTO>();

        var pedido = await _context.Pedidos
            .Include(p => p.Cuentas)
            .ThenInclude(c => c.Pagos)
            .FirstOrDefaultAsync(p => p.Id == ticketGuid, cancellationToken);

        if (pedido == null)
        {
            response.isSuccess = false;
            response.Message = "El código de ticket no corresponde a ningún pedido registrado.";
            response.Data = new ValidarTicketResultDTO { ExistePedido = false, TicketGuid = ticketGuid, MensajeError = response.Message };
            return response;
        }

        var cuenta = pedido.Cuentas.FirstOrDefault();
        var pago = cuenta?.Pagos.OrderByDescending(p => p.PagadoEn).FirstOrDefault();
        bool pagado = cuenta != null && cuenta.Total > 0 && pago != null;

        var facturaExistente = await _context.FacturasVenta
            .AnyAsync(f => f.PedidoId == ticketGuid && f.EstadoFiscal != EstadoFiscalFactura.Cancelada, cancellationToken);

        DateTime fechaBase = pago?.PagadoEn ?? pedido.CerradoEn ?? pedido.AbiertoEn;
        DateTime vigenciaLimite = fechaBase.Date.AddDays(_diasVigenciaAutofactura);
        bool vigente = DateTime.UtcNow.Date <= vigenciaLimite;

        var resultado = new ValidarTicketResultDTO
        {
            ExistePedido = true,
            PedidoPagado = pagado,
            YaFacturado = facturaExistente,
            Vigente = vigente,
            Total = cuenta?.Total ?? 0,
            FechaPedido = fechaBase,
            TicketGuid = ticketGuid
        };

        if (!pagado)
        {
            resultado.MensajeError = "El pedido todavía no ha sido cobrado; aún no puede facturarse.";
        }
        else if (facturaExistente)
        {
            resultado.MensajeError = "Este ticket ya fue facturado previamente.";
        }
        else if (!vigente)
        {
            resultado.MensajeError = $"La vigencia para autofacturar este ticket venció el {vigenciaLimite:yyyy-MM-dd}.";
        }

        response.isSuccess = true;
        response.Data = resultado;
        return response;
    }

    public async Task<Response<FacturaVentaDTO>> GenerarFacturaAsync(GenerarFacturaAutofacturaRequestDTO request, CancellationToken cancellationToken = default)
    {
        var response = new Response<FacturaVentaDTO>();

        var validacion = await ValidarTicketAsync(request.TicketGuid, cancellationToken);
        if (!validacion.isSuccess || validacion.Data == null)
        {
            response.isSuccess = false;
            response.Message = validacion.Message;
            return response;
        }

        var estado = validacion.Data;
        if (!estado.ExistePedido)
        {
            response.isSuccess = false;
            response.Message = "El código de ticket no corresponde a ningún pedido registrado.";
            return response;
        }

        if (!estado.PedidoPagado)
        {
            response.isSuccess = false;
            response.Message = "El pedido todavía no ha sido cobrado; aún no puede facturarse.";
            return response;
        }

        if (estado.YaFacturado)
        {
            response.isSuccess = false;
            response.Message = "Este ticket ya fue facturado previamente. No se puede generar una factura duplicada.";
            return response;
        }

        if (!estado.Vigente)
        {
            response.isSuccess = false;
            response.Message = "La vigencia para autofacturar este ticket ya venció.";
            return response;
        }

        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == request.TicketGuid, cancellationToken);
        if (pedido == null)
        {
            response.isSuccess = false;
            response.Message = "El pedido especificado no existe.";
            return response;
        }

        var resultadoTimbrado = await _facturaVentaService.TimbrarPedidoAsync(new TimbrarPedidoRequestDTO
        {
            PedidoId = request.TicketGuid,
            RfcReceptor = request.RfcReceptor,
            NombreReceptor = request.NombreReceptor,
            RegimenFiscalReceptor = request.RegimenFiscalReceptor,
            CodigoPostalReceptor = request.CodigoPostalReceptor,
            UsoCfdi = request.UsoCfdi,
            FormaPago = "99", // Autofacturación: "Por definir" salvo que el ticket registre la forma real de pago.
            MetodoPago = "PUE",
            CorreoReceptor = request.CorreoReceptor
        }, cancellationToken);

        if (resultadoTimbrado.isSuccess && resultadoTimbrado.Data != null)
        {
            var factura = await _context.FacturasVenta.FirstAsync(f => f.Id == resultadoTimbrado.Data.Id, cancellationToken);
            factura.GeneradaPorAutofactura = true;
            factura.VigenciaAutofactura = estado.FechaPedido?.Date.AddDays(_diasVigenciaAutofactura);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return resultadoTimbrado;
    }
}

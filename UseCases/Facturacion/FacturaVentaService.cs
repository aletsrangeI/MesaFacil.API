using Common;
using Domain.Entities;
using DTO.Compras;
using DTO.Facturacion;
using Interface.PAC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;

namespace UseCases.Facturacion;

public interface IFacturaVentaService
{
    Task<Response<FacturaVentaDTO>> TimbrarPedidoAsync(TimbrarPedidoRequestDTO request, CancellationToken cancellationToken = default);
    Task<Response<string>> DescargarXmlAsync(int facturaVentaId);
    Task<Response<byte[]>> DescargarPdfAsync(int facturaVentaId);
    Task<Response<bool>> EnviarCorreoAsync(int facturaVentaId);
    Task<Response<FacturaVentaDTO>> CancelarAsync(int facturaVentaId, CancelarFacturaRequestDTO request);
    Task<Response<BolsaTimbresDTO>> ObtenerBolsaTimbresAsync(int idEmpresa);
    Task<Response<List<FacturaVentaDTO>>> ListarAsync(int idEmpresa, DateTime? fechaInicio, DateTime? fechaFin, string? rfc);
}

/// <summary>
/// Spec 020: orquesta el timbrado fiscal (CFDI 4.0) de un Pedido ya cobrado: arma los
/// conceptos a partir de PedidoDetalle, genera el XML, calcula/sella la cadena original,
/// invoca al IPACTimbradoService configurado por la Empresa (Mock por defecto), descuenta la
/// bolsa de timbres y persiste el resultado. Reutilizado tanto por FacturasVentaController
/// (caja, con JWT) como por AutofacturacionComensalService (portal público QR).
/// </summary>
public class FacturaVentaService : IFacturaVentaService
{
    private readonly ApplicationDbContext _context;
    private readonly IGeneradorXmlCfdi40Service _generadorXml;
    private readonly ISelloDigitalService _selloDigital;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Vigencia por defecto (días naturales) para descargar/reenviar una factura ya emitida
    /// desde el POS. No debe confundirse con la vigencia de AUTOFACTURACIÓN (ver
    /// AutofacturacionComensalService), que aplica sobre pedidos aún NO facturados.
    /// </summary>
    public FacturaVentaService(
        ApplicationDbContext context,
        IGeneradorXmlCfdi40Service generadorXml,
        ISelloDigitalService selloDigital,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _generadorXml = generadorXml;
        _selloDigital = selloDigital;
        _serviceProvider = serviceProvider;
    }

    public async Task<Response<FacturaVentaDTO>> TimbrarPedidoAsync(TimbrarPedidoRequestDTO request, CancellationToken cancellationToken = default)
    {
        var response = new Response<FacturaVentaDTO>();

        if (!RfcHelper.EsRfcValido(request.RfcReceptor, out var rfcError))
        {
            response.isSuccess = false;
            response.Message = rfcError;
            return response;
        }

        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .Include(p => p.Cuentas)
            .FirstOrDefaultAsync(p => p.Id == request.PedidoId, cancellationToken);

        if (pedido == null)
        {
            response.isSuccess = false;
            response.Message = "El pedido especificado no existe.";
            return response;
        }

        // No duplicidad: un PedidoId no puede tener más de una factura activa.
        var facturaExistente = await _context.FacturasVenta
            .FirstOrDefaultAsync(f => f.PedidoId == request.PedidoId && f.EstadoFiscal != EstadoFiscalFactura.Cancelada, cancellationToken);

        if (facturaExistente != null)
        {
            response.isSuccess = false;
            response.Message = $"El pedido ya cuenta con una factura activa (Folio {facturaExistente.Serie}{facturaExistente.Folio}, UUID {facturaExistente.UUID}).";
            return response;
        }

        var cuenta = pedido.Cuentas.FirstOrDefault();
        if (cuenta == null || cuenta.Total <= 0)
        {
            response.isSuccess = false;
            response.Message = "El pedido no tiene una cuenta cobrada; no puede facturarse.";
            return response;
        }

        var configPac = await _context.EmpresaConfiguracionesPAC
            .FirstOrDefaultAsync(c => c.IdEmpresa == pedido.IdEmpresa, cancellationToken);

        if (configPac == null)
        {
            response.isSuccess = false;
            response.Message = "La Empresa no tiene configurado un proveedor PAC (EmpresaConfiguracionPAC).";
            return response;
        }

        var bolsa = await _context.EmpresaBolsasTimbres
            .FirstOrDefaultAsync(b => b.IdEmpresa == pedido.IdEmpresa, cancellationToken);

        if (bolsa == null || bolsa.TimbresDisponibles <= 0)
        {
            response.isSuccess = false;
            response.Message = "Saldo de timbres insuficiente. El cobro del pedido no se ve afectado; recargue su bolsa de timbres para poder facturar.";
            return response;
        }

        // 1) Conceptos a partir de los renglones NO cancelados del pedido.
        var conceptosInput = pedido.Detalles
            .Where(d => !d.Cancelado)
            .Select(d => new ConceptoCfdiInput
            {
                ClaveProdServ = "90101501", // Restaurantes (catálogo c_ClaveProdServ SAT)
                ClaveUnidad = "E48", // Unidad de servicio
                Cantidad = d.Cantidad,
                Descripcion = d.ProductoNombre,
                ValorUnitario = d.PrecioUnitario,
                TasaIva = d.TasaImpuesto > 0 ? d.TasaImpuesto / 100m : 0.16m
            })
            .ToList();

        if (conceptosInput.Count == 0)
        {
            response.isSuccess = false;
            response.Message = "El pedido no tiene renglones facturables (todos están cancelados).";
            return response;
        }

        var conceptosCalculados = _generadorXml.CalcularConceptos(conceptosInput);
        var totales = _generadorXml.CalcularTotales(conceptosCalculados);

        var folio = configPac.FolioSiguiente;

        var factura = new FacturaVenta
        {
            IdEmpresa = pedido.IdEmpresa,
            IdSucursal = pedido.IdSucursal,
            PedidoId = pedido.Id,
            Serie = configPac.SerieFacturacion,
            Folio = folio.ToString(),
            RfcReceptor = request.RfcReceptor.Trim().ToUpperInvariant(),
            NombreReceptor = request.NombreReceptor,
            RegimenFiscalReceptor = request.RegimenFiscalReceptor,
            CodigoPostalReceptor = request.CodigoPostalReceptor,
            UsoCfdi = request.UsoCfdi,
            FormaPago = request.FormaPago,
            MetodoPago = string.IsNullOrWhiteSpace(request.MetodoPago) ? "PUE" : request.MetodoPago,
            Subtotal = totales.Subtotal,
            Descuento = totales.Descuento,
            Iva = totales.Iva,
            Total = totales.Total,
            EstadoFiscal = EstadoFiscalFactura.Pendiente,
            TicketAutofacturaGuid = pedido.Id,
            FechaTimbrado = DateTime.UtcNow
        };

        var xmlSinTimbrar = _generadorXml.GenerarXmlComprobante(configPac, factura, conceptosCalculados);
        var cadenaOriginal = _selloDigital.CalcularCadenaOriginal(configPac, factura);
        var sello = _selloDigital.Sellar(configPac, cadenaOriginal);

        var pacService = ResolverPacService(configPac.ProveedorPAC);

        var timbradoRequest = new TimbrarCfdiRequestDTO
        {
            RfcEmisor = configPac.RfcEmisor,
            RazonSocialEmisor = configPac.RazonSocialEmisor,
            RegimenFiscalEmisor = configPac.RegimenFiscalEmisor,
            LugarExpedicionCP = configPac.LugarExpedicionCP,
            Serie = factura.Serie,
            Folio = factura.Folio,
            FechaEmision = factura.FechaTimbrado ?? DateTime.UtcNow,
            RfcReceptor = factura.RfcReceptor,
            NombreReceptor = factura.NombreReceptor,
            RegimenFiscalReceptor = factura.RegimenFiscalReceptor,
            CodigoPostalReceptor = factura.CodigoPostalReceptor,
            UsoCfdi = factura.UsoCfdi,
            FormaPago = factura.FormaPago,
            MetodoPago = factura.MetodoPago,
            Subtotal = factura.Subtotal,
            Descuento = factura.Descuento,
            Iva = factura.Iva,
            Total = factura.Total,
            XmlSellado = xmlSinTimbrar,
            CadenaOriginalSat = cadenaOriginal,
            SelloDigitalEmisor = sello.SelloDigital,
            NoCertificadoEmisor = sello.NoCertificado,
            PacApiKey = configPac.PacApiKey,
            PacApiSecret = configPac.PacApiSecret,
            EsProduccion = configPac.EsProduccion
        };

        var resultadoTimbrado = await pacService.TimbrarAsync(timbradoRequest, cancellationToken);

        if (!resultadoTimbrado.Exitoso)
        {
            response.isSuccess = false;
            response.Message = $"El PAC ({pacService.NombreProveedor}) rechazó el timbrado: {resultadoTimbrado.MensajeError}";
            return response;
        }

        factura.UUID = resultadoTimbrado.UUID;
        factura.FechaTimbrado = resultadoTimbrado.FechaTimbrado ?? DateTime.UtcNow;
        factura.CadenaOriginalSat = resultadoTimbrado.CadenaOriginalSat ?? cadenaOriginal;
        factura.SelloDigitalSat = resultadoTimbrado.SelloDigitalSat;
        factura.SelloDigitalEmisor = resultadoTimbrado.SelloDigitalEmisor ?? sello.SelloDigital;
        factura.NoCertificadoSat = resultadoTimbrado.NoCertificadoSat;
        factura.XmlSellado = resultadoTimbrado.XmlTimbrado ?? xmlSinTimbrar;
        factura.EstadoFiscal = EstadoFiscalFactura.Vigente;

        factura.Detalles = conceptosCalculados.Select(c => new FacturaVentaDetalle
        {
            ClaveProdServ = c.ClaveProdServ,
            ClaveUnidad = c.ClaveUnidad,
            Cantidad = c.Cantidad,
            Descripcion = c.Descripcion,
            ValorUnitario = c.ValorUnitario,
            Importe = c.Importe,
            BaseIva = c.BaseIva,
            TasaIva = c.TasaIva,
            ImporteIva = c.ImporteIva
        }).ToList();

        _context.FacturasVenta.Add(factura);
        configPac.FolioSiguiente += 1;

        // Descuento de 1 timbre de la bolsa de la empresa (regla de negocio: 1 timbrado = 1 timbre).
        bolsa.TimbresDisponibles -= 1;
        bolsa.TimbresConsumidos += 1;

        await _context.SaveChangesAsync(cancellationToken); // asigna factura.Id

        _context.ConsumosTimbreHistorial.Add(new ConsumoTimbreHistorial
        {
            IdEmpresa = pedido.IdEmpresa,
            IdEmpresaBolsaTimbres = bolsa.Id,
            Tipo = TipoMovimientoTimbre.Consumo,
            Cantidad = 1,
            SaldoResultante = bolsa.TimbresDisponibles,
            IdFacturaVenta = factura.Id,
            Descripcion = $"Timbrado de factura {factura.Serie}{factura.Folio} (Pedido {pedido.Id})."
        });

        await _context.SaveChangesAsync(cancellationToken);

        response.isSuccess = true;
        response.Message = "Factura timbrada exitosamente.";
        response.Data = MapToDto(factura);
        return response;
    }

    public async Task<Response<string>> DescargarXmlAsync(int facturaVentaId)
    {
        var response = new Response<string>();
        var factura = await _context.FacturasVenta.FindAsync(facturaVentaId);

        if (factura == null || string.IsNullOrWhiteSpace(factura.XmlSellado))
        {
            response.isSuccess = false;
            response.Message = "No se encontró el XML timbrado para la factura solicitada.";
            return response;
        }

        response.isSuccess = true;
        response.Data = factura.XmlSellado;
        return response;
    }

    public async Task<Response<byte[]>> DescargarPdfAsync(int facturaVentaId)
    {
        var response = new Response<byte[]>();
        var factura = await _context.FacturasVenta
            .Include(f => f.Detalles)
            .FirstOrDefaultAsync(f => f.Id == facturaVentaId);

        if (factura == null)
        {
            response.isSuccess = false;
            response.Message = "La factura solicitada no existe.";
            return response;
        }

        var configPac = await _context.EmpresaConfiguracionesPAC
            .FirstOrDefaultAsync(c => c.IdEmpresa == factura.IdEmpresa);

        response.isSuccess = true;
        response.Data = FacturaPdfGenerator.Generar(factura, configPac);
        return response;
    }

    public async Task<Response<bool>> EnviarCorreoAsync(int facturaVentaId)
    {
        var response = new Response<bool>();
        var factura = await _context.FacturasVenta.FindAsync(facturaVentaId);

        if (factura == null)
        {
            response.isSuccess = false;
            response.Message = "La factura solicitada no existe.";
            return response;
        }

        // El proyecto no tiene un IEmailService configurado (no hay SMTP/proveedor de correo
        // integrado en ninguna otra parte de la solución). Se registra la intención de envío
        // para no bloquear el flujo de facturación; conectar aquí un IEmailService real es
        // trabajo de infraestructura fuera del alcance de esta spec.
        factura.CorreoEnviadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        response.isSuccess = true;
        response.Message = "Envío de correo registrado (stub: no hay proveedor de correo configurado en el proyecto).";
        response.Data = true;
        return response;
    }

    public async Task<Response<FacturaVentaDTO>> CancelarAsync(int facturaVentaId, CancelarFacturaRequestDTO request)
    {
        var response = new Response<FacturaVentaDTO>();
        var factura = await _context.FacturasVenta.FirstOrDefaultAsync(f => f.Id == facturaVentaId);

        if (factura == null)
        {
            response.isSuccess = false;
            response.Message = "La factura solicitada no existe.";
            return response;
        }

        if (factura.EstadoFiscal == EstadoFiscalFactura.Cancelada)
        {
            response.isSuccess = false;
            response.Message = "La factura ya se encuentra cancelada.";
            return response;
        }

        if (string.IsNullOrWhiteSpace(factura.UUID))
        {
            response.isSuccess = false;
            response.Message = "La factura no tiene UUID (no fue timbrada correctamente); no puede cancelarse ante el SAT.";
            return response;
        }

        var configPac = await _context.EmpresaConfiguracionesPAC
            .FirstOrDefaultAsync(c => c.IdEmpresa == factura.IdEmpresa);

        if (configPac == null)
        {
            response.isSuccess = false;
            response.Message = "La Empresa no tiene configurado un proveedor PAC.";
            return response;
        }

        var pacService = ResolverPacService(configPac.ProveedorPAC);

        var resultado = await pacService.CancelarAsync(new CancelarCfdiRequestDTO
        {
            UUID = factura.UUID,
            RfcEmisor = configPac.RfcEmisor,
            MotivoSat = request.MotivoSat,
            FolioSustitucionUUID = request.FolioSustitucionUUID,
            PacApiKey = configPac.PacApiKey,
            PacApiSecret = configPac.PacApiSecret,
            EsProduccion = configPac.EsProduccion
        });

        if (!resultado.Exitoso)
        {
            response.isSuccess = false;
            response.Message = $"El PAC ({pacService.NombreProveedor}) rechazó la cancelación: {resultado.MensajeError}";
            return response;
        }

        factura.EstadoFiscal = EstadoFiscalFactura.Cancelada;
        factura.MotivoCancelacion = request.MotivoSat;
        factura.FechaCancelacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        response.isSuccess = true;
        response.Message = "Factura cancelada exitosamente.";
        response.Data = MapToDto(factura);
        return response;
    }

    public async Task<Response<BolsaTimbresDTO>> ObtenerBolsaTimbresAsync(int idEmpresa)
    {
        var response = new Response<BolsaTimbresDTO>();

        var bolsa = await _context.EmpresaBolsasTimbres
            .FirstOrDefaultAsync(b => b.IdEmpresa == idEmpresa);

        if (bolsa == null)
        {
            response.isSuccess = false;
            response.Message = "La Empresa no tiene una bolsa de timbres inicializada.";
            return response;
        }

        var historial = await _context.ConsumosTimbreHistorial
            .Where(h => h.IdEmpresa == idEmpresa)
            .OrderByDescending(h => h.FechaMovimiento)
            .Take(50)
            .Select(h => new ConsumoTimbreHistorialDTO
            {
                Tipo = h.Tipo,
                Cantidad = h.Cantidad,
                SaldoResultante = h.SaldoResultante,
                IdFacturaVenta = h.IdFacturaVenta,
                Descripcion = h.Descripcion,
                FechaMovimiento = h.FechaMovimiento
            })
            .ToListAsync();

        response.isSuccess = true;
        response.Data = new BolsaTimbresDTO
        {
            IdEmpresa = idEmpresa,
            TimbresDisponibles = bolsa.TimbresDisponibles,
            TimbresConsumidos = bolsa.TimbresConsumidos,
            UltimaRecargaFecha = bolsa.UltimaRecargaFecha,
            HistorialReciente = historial
        };
        return response;
    }

    private IPACTimbradoService ResolverPacService(string nombreProveedor)
    {
        var servicio = _serviceProvider.GetKeyedService<IPACTimbradoService>(nombreProveedor);
        return servicio ?? _serviceProvider.GetRequiredService<IPACTimbradoService>(); // fallback: MockPac por defecto
    }

    public async Task<Response<List<FacturaVentaDTO>>> ListarAsync(int idEmpresa, DateTime? fechaInicio, DateTime? fechaFin, string? rfc)
    {
        var query = _context.Set<FacturaVenta>().Where(f => f.IdEmpresa == idEmpresa);

        if (fechaInicio.HasValue)
            query = query.Where(f => f.FechaTimbrado >= fechaInicio.Value);
        if (fechaFin.HasValue)
            query = query.Where(f => f.FechaTimbrado <= fechaFin.Value);
        if (!string.IsNullOrWhiteSpace(rfc))
            query = query.Where(f => f.RfcReceptor.Contains(rfc));

        var facturas = await query
            .OrderByDescending(f => f.FechaTimbrado)
            .ToListAsync();

        return new Response<List<FacturaVentaDTO>>
        {
            isSuccess = true,
            Data = facturas.Select(MapToDto).ToList()
        };
    }

    private static FacturaVentaDTO MapToDto(FacturaVenta f) => new()
    {
        Id = f.Id,
        IdEmpresa = f.IdEmpresa,
        IdSucursal = f.IdSucursal,
        PedidoId = f.PedidoId,
        UUID = f.UUID,
        Serie = f.Serie,
        Folio = f.Folio,
        FechaTimbrado = f.FechaTimbrado,
        RfcReceptor = f.RfcReceptor,
        NombreReceptor = f.NombreReceptor,
        UsoCfdi = f.UsoCfdi,
        Subtotal = f.Subtotal,
        Descuento = f.Descuento,
        Iva = f.Iva,
        Total = f.Total,
        EstadoFiscal = f.EstadoFiscal,
        MotivoCancelacion = f.MotivoCancelacion,
        TicketAutofacturaGuid = f.TicketAutofacturaGuid,
        VigenciaAutofactura = f.VigenciaAutofactura
    };
}

using System.Globalization;
using System.Xml.Linq;
using DTO.Compras;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Compras;

public interface ICfdiXmlParserService
{
    Task<CfdiParseResultDTO> ParsearCfdiAsync(string xmlContent);
}

public class CfdiXmlParserService : ICfdiXmlParserService
{
    private readonly ApplicationDbContext? _context;

    public CfdiXmlParserService(ApplicationDbContext? context = null)
    {
        _context = context;
    }

    public async Task<CfdiParseResultDTO> ParsearCfdiAsync(string xmlContent)
    {
        if (string.IsNullOrWhiteSpace(xmlContent))
            throw new ArgumentException("El contenido XML no puede estar vacío.", nameof(xmlContent));

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xmlContent);
        }
        catch (Exception ex)
        {
            throw new FormatException($"El archivo proporcionado no es un XML válido: {ex.Message}", ex);
        }

        var root = doc.Root;
        if (root == null)
            throw new FormatException("El archivo XML no contiene un elemento raíz válido.");

        // Detectar namespace (soporte para CFDI 4.0 y CFDI 3.3)
        XNamespace cfdiNs = root.Name.Namespace;
        if (cfdiNs != "http://www.sat.gob.mx/cfd/4" && cfdiNs != "http://www.sat.gob.mx/cfd/3")
        {
            // Intentar buscar si viene con otro prefijo o default
            var comprobanteEl = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Comprobante");
            if (comprobanteEl != null)
            {
                root = comprobanteEl;
                cfdiNs = root.Name.Namespace;
            }
            else
            {
                throw new FormatException("El documento XML no parece ser un comprobante fiscal CFDI del SAT (falta nodo Comprobante).");
            }
        }

        var result = new CfdiParseResultDTO();

        // 1. Atributos generales del Comprobante
        result.Serie = root.Attribute("Serie")?.Value?.Trim();
        result.Folio = root.Attribute("Folio")?.Value?.Trim() ?? string.Empty;
        result.FormaPago = root.Attribute("FormaPago")?.Value?.Trim();
        result.MetodoPago = root.Attribute("MetodoPago")?.Value?.Trim();
        result.CondicionesDePago = root.Attribute("CondicionesDePago")?.Value?.Trim();
        result.Moneda = root.Attribute("Moneda")?.Value?.Trim() ?? "MXN";

        if (DateTime.TryParse(root.Attribute("Fecha")?.Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
        {
            result.FechaEmision = fecha;
        }
        else
        {
            result.FechaEmision = DateTime.UtcNow;
        }

        result.Subtotal = ParseDecimal(root.Attribute("SubTotal")?.Value);
        result.Descuento = ParseDecimal(root.Attribute("Descuento")?.Value);
        result.Total = ParseDecimal(root.Attribute("Total")?.Value);

        // 2. Timbre Fiscal Digital (UUID)
        XNamespace tfdNs = "http://www.sat.gob.mx/TimbreFiscalDigital";
        var timbre = root.Descendants(tfdNs + "TimbreFiscalDigital").FirstOrDefault()
                     ?? root.Descendants().FirstOrDefault(e => e.Name.LocalName == "TimbreFiscalDigital");

        if (timbre != null)
        {
            result.UUID = timbre.Attribute("UUID")?.Value?.Trim()?.ToUpperInvariant();
        }

        // Si el folio venía vacío en el comprobante, usar los primeros 8 dígitos del UUID
        if (string.IsNullOrWhiteSpace(result.Folio) && !string.IsNullOrWhiteSpace(result.UUID))
        {
            result.Folio = result.UUID.Length >= 8 ? result.UUID[..8] : result.UUID;
        }

        // 3. Emisor
        var emisor = root.Element(cfdiNs + "Emisor")
                     ?? root.Descendants().FirstOrDefault(e => e.Name.LocalName == "Emisor");

        if (emisor != null)
        {
            result.RfcEmisor = emisor.Attribute("Rfc")?.Value?.Trim()?.ToUpperInvariant() ?? string.Empty;
            result.NombreEmisor = emisor.Attribute("Nombre")?.Value?.Trim() ?? string.Empty;
            result.RegimenFiscalEmisor = emisor.Attribute("RegimenFiscal")?.Value?.Trim();
        }

        // 4. Receptor
        var receptor = root.Element(cfdiNs + "Receptor")
                       ?? root.Descendants().FirstOrDefault(e => e.Name.LocalName == "Receptor");

        if (receptor != null)
        {
            result.RfcReceptor = receptor.Attribute("Rfc")?.Value?.Trim()?.ToUpperInvariant() ?? string.Empty;
            result.NombreReceptor = receptor.Attribute("Nombre")?.Value?.Trim() ?? string.Empty;
        }

        // 5. Impuestos globales
        var impuestosGlobales = root.Element(cfdiNs + "Impuestos")
                                ?? root.Descendants().FirstOrDefault(e => e.Name.LocalName == "Impuestos" && e.Parent == root);

        decimal totalIvaGlobal = 0;
        decimal totalIepsGlobal = 0;

        if (impuestosGlobales != null)
        {
            var trasladosGlobales = impuestosGlobales.Descendants().Where(e => e.Name.LocalName == "Traslado");
            foreach (var t in trasladosGlobales)
            {
                var impuesto = t.Attribute("Impuesto")?.Value?.Trim();
                var importe = ParseDecimal(t.Attribute("Importe")?.Value);
                if (impuesto == "002" || impuesto == "IVA")
                    totalIvaGlobal += importe;
                else if (impuesto == "003" || impuesto == "IEPS")
                    totalIepsGlobal += importe;
            }
        }

        // 6. Conceptos
        var conceptosEl = root.Element(cfdiNs + "Conceptos")
                          ?? root.Descendants().FirstOrDefault(e => e.Name.LocalName == "Conceptos");

        var conceptosList = new List<CfdiConceptoDTO>();
        decimal sumIvaConceptos = 0;
        decimal sumIepsConceptos = 0;

        if (conceptosEl != null)
        {
            int renglon = 1;
            foreach (var c in conceptosEl.Elements().Where(e => e.Name.LocalName == "Concepto"))
            {
                var concepto = new CfdiConceptoDTO
                {
                    Renglon = renglon++,
                    ClaveProdServ = c.Attribute("ClaveProdServ")?.Value?.Trim() ?? string.Empty,
                    NoIdentificacion = c.Attribute("NoIdentificacion")?.Value?.Trim(),
                    Cantidad = ParseDecimal(c.Attribute("Cantidad")?.Value),
                    ClaveUnidad = c.Attribute("ClaveUnidad")?.Value?.Trim(),
                    Unidad = c.Attribute("Unidad")?.Value?.Trim(),
                    Descripcion = c.Attribute("Descripcion")?.Value?.Trim() ?? string.Empty,
                    ValorUnitario = ParseDecimal(c.Attribute("ValorUnitario")?.Value),
                    Importe = ParseDecimal(c.Attribute("Importe")?.Value),
                    Descuento = ParseDecimal(c.Attribute("Descuento")?.Value),
                    FactorConversionSugerido = 1.0m
                };

                // Impuestos del concepto
                var trasladosConcepto = c.Descendants().Where(e => e.Name.LocalName == "Traslado");
                foreach (var t in trasladosConcepto)
                {
                    var impuesto = t.Attribute("Impuesto")?.Value?.Trim();
                    var tasa = ParseDecimal(t.Attribute("TasaOCuota")?.Value);
                    var importe = ParseDecimal(t.Attribute("Importe")?.Value);

                    if (impuesto == "002" || impuesto == "IVA")
                    {
                        concepto.TasaIVA = tasa;
                        concepto.ImporteIVA = importe;
                        sumIvaConceptos += importe;
                    }
                    else if (impuesto == "003" || impuesto == "IEPS")
                    {
                        concepto.TasaIEPS = tasa;
                        concepto.ImporteIEPS = importe;
                        sumIepsConceptos += importe;
                    }
                }

                concepto.ImporteTotal = concepto.Importe - concepto.Descuento + concepto.ImporteIVA + concepto.ImporteIEPS;
                conceptosList.Add(concepto);
            }
        }

        result.Conceptos = conceptosList;
        result.TotalIVA = totalIvaGlobal > 0 ? totalIvaGlobal : sumIvaConceptos;
        result.TotalIEPS = totalIepsGlobal > 0 ? totalIepsGlobal : sumIepsConceptos;

        // 7. Validaciones cruzadas contra base de datos
        if (_context != null)
        {
            // 7.1 Verificar duplicidad de UUID
            if (!string.IsNullOrWhiteSpace(result.UUID))
            {
                var facturaExistente = await _context.ComprasFactura
                    .Where(cf => cf.UUID == result.UUID && cf.Estado != "Cancelada")
                    .Select(cf => new { cf.Id, cf.Estado, cf.Folio, cf.Serie })
                    .FirstOrDefaultAsync();

                if (facturaExistente != null)
                {
                    result.FacturaYaExiste = true;
                    result.FacturaExistenteId = facturaExistente.Id;
                    result.FacturaExistenteEstado = facturaExistente.Estado;
                    result.MensajeValidacion = $"ADVERTENCIA: La factura con UUID {result.UUID} ya fue registrada previamente (Folio: {facturaExistente.Serie}{facturaExistente.Folio}, Estado: {facturaExistente.Estado}).";
                }
            }

            // 7.2 Buscar si el proveedor ya está registrado por RFC
            if (!string.IsNullOrWhiteSpace(result.RfcEmisor))
            {
                var proveedorExistente = await _context.Proveedores
                    .Where(p => p.RFC == result.RfcEmisor && p.IsActive)
                    .Select(p => new { p.Id, p.RazonSocial, p.DiasCredito })
                    .FirstOrDefaultAsync();

                if (proveedorExistente != null)
                {
                    result.ProveedorExistenteId = proveedorExistente.Id;
                    result.ProveedorExistenteNombre = proveedorExistente.RazonSocial;
                    result.ProveedorExistenteDiasCredito = proveedorExistente.DiasCredito;
                    result.EsProveedorNuevo = false;
                }
                else
                {
                    result.EsProveedorNuevo = true;
                }
            }

            // 7.3 Empatado y Mapeo Inteligente Concepto SAT ➔ Insumo
            await AutoMapearConceptosAsync(result);
        }

        return result;
    }

    private async Task AutoMapearConceptosAsync(CfdiParseResultDTO result)
    {
        if (result.Conceptos.Count == 0) return;

        // Obtener mapeos previos del proveedor si existe
        List<Domain.Entities.MapeoInsumoProveedor> mapeosProveedor = new();
        if (result.ProveedorExistenteId.HasValue)
        {
            mapeosProveedor = await _context.MapeosInsumoProveedor
                .Include(m => m.Insumo)
                .ThenInclude(i => i!.UnidadMedidaBase)
                .Where(m => m.IdProveedor == result.ProveedorExistenteId.Value)
                .ToListAsync();
        }

        // Catálogo de insumos activos para empates por coincidencia textual o código
        var insumosActivos = await _context.Insumos
            .Include(i => i.UnidadMedidaBase)
            .Where(i => i.IsActive)
            .ToListAsync();

        foreach (var concepto in result.Conceptos)
        {
            // 1) Intentar coincidencia exacta de mapeo previo por ClaveProdServ y Descripción
            var mapeoPrevio = mapeosProveedor.FirstOrDefault(m =>
                m.ClaveProdServ == concepto.ClaveProdServ &&
                string.Equals(m.DescripcionSAT.Trim(), concepto.Descripcion.Trim(), StringComparison.OrdinalIgnoreCase));

            // Si no coincide exactamente la descripción, buscar por ClaveProdServ si solo hay un mapeo
            if (mapeoPrevio == null && !string.IsNullOrWhiteSpace(concepto.ClaveProdServ))
            {
                var porClave = mapeosProveedor.Where(m => m.ClaveProdServ == concepto.ClaveProdServ).ToList();
                if (porClave.Count == 1)
                {
                    mapeoPrevio = porClave[0];
                }
            }

            if (mapeoPrevio != null && mapeoPrevio.Insumo != null)
            {
                concepto.IdInsumoSugerido = mapeoPrevio.IdInsumo;
                concepto.InsumoCodigoSugerido = mapeoPrevio.Insumo.Codigo;
                concepto.InsumoNombreSugerido = mapeoPrevio.Insumo.Nombre;
                concepto.UnidadMedidaBaseSugerida = mapeoPrevio.Insumo.UnidadMedidaBase?.Codigo ?? "PZA";
                concepto.FactorConversionSugerido = mapeoPrevio.FactorConversion > 0 ? mapeoPrevio.FactorConversion : 1.0m;
                concepto.SugeridoPorMapeo = true;
                concepto.IdMapeoExistente = mapeoPrevio.Id;
                continue;
            }

            // 2) Coincidencia por texto en catálogo de insumos (nombre o código)
            var descNorm = concepto.Descripcion.Trim().ToLowerInvariant();
            var insumoMatch = insumosActivos.FirstOrDefault(i =>
                string.Equals(i.Nombre.Trim(), concepto.Descripcion.Trim(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(i.Codigo.Trim(), concepto.NoIdentificacion?.Trim(), StringComparison.OrdinalIgnoreCase) ||
                descNorm.Contains(i.Nombre.Trim().ToLowerInvariant()));

            if (insumoMatch != null)
            {
                concepto.IdInsumoSugerido = insumoMatch.Id;
                concepto.InsumoCodigoSugerido = insumoMatch.Codigo;
                concepto.InsumoNombreSugerido = insumoMatch.Nombre;
                concepto.UnidadMedidaBaseSugerida = insumoMatch.UnidadMedidaBase?.Codigo ?? "PZA";
                concepto.FactorConversionSugerido = 1.0m;
                concepto.SugeridoPorMapeo = false;
            }
        }
    }

    private static decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
            return val;
        return 0;
    }
}

namespace DTO.Facturacion;

/// <summary>
/// Spec 020: datos necesarios para que un IPACTimbradoService selle y timbre un CFDI 4.0.
/// Es agnóstico del proveedor (Mock, Finkok, Facturama, SW Sapien).
/// </summary>
public class TimbrarCfdiRequestDTO
{
    public string RfcEmisor { get; set; } = string.Empty;
    public string RazonSocialEmisor { get; set; } = string.Empty;
    public string RegimenFiscalEmisor { get; set; } = string.Empty;
    public string LugarExpedicionCP { get; set; } = string.Empty;

    public string Serie { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }

    public string RfcReceptor { get; set; } = string.Empty;
    public string NombreReceptor { get; set; } = string.Empty;
    public string RegimenFiscalReceptor { get; set; } = string.Empty;
    public string CodigoPostalReceptor { get; set; } = string.Empty;
    public string UsoCfdi { get; set; } = string.Empty;

    public string FormaPago { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = "PUE";

    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }

    /// <summary>
    /// XML del comprobante (Comprobante+Conceptos+Impuestos) ya generado y sellado con el
    /// certificado del emisor, PENDIENTE de timbrar (sin nodo TimbreFiscalDigital).
    /// </summary>
    public string XmlSellado { get; set; } = string.Empty;

    /// <summary>
    /// Cadena original del comprobante (previa al timbrado), calculada por SelloDigitalService.
    /// </summary>
    public string CadenaOriginalSat { get; set; } = string.Empty;

    public string? SelloDigitalEmisor { get; set; }

    public string? NoCertificadoEmisor { get; set; }

    /// <summary>
    /// Credenciales del PAC configuradas para la Empresa (nulas para MockPac).
    /// </summary>
    public string? PacApiKey { get; set; }
    public string? PacApiSecret { get; set; }
    public bool EsProduccion { get; set; }
}

public class TimbradoResultadoDTO
{
    public bool Exitoso { get; set; }
    public string? MensajeError { get; set; }

    public string? UUID { get; set; }
    public DateTime? FechaTimbrado { get; set; }
    public string? SelloDigitalSat { get; set; }
    public string? SelloDigitalEmisor { get; set; }
    public string? NoCertificadoSat { get; set; }
    public string? CadenaOriginalSat { get; set; }

    /// <summary>
    /// XML final con el nodo TimbreFiscalDigital insertado por el PAC.
    /// </summary>
    public string? XmlTimbrado { get; set; }
}

public class CancelarCfdiRequestDTO
{
    public string UUID { get; set; } = string.Empty;
    public string RfcEmisor { get; set; } = string.Empty;

    /// <summary>
    /// Clave del catálogo c_MotivoCancelacion SAT: 01 (con relación), 02 (error con relación),
    /// 03 (no se llevó a cabo), 04 (operación nominativa relacionada en factura global).
    /// </summary>
    public string MotivoSat { get; set; } = string.Empty;

    /// <summary>
    /// UUID del CFDI que sustituye a este (obligatorio solo para motivo 01).
    /// </summary>
    public string? FolioSustitucionUUID { get; set; }

    public string? PacApiKey { get; set; }
    public string? PacApiSecret { get; set; }
    public bool EsProduccion { get; set; }
}

public class CancelarCfdiResultadoDTO
{
    public bool Exitoso { get; set; }
    public string? MensajeError { get; set; }

    /// <summary>
    /// Cancelado | EnProceso (cuando el SAT solicita aceptación del receptor)
    /// </summary>
    public string? EstatusCancelacion { get; set; }
}

public class ConsultarSaldoResultadoDTO
{
    public bool Exitoso { get; set; }
    public string? MensajeError { get; set; }

    /// <summary>
    /// Timbres disponibles reportados por el PAC (nulo si el proveedor no expone este dato,
    /// como ocurre con MockPac, cuyo saldo se controla localmente en EmpresaBolsaTimbres).
    /// </summary>
    public int? TimbresDisponibles { get; set; }
}

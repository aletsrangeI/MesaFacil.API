namespace DTO.Facturacion;

public class TimbrarPedidoRequestDTO
{
    public Guid PedidoId { get; set; }
    public string RfcReceptor { get; set; } = string.Empty;
    public string NombreReceptor { get; set; } = string.Empty;
    public string RegimenFiscalReceptor { get; set; } = string.Empty;
    public string CodigoPostalReceptor { get; set; } = string.Empty;
    public string UsoCfdi { get; set; } = string.Empty;
    public string FormaPago { get; set; } = string.Empty;
    public string MetodoPago { get; set; } = "PUE";
    public string? CorreoReceptor { get; set; }
}

public class FacturaVentaDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public Guid PedidoId { get; set; }
    public string? UUID { get; set; }
    public string Serie { get; set; } = string.Empty;
    public string Folio { get; set; } = string.Empty;
    public DateTime? FechaTimbrado { get; set; }
    public string RfcReceptor { get; set; } = string.Empty;
    public string NombreReceptor { get; set; } = string.Empty;
    public string UsoCfdi { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    public string EstadoFiscal { get; set; } = string.Empty;
    public string? MotivoCancelacion { get; set; }
    public Guid TicketAutofacturaGuid { get; set; }
    public DateTime? VigenciaAutofactura { get; set; }
}

public class CancelarFacturaRequestDTO
{
    public string MotivoSat { get; set; } = string.Empty;
    public string? FolioSustitucionUUID { get; set; }
}

public class ConsumoTimbreHistorialDTO
{
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int SaldoResultante { get; set; }
    public int? IdFacturaVenta { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaMovimiento { get; set; }
}

public class BolsaTimbresDTO
{
    public int IdEmpresa { get; set; }
    public int TimbresDisponibles { get; set; }
    public int TimbresConsumidos { get; set; }
    public DateTime? UltimaRecargaFecha { get; set; }
    public List<ConsumoTimbreHistorialDTO> HistorialReciente { get; set; } = new();
}

public class ValidarTicketResultDTO
{
    public bool ExistePedido { get; set; }
    public bool PedidoPagado { get; set; }
    public bool YaFacturado { get; set; }
    public bool Vigente { get; set; }
    public decimal Total { get; set; }
    public DateTime? FechaPedido { get; set; }
    public Guid TicketGuid { get; set; }
    public string? MensajeError { get; set; }
}

public class GenerarFacturaAutofacturaRequestDTO
{
    public Guid TicketGuid { get; set; }
    public string RfcReceptor { get; set; } = string.Empty;
    public string NombreReceptor { get; set; } = string.Empty;
    public string RegimenFiscalReceptor { get; set; } = string.Empty;
    public string CodigoPostalReceptor { get; set; } = string.Empty;
    public string UsoCfdi { get; set; } = string.Empty;
    public string? CorreoReceptor { get; set; }
}

namespace DTO.CxP;

public class CuentaPorPagarItemDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public string SucursalNombre { get; set; } = string.Empty;
    public int IdProveedor { get; set; }
    public string ProveedorRFC { get; set; } = string.Empty;
    public string ProveedorRazonSocial { get; set; } = string.Empty;
    public string? ProveedorNombreComercial { get; set; }
    public int? IdCompraFactura { get; set; }
    public string? FacturaUUID { get; set; }
    public string? FacturaSerie { get; set; }
    public string? FacturaFolio { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal SaldoInsoluto { get; set; }
    public decimal TotalAbonado => MontoTotal - SaldoInsoluto;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public int DiasCredito { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string Semaforo { get; set; } = "AlCorriente"; // "Vencida", "PorVencer", "AlCorriente"
    public int DiasParaVencer { get; set; }
    public string? Observaciones { get; set; }
    public int CantidadAbonos { get; set; }
}

public class PagoCuentaPorPagarDTO
{
    public int Id { get; set; }
    public int IdCuentaPorPagar { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public int IdMetodoPago { get; set; }
    public string MetodoPagoNombre { get; set; } = string.Empty;
    public int? IdMovimientoCaja { get; set; }
    public string? TurnoUsuario { get; set; }
    public string? ReferenciaBancaria { get; set; }
    public string? ComprobanteUrl { get; set; }
    public int? IdUsuario { get; set; }
    public string? UsuarioNombre { get; set; }
    public string? Observaciones { get; set; }
}

public class CuentaPorPagarDTO : CuentaPorPagarItemDTO
{
    public List<PagoCuentaPorPagarDTO> Pagos { get; set; } = new();
}

public class RegistrarPagoCxPDTO
{
    public int IdCuentaPorPagar { get; set; }
    public decimal Monto { get; set; }
    public DateTime? FechaPago { get; set; }
    public int IdMetodoPago { get; set; }
    public bool PagarDesdeCajaChica { get; set; }
    public int? IdTurno { get; set; }
    public string? ReferenciaBancaria { get; set; }
    public string? Observaciones { get; set; }
}

public class FiltroCxPDTO
{
    public int? IdSucursal { get; set; }
    public int? IdProveedor { get; set; }
    public string? Estado { get; set; }
    public string? Semaforo { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Buscar { get; set; }
}

public class ResumenKpisCxPDTO
{
    public decimal TotalPorPagar { get; set; }
    public decimal TotalVencido { get; set; }
    public decimal TotalVenceEstaSemana { get; set; }
    public decimal TotalPagadoMes { get; set; }
    public int CantidadPendientes { get; set; }
    public int CantidadVencidas { get; set; }
}

public class AntiguedadBucketDTO
{
    public decimal AlCorriente { get; set; }
    public decimal De1A15 { get; set; }
    public decimal De16A30 { get; set; }
    public decimal De31A60 { get; set; }
    public decimal MasDe60 { get; set; }
    public decimal Total => AlCorriente + De1A15 + De16A30 + De31A60 + MasDe60;
}

public class AntiguedadProveedorDTO : AntiguedadBucketDTO
{
    public int IdProveedor { get; set; }
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
}

public class ReporteAntiguedadSaldosDTO
{
    public AntiguedadBucketDTO Totales { get; set; } = new();
    public List<AntiguedadProveedorDTO> Proveedores { get; set; } = new();
}

public class MovimientoEstadoCuentaDTO
{
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty; // "Factura", "Abono"
    public string Referencia { get; set; } = string.Empty;
    public decimal Cargo { get; set; }
    public decimal Abono { get; set; }
    public decimal SaldoAcumulado { get; set; }
    public string? MetodoPago { get; set; }
    public string? Observaciones { get; set; }
}

public class EstadoCuentaProveedorDTO
{
    public int IdProveedor { get; set; }
    public string RFC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public int DiasCredito { get; set; }
    public decimal SaldoTotalPendiente { get; set; }
    public decimal TotalCompradoCredito { get; set; }
    public decimal TotalAbonado { get; set; }
    public List<MovimientoEstadoCuentaDTO> Movimientos { get; set; } = new();
}

public class TurnoActivoDTO
{
    public int IdTurno { get; set; }
    public int IdSucursal { get; set; }
    public string SucursalNombre { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime Apertura { get; set; }
    public decimal CajaInicial { get; set; }
}

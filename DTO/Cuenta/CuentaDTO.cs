namespace DTO.Cuenta;

public class CuentaDTO
{
    public int Id { get; set; }
    public Guid IdPedido { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DescuentoTotal { get; set; }
    public decimal CargoServicio { get; set; }
    public decimal ImpuestoTotal { get; set; }
    public decimal Total { get; set; }
    public DateTime CreadaEn { get; set; }
    public int IdEstadoCuenta { get; set; }

    // Propiedades calculadas para Split Bill
    public decimal TotalPagado { get; set; }
    public decimal SaldoRestante { get; set; }
    public List<PagoResumenDTO> PagosRealizados { get; set; } = new();

    // Propiedades para Impresión Térmica y Auditoría de Descuentos (Spec 028)
    public decimal PorcentajeDescuento { get; set; }
    public string? MotivoDescuento { get; set; }
    public string? AutorizadoPor { get; set; }
    public string? MesaNombre { get; set; }
    public string? SucursalNombre { get; set; }
    public List<CuentaItemDTO> Items { get; set; } = new();
}

public class CuentaItemDTO
{
    public string ProductoNombre { get; set; } = string.Empty;
    public string? VarianteNombre { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total => Cantidad * PrecioUnitario;
    public List<string> Modificadores { get; set; } = new();
}

public class PagoResumenDTO
{
    public Guid Id { get; set; }
    public decimal Monto { get; set; }
    public decimal Propina { get; set; }
    public string? MetodoDePago { get; set; }
    public DateTime PagadoEn { get; set; }
}

public class AplicarDescuentoCuentaDTO
{
    public int IdCuenta { get; set; }
    public decimal PorcentajeDescuento { get; set; }
    public decimal MontoDescuento { get; set; }
    public string? MotivoDescuento { get; set; }
    public string? SupervisorAuthToken { get; set; }
}
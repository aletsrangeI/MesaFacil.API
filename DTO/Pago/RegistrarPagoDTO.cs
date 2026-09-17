namespace DTO.Pago;

public class RegistrarPagoDTO
{
    public int IdCuenta { get; set; }
    public decimal Monto { get; set; }
    public int IdMetodoDePago { get; set; }
    public string? Referencia { get; set; }
    public decimal Propina { get; set; }

    // Spec 028: Candado de supervisor para descuentos en POS
    public decimal PorcentajeDescuento { get; set; } = 0;
    public decimal MontoDescuento { get; set; } = 0;
    public string? MotivoDescuento { get; set; }
    public string? SupervisorAuthToken { get; set; }
}
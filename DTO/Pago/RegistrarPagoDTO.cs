namespace DTO.Pago;

public class RegistrarPagoDTO
{
    public int IdCuenta { get; set; }
    public decimal Monto { get; set; }
    public int IdMetodoDePago { get; set; }
    public string? Referencia { get; set; }
    public decimal Propina { get; set; }
}
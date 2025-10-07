namespace DTO.CorteCaja;

public class CorteCajaDTO
{
    public int Id { get; set; }
    public int? IdTurno { get; set; }
    public int? IdSucursal { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal TotalVentas { get; set; }
    public decimal TotalPagos { get; set; }
    public decimal TotalEfectivo { get; set; }
    public decimal TotalTarjeta { get; set; }
    public decimal TotalEgresos { get; set; }
    public decimal CajaEsperada { get; set; }
    public decimal Declarado { get; set; }
    public decimal Diferencia { get; set; }
}
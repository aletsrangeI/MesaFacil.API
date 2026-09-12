namespace DTO.CorteCaja;

public class ResumenCorteDTO
{
    public int? IdTurno { get; set; }
    public int IdSucursal { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal CajaInicial { get; set; }
    public decimal TotalVentas { get; set; }
    public decimal TotalPagos { get; set; }
    public decimal TotalEfectivo { get; set; }
    public decimal TotalTarjeta { get; set; }
    public decimal TotalPlataformas { get; set; }
    public decimal TotalOtros { get; set; }
    public decimal TotalPropinas { get; set; }
    public decimal TotalPropinasTarjeta { get; set; }
    public decimal TotalPropinasEfectivo { get; set; }
    public decimal TotalIngresos { get; set; }
    public decimal TotalEgresos { get; set; }
    public decimal CajaEsperada { get; set; }
    public int CantidadCuentasPagadas { get; set; }
}

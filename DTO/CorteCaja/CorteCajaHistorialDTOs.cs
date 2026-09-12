namespace DTO.CorteCaja;

public class CorteCajaHistorialItemDTO
{
    public int Id { get; set; }
    public int? IdTurno { get; set; }
    public int IdSucursal { get; set; }
    public string NombreSucursal { get; set; } = string.Empty;
    public string NombreCajero { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    public decimal CajaInicial { get; set; }
    public decimal TotalVentas { get; set; }
    public decimal TotalPagos { get; set; }
    public decimal TotalEfectivo { get; set; }
    public decimal TotalTarjeta { get; set; }
    public decimal TotalOtros { get; set; }

    public decimal TotalPropinas { get; set; }
    public decimal TotalPropinasTarjeta { get; set; }
    public decimal TotalPropinasEfectivo { get; set; }

    public decimal TotalIngresos { get; set; }
    public decimal TotalEgresos { get; set; }

    public decimal CajaEsperada { get; set; }
    public decimal Declarado { get; set; }
    public decimal Diferencia { get; set; }

    public string? Observaciones { get; set; }
    public int CantidadCuentasPagadas { get; set; }
    public DateTime CreadoEn { get; set; }
}

public class ResumenHistorialCortesDTO
{
    public decimal TotalVentas { get; set; }
    public decimal TotalEfectivo { get; set; }
    public decimal TotalTarjeta { get; set; }
    public decimal TotalPropinasTarjeta { get; set; }
    public decimal DiferenciaNeta { get; set; }
    public int CantidadCortes { get; set; }
    public IEnumerable<CorteCajaHistorialItemDTO> Cortes { get; set; } = new List<CorteCajaHistorialItemDTO>();
}

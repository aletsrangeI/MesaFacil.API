namespace DTO.CorteCaja;

public class RealizarCorteRequestDTO
{
    public int? IdTurno { get; set; }
    public int IdSucursal { get; set; }
    public decimal Declarado { get; set; }
    public string? Observaciones { get; set; }
}

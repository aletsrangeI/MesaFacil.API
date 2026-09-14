namespace DTO.MovimientoCaja;

public class MovimientoCajaItemDTO
{
    public Guid Id { get; set; }
    public int IdTurno { get; set; }
    public int IdSucursal { get; set; }
    public string NombreSucursal { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string? Concepto { get; set; }
    public string? Nota { get; set; }
    public DateTime CreatedAt { get; set; }
}

namespace DTO.MovimientoCaja;

public class MovimientoCajaDTO
{
    public int Id { get; set; }
    public int IdTurno { get; set; }
    public string Tipo { get; set; }
    public decimal Monto { get; set; }
    public string? Nota { get; set; }
}
namespace DTO.Turno;

public class TurnoDTO
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public int IdSucursal { get; set; }
    public DateTime Apertura { get; set; }
    public DateTime? Cierre { get; set; }
    public decimal CajaInicial { get; set; }
    public decimal? CajaFinal { get; set; }
}

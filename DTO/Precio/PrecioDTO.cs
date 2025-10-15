namespace DTO.Precio;

public class PrecioDTO
{
    public int Id { get; set; }
    public int IdVariante { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; }
    public int ImpuestoCatalogId { get; set; }
    public int ImpuestoItemId { get; set; }
    public DateTime? ValidoDesde { get; set; }
    public DateTime? ValidoHasta { get; set; }
    public string? Dias { get; set; }
    public string? Horario { get; set; }
}

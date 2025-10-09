namespace DTO.Pago;

public class PagoDTO
{
    public int Id { get; set; }
    public int IdCuenta { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; }
    public decimal Propina { get; set; }
    public DateTime PagadoEn { get; set; }
    public string? Referencia { get; set; }
    public int? RecibidoPor { get; set; }
    public int MetodoCatalogId { get; set; }
    public int MetodoItemId { get; set; }
}

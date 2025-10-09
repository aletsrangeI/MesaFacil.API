namespace DTO.DescuentoAplicado;

public class DescuentoAplicadoDTO
{
    public int Id { get; set; }
    public int IdCuenta { get; set; }
    public int TipoCatalogId { get; set; }
    public int TipoItemId { get; set; }
    public decimal Valor { get; set; }
    public string? Alcance { get; set; }
    public string? Condiciones { get; set; }
}

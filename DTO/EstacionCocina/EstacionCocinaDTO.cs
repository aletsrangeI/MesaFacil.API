namespace DTO.EstacionCocina;

public class EstacionCocinaDTO
{
    public int Id { get; set; }
    public int IdSucursal { get; set; }
    public string? Nombre { get; set; }
    public int MinutosAmbar { get; set; } = 5;
    public int MinutosRojo { get; set; } = 10;
}

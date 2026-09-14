namespace DTO.Producto;

public class PlatilloVarianteItemDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public decimal PrecioVenta { get; set; }
    public bool EsDefault { get; set; }
}

public class CrearPlatilloCompletoDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Codigo { get; set; }
    public int IdCategoria { get; set; }
    public int IdMenu { get; set; }
    public int? IdEstacionCocina { get; set; }

    public bool TieneVariantes { get; set; } = false;
    public decimal? PrecioVenta { get; set; }
    public List<PlatilloVarianteItemDTO>? Variantes { get; set; }
}

public class CrearPlatilloCompletoResultDTO
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IdVarianteDefault { get; set; }
    public decimal PrecioVentaDefault { get; set; }
}

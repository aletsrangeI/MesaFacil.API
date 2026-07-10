namespace DTO.VarianteProducto;

public class VarianteProductoDTO
{
    public int Id { get; set; }
    public int IdProducto { get; set; }
    public string? Nombre { get; set; }
    public string? Codigo { get; set; }
    public bool EsDefault { get; set; }
    public bool Activo { get; set; }
}

namespace DTO.GrupoModificador;

public class GrupoModificadorDTO
{
    public int Id { get; set; }
    public int IdProducto { get; set; }
    public string? Nombre { get; set; }
    public int MinSeleccion { get; set; }
    public int MaxSeleccion { get; set; }
    public bool Obligatorio { get; set; }
    public bool Activo { get; set; }
}

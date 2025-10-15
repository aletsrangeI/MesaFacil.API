namespace DTO.AccesoRuta;

public class AccesoRutaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string? Descripcion { get; set; }
}

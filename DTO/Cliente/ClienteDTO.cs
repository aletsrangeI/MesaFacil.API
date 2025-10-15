namespace DTO.Cliente;

public class ClienteDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
}
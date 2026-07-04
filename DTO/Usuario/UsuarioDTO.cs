namespace DTO.Usuario;

public class UsuarioDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Correo { get; set; }
    public string? Password { get; set; }
    public int? IdRol { get; set; }
}

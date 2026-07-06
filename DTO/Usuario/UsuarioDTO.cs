using System.Text.Json.Serialization;

namespace DTO.Usuario;

public class UsuarioDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public string? NombreEmpresa { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Correo { get; set; }
    public bool IsActive { get; set; } = true;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Pin { get; set; }
    
    public int? IdRol { get; set; }
    public string? NombreRol { get; set; }
}

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
    public int? IdSucursal { get; set; }
    public string? NombreSucursal { get; set; }

    // Indicadores de Seguridad y Operativos (Spec 024, Spec 025 y Turnos)
    public bool HasPassword { get; set; }
    public bool HasPin { get; set; }
    public bool HasPinSupervisor { get; set; }
    public bool IsPinSupervisorLocked { get; set; }
    public DateTime? PinBloqueadoHasta { get; set; }
    public bool HasOpenTurno { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PinSupervisor { get; set; }

    public bool? DesbloquearPinSupervisor { get; set; }
}

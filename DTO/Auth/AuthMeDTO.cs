namespace DTO.Auth;

public class AuthMeDTO
{
    public required int UsuarioId { get; init; }
    public required int IdEmpresa { get; init; }
    public string? Correo { get; init; }
    public string? Nombre { get; init; }
    public string? SucursalId { get; init; }
    public string? NombreSucursal { get; init; }
    public bool TurnoAbierto { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }
    public required IReadOnlyList<string> Permissions { get; init; }   // AccesoRuta.Key
    public required IReadOnlyList<string> Accesos { get; init; }       // Paths (para menú/guards de ruta)
    public string? PermsVersion { get; init; }                         // versión “oficial” en DB
    public bool PermissionsChanged { get; init; }                      // true si difiere de la del token
}
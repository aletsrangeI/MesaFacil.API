namespace DTO.Auth;

public class UserSessionDTO
{
    public required int UsuarioId { get; init; }
    public required int IdEmpresa { get; init; }
    public required string Correo { get; init; }
    public string? NombreCompleto { get; init; }

    public required IReadOnlyList<string> Roles { get; init; }
    public required IReadOnlyList<string> Accesos { get; init; } // e.g. ["/", "/admin", "/mesero"]
    public string? PermsVersion { get; init; } // opcional: hash/etag de permisos
}
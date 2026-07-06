using Common;
using DTO.Auth;

namespace Interface.UseCases;

public interface IAuthApplication
{
    Task<Response<AuthResponseDTO>> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<Response<AuthResponseDTO>> LoginWithPinAsync(PinLoginRequest request, CancellationToken ct);
}

public sealed record LoginRequest(string UserOrEmail, string Password, int? EmpresaId = null, int? SucursalId = null);
public sealed record PinLoginRequest(string UserOrEmail, string Pin, int? EmpresaId = null, int? SucursalId = null);

public sealed record TokenResult(
    bool Success,
    string? AccessToken,
    DateTime? ExpiresAtUtc,
    string? RefreshToken,
    string? Error,

    // NUEVO: sesión
    int? UsuarioId = null,
    int? IdEmpresa = null,
    string? Correo = null,
    string? NombreCompleto = null,
    IReadOnlyList<string>? Roles = null,
    IReadOnlyList<string>? Accesos = null,
    string? PermsVersion = null
);

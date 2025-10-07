namespace Interface.UseCases;

public interface IAuthApplication
{
    Task<TokenResult> LoginAsync(LoginRequest request, CancellationToken ct);
}

public sealed record LoginRequest(string UserOrEmail, string Password, int? EmpresaId = null, int? SucursalId = null);

public sealed record TokenResult(
    bool Success,
    string? AccessToken,
    DateTime? ExpiresAtUtc,
    string? RefreshToken,
    string? Error);
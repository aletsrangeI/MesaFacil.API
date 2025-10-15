namespace DTO.Auth;

public sealed class TokenDTO
{
    public required string AccessToken { get; init; }
    public required DateTime ExpiresAtUtc { get; init; }
    public string? RefreshToken { get; init; }
}
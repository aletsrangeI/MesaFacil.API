namespace DTO.Auth;

public class AuthResponseDTO
{
    public required TokenDTO Token { get; init; }
    public required UserSessionDTO Session { get; init; }
}
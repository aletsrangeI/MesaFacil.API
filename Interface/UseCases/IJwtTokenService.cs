using System.Security.Claims;

namespace Interface.UseCases;

public interface IJwtTokenService
{
    string GenerateToken(IEnumerable<Claim> claims, DateTime nowUtc, out DateTime expiresAtUtc);
}
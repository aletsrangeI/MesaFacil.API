using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Interface.UseCases;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MesaFacil.API.Modules.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opt;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _opt = options.Value;
    }

    public string GenerateToken(IEnumerable<Claim> claims, DateTime nowUtc, out DateTime expiresAtUtc)
    {
        expiresAtUtc = nowUtc.AddMinutes(_opt.ExpMinutes);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: expiresAtUtc,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
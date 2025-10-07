using Interface.Persistence;
using Interface.UseCases;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace UseCases.Auth;

public class AuthApplication : IAuthApplication
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthApplication(IUsuarioRepository usuarios, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<TokenResult> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        // 1) Buscar usuario por correo (o username si agregas campo)
        var usuario = await _usuarios.GetByCorreoWithRolesAndCredentialsAsync(request.UserOrEmail, ct);
        if (usuario is null || !usuario.Activo)
            return new TokenResult(false, null, null, null, "Usuario no encontrado o inactivo.");

        // 2) Seleccionar credencial de tipo "Password" (según tu catálogo)
        //    Aquí asumo que tu catalogo de credenciales mapea TipoItem.Name == "Password"
        var cred = usuario.Credenciales.FirstOrDefault(); // Ajusta el filtro si manejas múltiples tipos
        if (cred is null)
            return new TokenResult(false, null, null, null, "Usuario sin credenciales.");

        // 3) Verificar hash
        bool ok = _hasher.Verify(request.Password, cred.Hash, cred.Salt);
        if (!ok)
            return new TokenResult(false, null, null, null, "Credenciales inválidas.");

        // 4) Claims base
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo ?? string.Empty),
            new Claim("uid", usuario.Id.ToString()),
            new Claim("empresa_id", usuario.IdEmpresa.ToString()),
            new Claim("nombre", usuario.NombreCompleto ?? string.Empty)
        };

        // 5) Roles
        foreach (var ur in usuario.UsuarioRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, ur.Rol.Nombre));
        }

        // 6) Contexto opcional (Sucursal/Turno)
        if (request.SucursalId.HasValue)
            claims.Add(new Claim("sucursal_id", request.SucursalId.Value.ToString()));

        bool tieneTurnoAbierto = await _usuarios.HasOpenTurnoAsync(usuario.Id, ct);
        claims.Add(new Claim("turno_abierto", tieneTurnoAbierto ? "1" : "0"));

        // 7) Generar JWT
        var nowUtc = DateTime.UtcNow;
        var token = _jwt.GenerateToken(claims, nowUtc, out var expiresAtUtc);

        return new TokenResult(true, token, expiresAtUtc, null, null);
    }
}
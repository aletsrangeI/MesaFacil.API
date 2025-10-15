using Interface.Persistence;
using Interface.UseCases;
using System.Security.Claims;
using Common;
using DTO.Auth;
using Microsoft.IdentityModel.JsonWebTokens;

namespace UseCases.Auth;

public class AuthApplication : IAuthApplication
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;
    private readonly IAppLogger<AuthApplication> _logger;

    public AuthApplication(IUsuarioRepository usuarios, IPasswordHasher hasher, IJwtTokenService jwt, IAppLogger<AuthApplication> logger)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _jwt = jwt;
        _logger = logger;
    }

     public async Task<Response<AuthResponseDTO>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var response = new Response<AuthResponseDTO>();

        try
        {
            // 1) Buscar usuario por correo/usuario con roles y credenciales
            var usuario = await _usuarios.GetByCorreoWithRolesAndCredentialsAsync(request.UserOrEmail, ct);
            if (usuario is null || !usuario.IsActive)
            {
                response.Message = "Usuario no encontrado o inactivo.";
                return response; // isSuccess = false por defecto
            }

            // 2) Seleccionar credencial (password). Ajusta si manejas múltiples tipos
            var cred = usuario.Credenciales.FirstOrDefault();
            if (cred is null)
            {
                response.Message = "Usuario sin credenciales.";
                return response;
            }

            // 3) Verificar hash
            if (!_hasher.Verify(request.Password, cred.Hash, cred.Salt))
            {
                response.Message = "Credenciales inválidas.";
                return response;
            }

            // 4) Claims base
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Correo ?? string.Empty),
                new Claim("uid", usuario.Id.ToString()),
                new Claim("empresa_id", usuario.IdEmpresa.ToString()),
                new Claim("nombre", usuario.NombreCompleto ?? string.Empty)
            };

            // 5) Roles (para claims y para la sesión)
            var roles = usuario.UsuarioRoles
                .Select(ur => ur.Rol.Nombre)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();

            foreach (var roleName in roles)
                claims.Add(new Claim(ClaimTypes.Role, roleName));

            // 6) Contexto opcional
            if (request.SucursalId.HasValue)
                claims.Add(new Claim("sucursal_id", request.SucursalId.Value.ToString()));

            var tieneTurnoAbierto = await _usuarios.HasOpenTurnoAsync(usuario.Id, ct);
            claims.Add(new Claim("turno_abierto", tieneTurnoAbierto ? "1" : "0"));

            // 7) Generar JWT
            var nowUtc = DateTime.UtcNow;
            var accessToken = _jwt.GenerateToken(claims, nowUtc, out var expiresAtUtc);

            // 8) Accesos por roles
            var accesos = await _usuarios.GetAccesoPathsByUsuarioIdAsync(usuario.Id, ct);

            // Garantiza Dashboard "/" para todos por tu regla
            if (!accesos.Contains("/"))
                accesos = accesos.Concat(new[] { "/" }).Distinct().ToList();

            // 9) Construir DTOs
            var tokenDto = new TokenDTO
            {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc,
                RefreshToken = null // genera si aplicas refresh
            };

            var sessionDto = new UserSessionDTO()
            {
                UsuarioId = usuario.Id,
                IdEmpresa = usuario.IdEmpresa,
                Correo = usuario.Correo!,
                NombreCompleto = usuario.NombreCompleto,
                Roles = roles,
                Accesos = accesos,
                PermsVersion = null
            };

            response.Data = new AuthResponseDTO
            {
                Token = tokenDto,
                Session = sessionDto
            };

            response.isSuccess = true;
            response.Message = "Autenticación exitosa.";
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error en LoginAsync");
            response.Message = "No fue posible procesar la autenticación.";
            return response;
        }
    }
}
using Interface.Persistence;
using Interface.UseCases;
using System.Security.Claims;
using Common;
using DTO.Auth;
using Microsoft.IdentityModel.JsonWebTokens;
using Domain.Entities;

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
            // 1) Usuario + roles + credenciales
            var usuario = await _usuarios.GetByCorreoWithRolesAndCredentialsAsync(request.UserOrEmail, ct);
            if (usuario is null || !usuario.IsActive)
            {
                response.Message = "Usuario no encontrado o inactivo.";
                return response;
            }

            // 2) Credencial (PASSWORD)
            var cred = usuario.Credenciales.FirstOrDefault(c => c.IsActive && c.CatCredencial.Descripcion == "PASSWORD");
            if (cred is null)
            {
                response.Message = "Usuario sin contraseña activa.";
                return response;
            }

            // 3) Verificar hash
            if (!_hasher.Verify(request.Password, cred.Hash, cred.Salt))
            {
                response.Message = "Credenciales inválidas.";
                return response;
            }

            var roles = usuario.UsuarioRoles
                .Select(ur => ur.Rol.Nombre)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();

            response.Data = await BuildAuthResponseAsync(usuario, roles, request.SucursalId, ct);
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

    public async Task<Response<AuthResponseDTO>> LoginWithPinAsync(PinLoginRequest request, CancellationToken ct)
    {
        var response = new Response<AuthResponseDTO>();

        try
        {
            // 1) Usuario + roles + credenciales
            Usuario? usuario = null;
            if (request.UsuarioId.HasValue)
            {
                usuario = await _usuarios.GetAsync(request.UsuarioId.Value);
            }
            else if (!string.IsNullOrEmpty(request.UserOrEmail))
            {
                usuario = await _usuarios.GetByCorreoWithRolesAndCredentialsAsync(request.UserOrEmail, ct);
            }

            if (usuario is null || !usuario.IsActive)
            {
                response.Message = "Usuario no encontrado o inactivo.";
                return response;
            }

            // 2) Credencial (PIN)
            var cred = usuario.Credenciales.FirstOrDefault(c => c.IsActive && c.CatCredencial.Descripcion == "PIN");
            if (cred is null)
            {
                response.Message = "Usuario sin PIN activo.";
                return response;
            }

            // 3) Verificar hash
            if (!_hasher.Verify(request.Pin, cred.Hash, cred.Salt))
            {
                response.Message = "Credenciales inválidas.";
                return response;
            }

            var roles = usuario.UsuarioRoles
                .Select(ur => ur.Rol.Nombre)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();

            response.Data = await BuildAuthResponseAsync(usuario, roles, request.SucursalId, ct);
            response.isSuccess = true;
            response.Message = "Autenticación exitosa.";
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error en LoginWithPinAsync");
            response.Message = "No fue posible procesar la autenticación.";
            return response;
        }
    }

    private async Task<AuthResponseDTO> BuildAuthResponseAsync(Usuario usuario, List<string> roles, int? sucursalId, CancellationToken ct)
    {
        int? finalSucursalId = sucursalId ?? usuario.IdSucursal;
        string? nombreSucursal = usuario.Sucursal?.Nombre;

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
        foreach (var roleName in roles)
            claims.Add(new Claim(ClaimTypes.Role, roleName));

        // 6) Contexto de Sucursal
        if (finalSucursalId.HasValue)
        {
            claims.Add(new Claim("sucursal_id", finalSucursalId.Value.ToString()));
            if (!string.IsNullOrEmpty(nombreSucursal))
                claims.Add(new Claim("sucursal_nombre", nombreSucursal));
        }

        var tieneTurnoAbierto = await _usuarios.HasOpenTurnoAsync(usuario.Id, ct);
        claims.Add(new Claim("turno_abierto", tieneTurnoAbierto ? "1" : "0"));

        // 7) Permisos (AccesoRuta.Key) -> claims "perm"
        var permissionKeys = await _usuarios.GetPermissionKeysByUsuarioIdAsync(usuario.Id, ct);
        foreach (var key in permissionKeys.Distinct())
            claims.Add(new Claim("perm", key));
        
        var permsVersion = await _usuarios.GetPermissionsVersionAsync(usuario.Id, ct);
        claims.Add(new Claim("perms_version", permsVersion));

        // 8) Token
        var nowUtc = DateTime.UtcNow;
        var accessToken = _jwt.GenerateToken(claims, nowUtc, out var expiresAtUtc);

        // 9) Accesos (paths) para construir menú/guardas por ruta en el front
        var accesos = await _usuarios.GetAccesoPathsByUsuarioIdAsync(usuario.Id, ct);
        if (!accesos.Contains("/")) // dashboard por convención
            accesos = accesos.Concat(new[] { "/" }).Distinct().ToList();

        // 10) DTOs
        var tokenDto = new TokenDTO
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAtUtc,
            RefreshToken = null
        };

        var sessionDto = new UserSessionDTO
        {
            UsuarioId = usuario.Id,
            IdEmpresa = usuario.IdEmpresa,
            Correo = usuario.Correo!,
            NombreCompleto = usuario.NombreCompleto,
            IdSucursal = finalSucursalId,
            NombreSucursal = nombreSucursal,
            Roles = roles,
            Accesos = accesos,
            PermsVersion = null
        };

        return new AuthResponseDTO
        {
            Token = tokenDto,
            Session = sessionDto
        };
    }

}
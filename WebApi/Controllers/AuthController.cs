using System.Security.Claims;
using Common;
using DTO.Auth;
using Interface.Persistence;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Validator;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthApplication _authApplication;
    private readonly IUsuarioRepository _usuariosRepository;
    private readonly LoginRequestValidator _loginValidator;
    private readonly PinLoginRequestValidator _pinValidator;

    public AuthController(
        IAuthApplication authApplication,
        IUsuarioRepository usuariosRepository,
        LoginRequestValidator loginValidator,
        PinLoginRequestValidator pinValidator)
    {
        _authApplication = authApplication;
        _usuariosRepository = usuariosRepository;
        _loginValidator = loginValidator;
        _pinValidator = pinValidator;
    }

    [AllowAnonymous]
    [HttpPost("login", Name = "Auth_Login")]
    public async Task<ActionResult<Response<AuthResponseDTO>>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var val = await _loginValidator.ValidateAsync(req, ct);
        if (!val.IsValid)
        {
            var bad = new Response<AuthResponseDTO>
            {
                Data = default!,
                isSuccess = false,
                Message = "Solicitud inválida",
                Errors = val.Errors
            };
            return BadRequest(bad);
        }

        var result = await _authApplication.LoginAsync(req, ct);
        if (!result.isSuccess || result.Data is null)
            return Unauthorized(result);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login-pin", Name = "Auth_LoginWithPin")]
    public async Task<ActionResult<Response<AuthResponseDTO>>> LoginWithPin([FromBody] PinLoginRequest req, CancellationToken ct)
    {
        var val = await _pinValidator.ValidateAsync(req, ct);
        if (!val.IsValid)
        {
            var bad = new Response<AuthResponseDTO>
            {
                Data = default!,
                isSuccess = false,
                Message = "Solicitud inválida",
                Errors = val.Errors
            };
            return BadRequest(bad);
        }

        var result = await _authApplication.LoginWithPinAsync(req, ct);
        if (!result.isSuccess || result.Data is null)
            return Unauthorized(result);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("me", Name = "Auth_Me")]
    public async Task<ActionResult<Response<AuthMeDTO>>> Me(CancellationToken ct)
    {
        // Obtenemos el usuario autenticado del HttpContext
        var user = HttpContext.User;
        
        var uidStr = user.FindFirst("uid")?.Value;
        if (!int.TryParse(uidStr, out var uid))
            return Unauthorized();

        int.TryParse(user.FindFirst("empresa_id")?.Value, out var empresaId);
        var tokenPermsVersion = user.FindFirst("perms_version")?.Value ?? "";

        // Claims actuales (rápidos)
        var rolesFromToken = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct()
            .ToArray();

        var permsFromToken = user.Claims
            .Where(c => c.Type == "perm")
            .Select(c => c.Value)
            .Distinct()
            .ToArray();

        // Versión “oficial” en DB (hash o campo persistido)
        var currentPermsVersion = await _usuariosRepository.GetPermissionsVersionAsync(uid, ct) ?? "";

        var permissionsChanged = !string.Equals(tokenPermsVersion, currentPermsVersion, StringComparison.Ordinal);

        IReadOnlyList<string> permissions;
        
        // Obtenemos accesos de DB para mantenerlos al día
        var accesos = await _usuariosRepository.GetAccesoPathsByUsuarioIdAsync(uid, ct);

        if (permissionsChanged)
        {
            // Recalcula desde DB si hubo cambios
            permissions = await _usuariosRepository.GetPermissionKeysByUsuarioIdAsync(uid, ct);
        }
        else
        {
            // Usa lo del token (rápido)
            permissions = permsFromToken;
        }

        var dto = new AuthMeDTO
        {
            UsuarioId = uid,
            IdEmpresa = empresaId,
            Correo = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value,
            Nombre = user.FindFirst("nombre")?.Value ?? user.Identity?.Name,
            SucursalId = user.FindFirst("sucursal_id")?.Value,
            TurnoAbierto = (user.FindFirst("turno_abierto")?.Value ?? "0") == "1",
            Roles = rolesFromToken,
            Permissions = permissions,
            Accesos = accesos,
            PermsVersion = currentPermsVersion,
            PermissionsChanged = permissionsChanged
        };

        return Ok(new Response<AuthMeDTO>
        {
            Data = dto,
            isSuccess = true,
            Message = "OK"
        });
    }
}
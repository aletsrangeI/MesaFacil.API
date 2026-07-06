using System.Security.Claims;
using Common; // Response<T>
using DTO.Auth;
using FluentValidation.Results;
using Interface.Persistence;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Validator;

namespace MesaFacil.API.Modules.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .WithOpenApi();
        
        // POST /api/auth/login
        group.MapPost(
            "/login",
            async Task<Results<
                Ok<Response<AuthResponseDTO>>,
                BadRequest<Response<AuthResponseDTO>>,
                UnauthorizedHttpResult
            >>(
                LoginRequest req,
                IAuthApplication svc,
                LoginRequestValidator validator,
                CancellationToken ct) =>
            {
                var val = await validator.ValidateAsync(req, ct);
                if (!val.IsValid)
                {
                    var bad = new Response<AuthResponseDTO>
                    {
                        Data = default!,
                        isSuccess = false,
                        Message = "Solicitud inválida",
                        Errors = val.Errors
                    };
                    return TypedResults.BadRequest(bad);
                }

                var result = await svc.LoginAsync(req, ct);
                if (!result.isSuccess || result.Data is null)
                    return TypedResults.Unauthorized();

                return TypedResults.Ok(result);
            })
            .WithName("Auth_Login");

        // POST /api/auth/login-pin
        group.MapPost(
            "/login-pin",
            async Task<Results<
                Ok<Response<AuthResponseDTO>>,
                BadRequest<Response<AuthResponseDTO>>,
                UnauthorizedHttpResult
            >>(
                PinLoginRequest req,
                IAuthApplication svc,
                PinLoginRequestValidator validator,
                CancellationToken ct) =>
            {
                var val = await validator.ValidateAsync(req, ct);
                if (!val.IsValid)
                {
                    var bad = new Response<AuthResponseDTO>
                    {
                        Data = default!,
                        isSuccess = false,
                        Message = "Solicitud inválida",
                        Errors = val.Errors
                    };
                    return TypedResults.BadRequest(bad);
                }

                var result = await svc.LoginWithPinAsync(req, ct);
                if (!result.isSuccess || result.Data is null)
                    return TypedResults.Unauthorized();

                return TypedResults.Ok(result);
            })
            .WithName("Auth_LoginWithPin");

        // GET /api/auth/me
        group.MapGet(
            "/me",
            [Authorize] async Task<Results<Ok<Response<AuthMeDTO>>, UnauthorizedHttpResult>>(
                ClaimsPrincipal user,
                IUsuarioRepository usuarios,     // servicio de consultas (lee DB)
                CancellationToken ct) =>
            {
                var uidStr = user.FindFirst("uid")?.Value;
                if (!int.TryParse(uidStr, out var uid))
                    return TypedResults.Unauthorized();

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
                var currentPermsVersion = await usuarios.GetPermissionsVersionAsync(uid, ct) ?? "";

                var permissionsChanged = !string.Equals(tokenPermsVersion, currentPermsVersion, StringComparison.Ordinal);

                IReadOnlyList<string> permissions;
                // Recomiendo obtener accesos (paths) siempre de DB para mantenerlos al día
                var accesos = await usuarios.GetAccesoPathsByUsuarioIdAsync(uid, ct);

                if (permissionsChanged)
                {
                    // Recalcula desde DB si hubo cambios
                    permissions = await usuarios.GetPermissionKeysByUsuarioIdAsync(uid, ct);
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
                    Correo = user.FindFirst(ClaimTypes.Email)?.Value 
                             ?? user.FindFirst("email")?.Value,
                    Nombre = user.FindFirst("nombre")?.Value ?? user.Identity?.Name,
                    SucursalId = user.FindFirst("sucursal_id")?.Value,
                    TurnoAbierto = (user.FindFirst("turno_abierto")?.Value ?? "0") == "1",

                    Roles = rolesFromToken,
                    Permissions = permissions,
                    Accesos = accesos,
                    PermsVersion = currentPermsVersion,
                    PermissionsChanged = permissionsChanged
                };

                return TypedResults.Ok(new Response<AuthMeDTO>
                {
                    Data = dto,
                    isSuccess = true,
                    Message = "OK"
                });
            })
            .WithName("Auth_Me");

        return app;
    }
}

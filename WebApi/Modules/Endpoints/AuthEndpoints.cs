using System.Security.Claims;
using Common; // Response<T>
using DTO.Auth;
using FluentValidation.Results;
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

        // ---------------------------
        // POST /api/auth/login
        // ---------------------------
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
                // 1) Validación
                var val = await validator.ValidateAsync(req, ct);
                if (!val.IsValid)
                {
                    var bad = new Response<AuthResponseDTO>
                    {
                        Data = default!,
                        isSuccess = false,
                        Message = "Solicitud inválida",
                        Errors = val.Errors // FluentValidation errors
                    };
                    return TypedResults.BadRequest(bad);
                }

                // 2) Lógica de autenticación
                var result = await svc.LoginAsync(req, ct);

                // (Tu capa de aplicación ya devuelve Response<AuthResponseDTO>)
                if (!result.isSuccess || result.Data is null)
                {
                    // Puedes devolver 401 vacío (más estándar) o incluir mensaje con 401 JSON.
                    // Manteniendo tu patrón, devuelvo 401 sin cuerpo:
                    return TypedResults.Unauthorized();
                }

                // 3) Éxito
                return TypedResults.Ok(result);
            })
            .WithName("Auth_Login");

        // ---------------------------
        // GET /api/auth/me
        // ---------------------------
        // group.MapGet(
        //     "/me",
        //     [Authorize] async Task<Results<
        //         Ok<Response<UserSessionDTO>>,
        //         UnauthorizedHttpResult
        //     >>(
        //         ClaimsPrincipal user,
        //         IAuthApplication svc,
        //         CancellationToken ct) =>
        //     {
        //         // sub / nameidentifier
        //         var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
        //                   ?? user.FindFirst("sub")?.Value;
        //
        //         if (!int.TryParse(sub, out var usuarioId))
        //         {
        //             return TypedResults.Unauthorized();
        //         }
        //
        //         var session = await svc.GetSessionAsync(usuarioId, ct);
        //         if (session is null)
        //         {
        //             // Si prefieres NotFound, cambia la firma por Results<Ok<...>, Unauthorized, NotFound<...>>
        //             return TypedResults.Unauthorized();
        //         }
        //
        //         var ok = new Response<UserSessionDTO>
        //         {
        //             Data = session,
        //             isSuccess = true,
        //             Message = "Perfil de sesión",
        //             Errors = Array.Empty<ValidationFailure>()
        //         };
        //
        //         return TypedResults.Ok(ok);
        //     })
        //     .WithName("Auth_Me");

        // ---------------------------
        // (Opcional) POST /api/auth/refresh
        // ---------------------------
        // Descomenta si ya tienes refresh en IAuthApplication / IJwtTokenService.
        /*
        group.MapPost(
            "/refresh",
            async Task<Results<
                Ok<Response<TokenDTO>>,
                BadRequest<Response<TokenDTO>>,
                UnauthorizedHttpResult
            >>(
                RefreshRequest req,             // define DTO con RefreshToken
                IAuthApplication svc,
                CancellationToken ct) =>
            {
                var resp = await svc.RefreshAsync(req, ct); // Response<TokenDTO>
                if (!resp.isSuccess || resp.Data is null)
                    return TypedResults.Unauthorized();

                return TypedResults.Ok(resp);
            })
            .WithName("Auth_Refresh");
        */

        return app;
    }
}

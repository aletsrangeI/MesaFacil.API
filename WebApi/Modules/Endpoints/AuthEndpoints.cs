using Common;
using DTO.Auth;
using FluentValidation.Results;
using Interface.UseCases;
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

        group.MapPost(
                "/login",
                async Task<Results<
                        Ok<Response<TokenDTO>>,
                        BadRequest<Response<TokenDTO>>,
                        UnauthorizedHttpResult>> // ✅ UnauthorizedHttpResult, no genérico
                    (LoginRequest req, IAuthApplication svc, LoginRequestValidator v, CancellationToken ct) =>
                {
                    // 1) Validación
                    var val = await v.ValidateAsync(req, ct);
                    if (!val.IsValid)
                    {
                        var resp = new Response<TokenDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Solicitud inválida",
                            Errors = val.Errors
                        };
                        return TypedResults.BadRequest(resp);
                    }

                    // 2) Lógica de autenticación
                    var result = await svc.LoginAsync(req, ct);
                    if (!result.Success)
                    {
                        // Podemos devolver el cuerpo JSON explícitamente con Results.Json
                        var errorResponse = new Response<TokenDTO>
                        {
                            Data = default!,
                            isSuccess = false,
                            Message = "Credenciales inválidas",
                            Errors = Array.Empty<ValidationFailure>()
                        };

                        // 401 sin tipo genérico
                        return TypedResults.Unauthorized();
                    }

                    // 3) Éxito
                    var dto = new TokenDTO
                    {
                        AccessToken = result.AccessToken!,
                        ExpiresAtUtc = result.ExpiresAtUtc!.Value,
                        RefreshToken = result.RefreshToken
                    };

                    var ok = new Response<TokenDTO>
                    {
                        Data = dto,
                        isSuccess = true,
                        Message = "Login exitoso",
                        Errors = Array.Empty<ValidationFailure>()
                    };

                    return TypedResults.Ok(ok);
                })
            .WithName("Auth_Login");

        return app;
    }
}
using DTO.Auth;
using Interface.UseCases;
using Microsoft.AspNetCore.Http;
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

        group.MapPost("/login",
                async (LoginRequest req, IAuthApplication svc, LoginRequestValidator v, CancellationToken ct) =>
                {
                    var val = await v.ValidateAsync(req, ct);
                    if (!val.IsValid)
                    {
                        var errors = string.Join("; ", val.Errors.Select(e => e.ErrorMessage));
                        return Results.BadRequest(new { message = errors });
                    }

                    var result = await svc.LoginAsync(req, ct);
                    if (!result.Success)
                        return Results.Unauthorized();

                    var dto = new TokenDTO
                    {
                        AccessToken = result.AccessToken!,
                        ExpiresAtUtc = result.ExpiresAtUtc!.Value,
                        RefreshToken = result.RefreshToken
                    };

                    return Results.Ok(dto);
                })
            .WithName("Auth_Login");

        return app;
    }
}
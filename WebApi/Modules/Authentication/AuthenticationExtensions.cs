using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.Security;

namespace MesaFacil.API.Modules.Authentication;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Registra la autenticación JWT para MesaFacil consumiendo la sección "Jwt" de appsettings.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind de opciones (ya sea aquí o también con services.Configure<JwtOptions>(...))
        var section = configuration.GetSection(JwtOptions.SectionName);
        var jwt = section.Get<JwtOptions>();
        if (jwt is null || string.IsNullOrWhiteSpace(jwt.SigningKey))
            throw new InvalidOperationException("JWT configuration is missing or invalid. Check appsettings: \"Jwt\" section.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));

        // Importante: mapeamos NameClaimType/RoleClaimType para que ClaimsPrincipal funcione como esperamos.
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            // Para que User.Identity.Name sea el "sub" (IdUsuario) y los roles vengan de ClaimTypes.Role
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // ponlo true en producción detrás de HTTPS
                options.SaveToken = false;
                options.TokenValidationParameters = tokenValidationParams;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Aquí puedes leer claims ya validadas
                        // sub = IdUsuario (lo generamos en el UseCase)
                        var userId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
                        var empresaId = context.Principal?.FindFirstValue("empresa_id");
                        var turnoAbierto = context.Principal?.FindFirstValue("turno_abierto"); // "1" o "0"

                        // Si quisieras aplicar lógica adicional (p.ej. negar si usuario está bloqueado),
                        // podrías inyectar un servicio con context.HttpContext.RequestServices y consultar BD.
                        // En este flujo nuevo no resolvemos usuario aquí; sólo validamos claims.

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}

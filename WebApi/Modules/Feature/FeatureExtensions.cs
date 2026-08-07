using System.Text.Json.Serialization;

namespace MesaFacil.API.Modules.Feature;

public static class FeatureExtensions
{
    public static IServiceCollection AddFeature(this IServiceCollection services, IConfiguration configuration)
    {
        string myPolicy = "policyMesaFacil";

        services.AddCors(options =>
        {
            options.AddPolicy("policyMesaFacil", p => p
                    .WithOrigins(
                        "http://100.110.215.58:8081",
                        "http://100.110.215.58",
                        "https://orionsys.net",
                        "http://localhost:5173"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()          // solo si usas cookies/withCredentials
            );
        });


        return services;
    }
}
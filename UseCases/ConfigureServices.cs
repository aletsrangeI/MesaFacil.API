using System.Reflection;
using Interface.UseCases;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Catalogs;
using Validator;

namespace UseCases;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            /* opcional: config extra */
        }, Assembly.GetExecutingAssembly());
        
        
        //inyeccion de dependencias 
        services.AddScoped<ICatalogApplication, CatalogApplication>();

        //validators
        services.AddTransient<CatalogDTOValidator>();

        return services;
    }
}
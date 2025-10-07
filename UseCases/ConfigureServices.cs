using System.Reflection;
using Interface.UseCases;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Areas;
using UseCases.CatalogItems;
using UseCases.Catalogs;
using UseCases.CategoriaMenus;
using UseCases.Clientes;
using UseCases.CorteCajas;
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
        services.AddScoped<IAreaApplication, AreaApplication>();
        services.AddScoped<ICategoriaMenuApplication, CategoriaMenuApplication>();
        services.AddScoped<ICatalogItemApplication, CatalogItemApplication>();
        services.AddScoped<IClienteApplication, ClienteApplication>();
        services.AddScoped<ICorteCajaApplication, CorteCajaApplication>();

        //validators
        services.AddTransient<CatalogDTOValidator>();
        services.AddTransient<AreaDTOValidator>();
        services.AddTransient<CatalogItemDTOValidator>();
        services.AddTransient<CategoriaMenuDTOValidator>();
        services.AddTransient<ClienteDTOValidator>();
        services.AddTransient<CorteCajaDTOValidator>();

        return services;
    }
}
using System.Reflection;
using Interface.UseCases;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Areas;
using UseCases.Auth;
using UseCases.CatalogItems;
using UseCases.Catalogs;
using UseCases.CategoriaMenus;
using UseCases.Clientes;
using UseCases.CorteCajas;
using UseCases.Credenciales;
using UseCases.Cuentas;
using UseCases.DescuentosAplicados;
using UseCases.DetalleCuentas;
using Validator;
// Si registras validadores aquí, mantén solo los que no dependen de WebApi

namespace UseCases;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            /* opcional: config extra */
        }, Assembly.GetExecutingAssembly());

        // Casos de uso (Application Layer)
        services.AddScoped<ICatalogApplication, CatalogApplication>();
        services.AddScoped<IAreaApplication, AreaApplication>();
        services.AddScoped<ICategoriaMenuApplication, CategoriaMenuApplication>();
        services.AddScoped<ICatalogItemApplication, CatalogItemApplication>();
        services.AddScoped<IClienteApplication, ClienteApplication>();
        services.AddScoped<ICorteCajaApplication, CorteCajaApplication>();
        services.AddScoped<ICredencialApplication, CredencialApplication>();
        services.AddScoped<IAuthApplication, AuthApplication>();
        services.AddScoped<ICuentaApplication, CuentaApplication>();
        services.AddScoped<IDescuentoAplicadoApplication, DescuentoAplicadoApplication>();
        services.AddScoped<IDetalleCuentaApplication, DetalleCuentaApplication>();

        // Validadores (si quieres mantenerlos aquí está bien; no dependen de WebApi)
        services.AddTransient<CatalogDTOValidator>();
        services.AddTransient<AreaDTOValidator>();
        services.AddTransient<CatalogItemDTOValidator>();
        services.AddTransient<CategoriaMenuDTOValidator>();
        services.AddTransient<ClienteDTOValidator>();
        services.AddTransient<CorteCajaDTOValidator>();
        services.AddTransient<CredencialDTOValidator>();
        services.AddTransient<LoginRequestValidator>();
        services.AddTransient<CuentaDTOValidator>();
        services.AddTransient<DescuentoAplicadoDTOValidator>();
        services.AddTransient<DetalleCuentaDTOValidator>();

        return services;
    }
}

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Menus;

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

        // Magia de Scrutor: Auto-descubrimiento de Casos de Uso (Applications)
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(MenuApplication)) // Busca en este proyecto
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Application")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Auto-descubrimiento de Validadores de FluentValidation
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(MenuApplication))
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("DTOValidator")))
            .AsSelf() // Los validadores suelen registrarse por su propio tipo, no por interfaz
            .WithTransientLifetime()); // Los validadores suelen ser Transient

        return services;
    }
}

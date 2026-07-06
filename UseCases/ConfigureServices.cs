using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Menus;
using Validator; // <-- Asegúrate de agregar este using

namespace UseCases;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            /* opcional: config extra */
        }, Assembly.GetExecutingAssembly());

        // 1. Auto-descubrimiento de Casos de Uso (Busca en el proyecto UseCases)
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(MenuApplication)) // Apunta al .dll de UseCases
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Application")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // 2. Auto-descubrimiento de Validadores (Busca en el proyecto Validator)
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(MenuDTOValidator)) // <-- CAMBIO AQUÍ: Apunta al .dll de Validator
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Validator")))
            .AsSelf() 
            .WithTransientLifetime()); 

        return services;
    }
}
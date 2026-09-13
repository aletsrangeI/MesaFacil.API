using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Persistence.Context;
using Persistence.Interceptors;
using Persistence.Repositories;

namespace Persistence;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("mesafacil_db"),
                    builder =>
                        builder.MigrationsAssembly(
                            typeof(ApplicationDbContext).Assembly.FullName
                        )
                )
        );
        
        var connString = configuration.GetConnectionString("mesafacil_db");
        var dsBuilder  = new NpgsqlDataSourceBuilder(connString);
        dsBuilder.EnableDynamicJson(); // <== Clave para json/jsonb con List<T>
        // Opcional: dsBuilder.UseNodaTime();
        var dataSource = dsBuilder.Build();

        services.Scan(scan => scan
            // 1. Busca en el ensamblado donde vive ApplicationDbContext (tu capa de persistencia)
            .FromAssembliesOf(typeof(ApplicationDbContext))
            // 2. Filtra todas las clases cuyo nombre termine en "Repository"
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            // 3. Regístralas usando la interfaz que implementan (Ej: ICuentaRepository)
            .AsImplementedInterfaces()
            // 4. Hazlo con ciclo de vida Scoped
            .WithScopedLifetime());
        
        // En Persistence/ConfigureServices.cs
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        // Spec 019: foliador atómico por sucursal/día y outbox de sincronización Edge-Cloud
        services.AddScoped<IFoliadorSucursalService, Persistence.Services.FoliadorSucursalService>();
        services.AddScoped<OutboxSaveChangesInterceptor>();

        // Repositorio genérico para todos los catálogos simples (Cat*).
        // Al registrar el tipo abierto, el DI resuelve IGenericRepository<CatMoneda>
        // → GenericCatalogRepository<CatMoneda> automáticamente.
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericCatalogRepository<>));

        return services;
    }
}

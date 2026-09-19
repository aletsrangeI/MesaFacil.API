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

        var rawConn = new[]
        {
            Environment.GetEnvironmentVariable("ConnectionStrings__mesafacil_db"),
            configuration.GetConnectionString("mesafacil_db"),
            configuration["ConnectionStrings:mesafacil_db"],
            configuration["ConnectionStrings__mesafacil_db"],
            configuration["mesafacil_db"]
        }.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

        var connString = SanitizeConnectionString(rawConn);

        services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseNpgsql(
                    connString,
                    builder =>
                        builder.MigrationsAssembly(
                            typeof(ApplicationDbContext).Assembly.FullName
                        )
                )
        );
        
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

    public static string SanitizeConnectionString(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        var s = raw.Trim();
        // Remover comillas envolventes simples o dobles que inyecta Infisical o el archivo .env
        if ((s.StartsWith("'") && s.EndsWith("'")) || (s.StartsWith("\"") && s.EndsWith("\"")))
        {
            s = s.Substring(1, s.Length - 2).Trim();
        }

        // Normalizar claves comunes a PostgreSQL / Npgsql
        if (s.Contains("Server=", StringComparison.OrdinalIgnoreCase) &&
            !s.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        {
            s = System.Text.RegularExpressions.Regex.Replace(s, @"(?i)\bServer\s*=", "Host=");
        }

        if (s.Contains("User Id=", StringComparison.OrdinalIgnoreCase) &&
            !s.Contains("Username=", StringComparison.OrdinalIgnoreCase))
        {
            s = System.Text.RegularExpressions.Regex.Replace(s, @"(?i)\bUser Id\s*=", "Username=");
        }

        return s;
    }
}

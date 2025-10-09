using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<ICategoriaMenuRepository, CategoriaMenuRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ICorteCajaRepository, CorteCajaRepository>();
        services.AddScoped<ICredencialRepository, CredencialRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICuentaRepository, CuentaRepository>();
        

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
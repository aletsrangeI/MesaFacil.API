using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Persistence.Interceptors;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace Persistence.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // 1. Construir la configuración
        // El método SetBasePath requiere Microsoft.Extensions.Configuration.Json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WebApi")) 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // 2. Obtener la cadena de conexión
        var connectionString = configuration.GetConnectionString("mesafacil_db");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("No se encontró la cadena de conexión 'mesafacil_db'.");
        }

        optionsBuilder.UseNpgsql(connectionString);

        // 3. Interceptores (Instancia manual para diseño)
        var interceptor = new AuditableEntitySaveChangesInterceptor();
        var outboxInterceptor = new OutboxSaveChangesInterceptor();

        return new ApplicationDbContext(optionsBuilder.Options, interceptor, outboxInterceptor);
    }
}
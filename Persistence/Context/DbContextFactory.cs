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
        // 1. Construir la configuración buscando appsettings en rutas relativas comunes
        var currentDir = Directory.GetCurrentDirectory();
        var candidatePaths = new[]
        {
            Path.Combine(currentDir, "WebApi"),
            Path.Combine(currentDir, "../WebApi"),
            Path.Combine(currentDir, "MesaFacil.API/WebApi"),
            currentDir
        };

        string basePath = candidatePaths.FirstOrDefault(p => File.Exists(Path.Combine(p, "appsettings.json"))) ?? currentDir;

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // 2. Obtener la cadena de conexión
        var connectionString = configuration.GetConnectionString("mesafacil_db") 
            ?? "Server=100.110.215.58;Port=5432;Database=MesaFacil;User Id=orionsys;Password=fenderstrato;";

        optionsBuilder.UseNpgsql(connectionString);

        // 3. Interceptores (Instancia manual para diseño)
        var interceptor = new AuditableEntitySaveChangesInterceptor();
        var outboxInterceptor = new OutboxSaveChangesInterceptor();

        return new ApplicationDbContext(optionsBuilder.Options, interceptor, outboxInterceptor);
    }
}
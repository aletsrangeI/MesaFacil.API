using System.Reflection;
using Domain.Entities;
using Interface.UseCases;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Common;
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

        // 3. Registro de GenericCatalogApplication para cada catálogo simple (Cat*).
        //    Se usa keyed services (.NET 8+) para que el CatalogosController pueda
        //    resolver la implementación correcta por nombre de ruta.
        RegisterGenericCatalog<CatCredencial>         (services, "credenciales");
        RegisterGenericCatalog<CatEstacionesCocina>   (services, "estaciones-cocina");
        RegisterGenericCatalog<CatEstadoCuenta>       (services, "estados-cuenta");
        RegisterGenericCatalog<CatEstadoItemKDS>      (services, "estados-item-kds");
        RegisterGenericCatalog<CatEstadoMesa>         (services, "estados-mesa");
        RegisterGenericCatalog<CatEstadoPedido>       (services, "estados-pedido");
        RegisterGenericCatalog<CatEstadoPedidoDetalle>(services, "estados-pedido-detalle");
        RegisterGenericCatalog<CatEstadoTicketCocina> (services, "estados-ticket-cocina");
        RegisterGenericCatalog<CatImpuesto>           (services, "impuestos");
        RegisterGenericCatalog<CatMetodoDePago>       (services, "metodos-pago");
        RegisterGenericCatalog<CatMoneda>             (services, "monedas");
        RegisterGenericCatalog<CatTipoDescuento>      (services, "tipos-descuento");
        RegisterGenericCatalog<CatTipoPedido>         (services, "tipos-pedido");

        return services;
    }

    /// <summary>
    /// Registra GenericCatalogApplication&lt;TEntity&gt; como IGenericCatalogApplication
    /// con una clave de servicio (serviceKey) igual al segmento de ruta del catálogo.
    /// </summary>
    private static void RegisterGenericCatalog<TEntity>(IServiceCollection services, string serviceKey)
        where TEntity : BaseAuditableEntity, ICatalogEntity, new()
    {
        services.AddKeyedScoped<IGenericCatalogApplication, GenericCatalogApplication<TEntity>>(serviceKey);
    }
}
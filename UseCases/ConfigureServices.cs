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
        RegisterGenericCatalog<CatTipoAlmacen>        (services, "tipos-almacen");
        RegisterGenericCatalog<CatMotivoMovimientoInventario>(services, "motivos-movimiento-inventario");
        RegisterGenericCatalog<CatConceptoMovimientoCaja>(services, "conceptos-movimiento-caja");
        RegisterGenericCatalog<CatMotivoCancelacionPedido>(services, "motivos-cancelacion-pedido");
        RegisterGenericCatalog<CatCanalVenta>         (services, "canales-venta");
        RegisterGenericCatalog<CatRegimenFiscal>      (services, "regimenes-fiscales");

        // Spec 015: Servicio de Descuento de Inventario por Liquidación en POS
        services.AddScoped<IDescuentoInventarioService, Inventario.DescuentoInventarioService>();

        // Spec 016: Servicios de CFDI SAT y Recepción de Compras
        services.AddScoped<Compras.ICfdiXmlParserService, Compras.CfdiXmlParserService>();
        services.AddScoped<Compras.IRecepcionCompraService, Compras.RecepcionCompraService>();

        // Spec 017: Servicio de Cuentas por Pagar (CxP), Programación de Pagos y Egresos
        services.AddScoped<CxP.ICxPService, CxP.CxPService>();

        // Spec 020: Facturación CFDI 4.0 a Comensales, Autofacturación QR y Bolsa de Timbres
        services.AddScoped<Facturacion.IGeneradorXmlCfdi40Service, Facturacion.GeneradorXmlCfdi40Service>();
        services.AddScoped<Facturacion.ISelloDigitalService, Facturacion.SelloDigitalService>();
        services.AddScoped<Facturacion.IFacturaVentaService, Facturacion.FacturaVentaService>();
        services.AddScoped<Facturacion.IAutofacturacionComensalService, Facturacion.AutofacturacionComensalService>();

        services.AddHttpClient();

        // IPACTimbradoService es agnóstico de proveedor: MockPacService es la implementación
        // DEFAULT (funciona end-to-end sin credenciales externas). Finkok/Facturama se resuelven
        // por nombre (servicio con clave) cuando EmpresaConfiguracionPAC.ProveedorPAC lo indique.
        services.AddScoped<Interface.PAC.IPACTimbradoService, Facturacion.MockPacService>();
        services.AddKeyedScoped<Interface.PAC.IPACTimbradoService, Facturacion.MockPacService>(Domain.Entities.ProveedorPacNombres.Mock);
        services.AddKeyedScoped<Interface.PAC.IPACTimbradoService>(Domain.Entities.ProveedorPacNombres.Finkok,
            (sp, _) => new Facturacion.FinkokPacAdapter(sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(Facturacion.FinkokPacAdapter))));
        services.AddKeyedScoped<Interface.PAC.IPACTimbradoService>(Domain.Entities.ProveedorPacNombres.Facturama,
            (sp, _) => new Facturacion.FacturamaPacAdapter(sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(Facturacion.FacturamaPacAdapter))));

        // Spec 021: SaaS Packaging, Tiers y Feature Gating Desacoplado. Con FeatureGating:Enabled
        // en false (valor por defecto) el servicio concede acceso total sin tocar la BD.
        services.AddScoped<Interface.Suscripciones.IFeatureGateService, Suscripciones.FeatureGateService>();

        // Spec 022: Importador Inteligente de Menú y Catálogos (Excel / CSV). Requiere IMemoryCache
        // (registrado en WebApi/Program.cs) para guardar el análisis del preview por 15 minutos.
        services.AddScoped<Importacion.IImportadorMenuService, Importacion.ImportadorMenuService>();

        // Spec 023: Asistente de Autodiagnóstico y Auto-Recuperación de Impresoras Térmicas
        // (ESC/POS vía socket TCP 9100, timeout estricto de 1.5s).
        services.AddScoped<Impresoras.IConfiguracionImpresoraService, Impresoras.ConfiguracionImpresoraService>();
        services.AddScoped<Impresoras.IImpresoraDiagnosticService, Impresoras.ImpresoraDiagnosticService>();

        // Spec 024: Candado de Supervisor (PIN 4 dígitos) y Alerta de Cancelaciones Sospechosas
        services.AddScoped<Interface.UseCases.ISupervisorPinSecurityService, Seguridad.SupervisorPinSecurityService>();
        services.AddScoped<Interface.UseCases.IAuditoriaCancelacionesService, Auditoria.AuditoriaCancelacionesService>();
        RegisterGenericCatalog<CatMotivoCancelacion>(services, "motivos-cancelacion");

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
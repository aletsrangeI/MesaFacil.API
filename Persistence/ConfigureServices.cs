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
        services.AddScoped<IDescuentoAplicadoRepository, DescuentoAplicadoRepository>();
        services.AddScoped<IDetalleCuentaRepository, DetalleCuentaRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IEstacionCocinaRepository, EstacionCocinaRepository>();
        services.AddScoped<IEventoPedidoRepository, EventoPedidoRepository>();
        services.AddScoped<IGrupoModificadorRepository, GrupoModificadorRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMesaRepository, MesaRepository>();
        services.AddScoped<IMovimientoCajaRepository, MovimientoCajaRepository>();
        services.AddScoped<IOpcionModificadorRepository, OpcionModificadorRepository>();
        services.AddScoped<IPagoRepository, PagoRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IPedidoAsientoRepository, PedidoAsientoRepository>();
        services.AddScoped<IPedidoDetalleRepository, PedidoDetalleRepository>();
        services.AddScoped<IPedidoModificadorRepository, PedidoModificadorRepository>();
        services.AddScoped<IPrecioRepository, PrecioRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<ISucursalRepository, SucursalRepository>();
        services.AddScoped<ITicketCocinaRepository, TicketCocinaRepository>();
        services.AddScoped<ITicketDetalleRepository, TicketDetalleRepository>();
        services.AddScoped<ITurnoRepository, TurnoRepository>();
        services.AddScoped<IUsuarioRolRepository, UsuarioRolRepository>();
        services.AddScoped<IVarianteProductoRepository, VarianteProductoRepository>();
        services.AddScoped<IFormFieldRepository, FormFieldRepository>();


        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

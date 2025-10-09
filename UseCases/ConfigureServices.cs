using System.Reflection;
using Interface.UseCases;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Areas;
using UseCases.Auth;
using UseCases.CatalogItems;
using UseCases.Catalogs;
using UseCases.CategoriaMenus;
using UseCases.Clientes;
using UseCases.CorteCajas;
using UseCases.Credenciales;
using UseCases.Cuentas;
using UseCases.DescuentosAplicados;
using UseCases.DetalleCuentas;
using UseCases.Empresas;
using UseCases.EstacionesCocina;
using UseCases.EventosPedido;
using UseCases.GruposModificador;
using UseCases.Menus;
using UseCases.Mesas;
using UseCases.MovimientosCaja;
using UseCases.OpcionesModificador;
using UseCases.Pagos;
using UseCases.Pedidos;
using UseCases.PedidosAsiento;
using UseCases.PedidoDetalles;
using UseCases.PedidoModificadores;
using UseCases.Precios;
using UseCases.Productos;
using UseCases.Roles;
using UseCases.Sucursales;
using UseCases.TicketsCocina;
using UseCases.TicketDetalles;
using UseCases.Turnos;
using UseCases.Usuarios;
using UseCases.UsuarioRoles;
using UseCases.VarianteProductos;
using Validator;
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

        // Casos de uso (Application Layer)
        services.AddScoped<ICatalogApplication, CatalogApplication>();
        services.AddScoped<IAreaApplication, AreaApplication>();
        services.AddScoped<ICategoriaMenuApplication, CategoriaMenuApplication>();
        services.AddScoped<ICatalogItemApplication, CatalogItemApplication>();
        services.AddScoped<IClienteApplication, ClienteApplication>();
        services.AddScoped<ICorteCajaApplication, CorteCajaApplication>();
        services.AddScoped<ICredencialApplication, CredencialApplication>();
        services.AddScoped<IAuthApplication, AuthApplication>();
        services.AddScoped<ICuentaApplication, CuentaApplication>();
        services.AddScoped<IDescuentoAplicadoApplication, DescuentoAplicadoApplication>();
        services.AddScoped<IDetalleCuentaApplication, DetalleCuentaApplication>();
        services.AddScoped<IEmpresaApplication, EmpresaApplication>();
        services.AddScoped<IEstacionCocinaApplication, EstacionCocinaApplication>();
        services.AddScoped<IEventoPedidoApplication, EventoPedidoApplication>();
        services.AddScoped<IGrupoModificadorApplication, GrupoModificadorApplication>();
        services.AddScoped<IMenuApplication, MenuApplication>();
        services.AddScoped<IMesaApplication, MesaApplication>();
        services.AddScoped<IMovimientoCajaApplication, MovimientoCajaApplication>();
        services.AddScoped<IOpcionModificadorApplication, OpcionModificadorApplication>();
        services.AddScoped<IPagoApplication, PagoApplication>();
        services.AddScoped<IPedidoApplication, PedidoApplication>();
        services.AddScoped<IPedidoAsientoApplication, PedidoAsientoApplication>();
        services.AddScoped<IPedidoDetalleApplication, PedidoDetalleApplication>();
        services.AddScoped<IPedidoModificadorApplication, PedidoModificadorApplication>();
        services.AddScoped<IPrecioApplication, PrecioApplication>();
        services.AddScoped<IProductoApplication, ProductoApplication>();
        services.AddScoped<IRolApplication, RolApplication>();
        services.AddScoped<ISucursalApplication, SucursalApplication>();
        services.AddScoped<ITicketCocinaApplication, TicketCocinaApplication>();
        services.AddScoped<ITicketDetalleApplication, TicketDetalleApplication>();
        services.AddScoped<ITurnoApplication, TurnoApplication>();
        services.AddScoped<IUsuarioApplication, UsuarioApplication>();
        services.AddScoped<IUsuarioRolApplication, UsuarioRolApplication>();
        services.AddScoped<IVarianteProductoApplication, VarianteProductoApplication>();

        // Validadores (si quieres mantenerlos aquí está bien; no dependen de WebApi)
        services.AddTransient<CatalogDTOValidator>();
        services.AddTransient<AreaDTOValidator>();
        services.AddTransient<CatalogItemDTOValidator>();
        services.AddTransient<CategoriaMenuDTOValidator>();
        services.AddTransient<ClienteDTOValidator>();
        services.AddTransient<CorteCajaDTOValidator>();
        services.AddTransient<CredencialDTOValidator>();
        services.AddTransient<LoginRequestValidator>();
        services.AddTransient<CuentaDTOValidator>();
        services.AddTransient<DescuentoAplicadoDTOValidator>();
        services.AddTransient<DetalleCuentaDTOValidator>();
        services.AddTransient<EmpresaDTOValidator>();
        services.AddTransient<EstacionCocinaDTOValidator>();
        services.AddTransient<EventoPedidoDTOValidator>();
        services.AddTransient<GrupoModificadorDTOValidator>();
        services.AddTransient<MenuDTOValidator>();
        services.AddTransient<MesaDTOValidator>();
        services.AddTransient<MovimientoCajaDTOValidator>();
        services.AddTransient<OpcionModificadorDTOValidator>();
        services.AddTransient<PagoDTOValidator>();
        services.AddTransient<PedidoDTOValidator>();
        services.AddTransient<PedidoAsientoDTOValidator>();
        services.AddTransient<PedidoDetalleDTOValidator>();
        services.AddTransient<PedidoModificadorDTOValidator>();
        services.AddTransient<PrecioDTOValidator>();
        services.AddTransient<ProductoDTOValidator>();
        services.AddTransient<RolDTOValidator>();
        services.AddTransient<SucursalDTOValidator>();
        services.AddTransient<TicketCocinaDTOValidator>();
        services.AddTransient<TicketDetalleDTOValidator>();
        services.AddTransient<TurnoDTOValidator>();
        services.AddTransient<UsuarioDTOValidator>();
        services.AddTransient<UsuarioRolDTOValidator>();
        services.AddTransient<VarianteProductoDTOValidator>();

        return services;
    }
}

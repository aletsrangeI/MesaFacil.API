namespace Interface.Persistence;

public interface IUnitOfWork : IDisposable
{
    IAreaRepository Areas { get; }
    ICategoriaMenuRepository CategoriaMenus { get; }
    IClienteRepository Clientes { get; }
    ICorteCajaRepository CorteCajas { get; }
    ICredencialRepository Credenciales { get; }
    ICuentaRepository Cuentas { get; }
    IDescuentoAplicadoRepository DescuentosAplicados { get; }
    IDetalleCuentaRepository DetalleCuentas { get; }
    IEmpresaRepository Empresas { get; }
    IEstacionCocinaRepository EstacionesCocina { get; }
    IEventoPedidoRepository EventosPedido { get; }
    IGrupoModificadorRepository GruposModificador { get; }
    IMenuRepository Menus { get; }
    IMesaRepository Mesas { get; }
    IMovimientoCajaRepository MovimientosCaja { get; }
    IOpcionModificadorRepository OpcionesModificador { get; }
    IPagoRepository Pagos { get; }
    IPedidoRepository Pedidos { get; }
    IPedidoAsientoRepository PedidosAsiento { get; }
    IPedidoDetalleRepository PedidoDetalles { get; }
    IPedidoModificadorRepository PedidoModificadores { get; }
    IPrecioRepository Precios { get; }
    IProductoRepository Productos { get; }
    IRolRepository Roles { get; }
    ISucursalRepository Sucursales { get; }
    ITicketCocinaRepository TicketsCocina { get; }
    ITicketDetalleRepository TicketDetalles { get; }
    ITurnoRepository Turnos { get; }
    IUsuarioRepository Usuarios { get; }
    IUsuarioRolRepository UsuarioRoles { get; }
    IVarianteProductoRepository VarianteProductos { get; }
    IFormFieldRepository FormFields { get; }
    IAccesoRutaRepository AccesoRutas { get; }
    IRolAccesoRutaRepository RolAccesoRutas { get; }
    IFormularioRepository Formularios { get; }
    
    ICatCredencialRepository CatCredenciales { get; } 
}
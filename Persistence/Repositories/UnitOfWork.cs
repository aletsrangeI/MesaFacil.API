using Interface.Persistence;
using Persistence.Context;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public ICatalogRepository Catalogs { get; }

    private readonly ApplicationDbContext _context;

    public ICatalogItemRepository CatalogItems { get; }

    public IAreaRepository Areas { get; }

    public ICategoriaMenuRepository CategoriaMenus { get; }
    public IClienteRepository Clientes { get; }

    public ICorteCajaRepository CorteCajas { get; }

    public ICredencialRepository Credenciales { get; }

    public ICuentaRepository Cuentas { get; }
    public IDescuentoAplicadoRepository DescuentosAplicados { get; }

    public IDetalleCuentaRepository DetalleCuentas { get; }

    public IEmpresaRepository Empresas { get; }
    public IEstacionCocinaRepository EstacionesCocina { get; }
    public IEventoPedidoRepository EventosPedido { get; }

    public IGrupoModificadorRepository GruposModificador { get; }
    public IMenuRepository Menus { get; }
    public IMesaRepository Mesas { get; }

    public IMovimientoCajaRepository MovimientosCaja { get; }
    public IOpcionModificadorRepository OpcionesModificador { get; }
    public IPagoRepository Pagos { get; }

    public IPedidoRepository Pedidos { get; }

    public IPedidoAsientoRepository PedidosAsiento { get; }

    public IPedidoDetalleRepository PedidoDetalles { get; }

    public IPedidoModificadorRepository PedidoModificadores { get; }
    public IPrecioRepository Precios { get; }

    public IProductoRepository Productos { get; }
    public IRolRepository Roles { get; }

    public ISucursalRepository Sucursales { get; }
    public ITicketCocinaRepository TicketsCocina { get; }
    public ITicketDetalleRepository TicketDetalles { get; }

    public ITurnoRepository Turnos { get; }
    public IUsuarioRepository Usuarios { get; }
    public IUsuarioRolRepository UsuarioRoles { get; }
    public IVarianteProductoRepository VarianteProductos { get; }

    public IFormFieldRepository FormFields { get; }
    public IAccesoRutaRepository AccesoRutas { get; }
    public IRolAccesoRutaRepository RolAccesoRutas { get; }
    public UnitOfWork(ApplicationDbContext context,
        ICatalogRepository catalogRepository,
        ICatalogItemRepository catalogItems,
        IAreaRepository areaRepository,
        ICategoriaMenuRepository categoriaMenuRepository,
        IClienteRepository clienteRepository,
        ICorteCajaRepository corteCajasRepository,
        ICredencialRepository credencialRepository,
        ICuentaRepository cuentaRepository,
        IDescuentoAplicadoRepository descuentosAplicadosRepository,
        IDetalleCuentaRepository detalleCuentaRepository,
        IEmpresaRepository empresaRepository,
        IEstacionCocinaRepository estacionesCocinaRepository,
        IEventoPedidoRepository eventosPedidoRepository,
        IGrupoModificadorRepository grupoModificadorRepository,
        IMenuRepository menuRepository,
        IMesaRepository mesaRepository,
        IMovimientoCajaRepository movimientoCajaRepository,
        IOpcionModificadorRepository opcionesModificadorRepository,
        IPagoRepository pagoRepository,
        IPedidoRepository pedidoRepository,
        IPedidoAsientoRepository pedidoAsientoRepository,
        IPedidoDetalleRepository pedidoDetalleRepository,
        IPedidoModificadorRepository pedidoModificadorRepository,
        IPrecioRepository precioRepository,
        IProductoRepository productoRepository,
        IRolRepository rolRepository,
        ISucursalRepository sucursalRepository,
        ITicketCocinaRepository ticketCocinaRepository,
        ITicketDetalleRepository ticketDetalleRepository,
        ITurnoRepository turnoRepository,
        IUsuarioRepository usuarioRepository,
        IUsuarioRolRepository usuarioRolRepository,
        IVarianteProductoRepository varianteProductoRepository,
        IFormFieldRepository formFieldRepository,
        IAccesoRutaRepository accesoRutaRepository,
        IRolAccesoRutaRepository rolAccesoRutaRepository
    )
    {
        RolAccesoRutas = rolAccesoRutaRepository;
        AccesoRutas = accesoRutaRepository;
        FormFields = formFieldRepository;
        VarianteProductos = varianteProductoRepository;
        UsuarioRoles = usuarioRolRepository;
        Usuarios = usuarioRepository;
        Turnos = turnoRepository;
        TicketDetalles = ticketDetalleRepository;
        TicketsCocina = ticketCocinaRepository;
        Sucursales = sucursalRepository;
        Roles = rolRepository;
        Productos = productoRepository;
        Precios = precioRepository;
        PedidoModificadores = pedidoModificadorRepository;
        PedidoDetalles = pedidoDetalleRepository;
        PedidosAsiento = pedidoAsientoRepository;
        Pedidos = pedidoRepository;
        Pagos = pagoRepository;
        OpcionesModificador = opcionesModificadorRepository;
        MovimientosCaja = movimientoCajaRepository;
        Mesas = mesaRepository;
        Menus = menuRepository;
        GruposModificador = grupoModificadorRepository;
        EventosPedido = eventosPedidoRepository;
        EstacionesCocina = estacionesCocinaRepository;
        Empresas = empresaRepository;
        DetalleCuentas = detalleCuentaRepository;
        DescuentosAplicados = descuentosAplicadosRepository;
        Cuentas = cuentaRepository;
        Credenciales = credencialRepository;
        CorteCajas = corteCajasRepository;
        Clientes = clienteRepository;
        CategoriaMenus = categoriaMenuRepository;
        Areas = areaRepository;
        CatalogItems = catalogItems;
        _context = context;
        Catalogs = catalogRepository;
    }

    public async Task<int> Save(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

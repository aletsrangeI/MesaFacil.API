using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Interceptors;

namespace Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    static ApplicationDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<CategoriaMenu> CategoriaMenus { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<CorteCaja> CorteCajas { get; set; }
    public DbSet<Credencial> Credenciales { get; set; }
    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<DescuentoAplicado> DescuentosAplicados { get; set; }
    public DbSet<DetalleCuenta> DetalleCuentas { get; set; }
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<EstacionCocina> EstacionesCocina { get; set; }
    public DbSet<EventoPedido> EventosPedido { get; set; }
    public DbSet<GrupoModificador> GruposModificador { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Mesa> Mesas { get; set; }
    public DbSet<MovimientoCaja> MovimientosCaja { get; set; }
    public DbSet<OpcionModificador> OpcionesModificador { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoAsiento> PedidosAsiento { get; set; }
    public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
    public DbSet<PedidoModificador> PedidoModificadores { get; set; }
    public DbSet<Precio> Precios { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Sucursal> Sucursales { get; set; }
    public DbSet<TicketCocina> TicketsCocina { get; set; }
    public DbSet<TicketDetalle> TicketDetalles { get; set; }
    public DbSet<Turno> Turnos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<UsuarioRol> UsuarioRoles { get; set; }
    public DbSet<VarianteProducto> VarianteProductos { get; set; }
    public DbSet<FormField> FormFields { get; set; }
    public DbSet<AccesoRuta> AccesoRutas { get; set; }
    public DbSet<RolAccesoRuta> RolAccesoRutas { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<FormValidation>();
        modelBuilder.Ignore<SelectFormOption>();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        optionsBuilder.EnableSensitiveDataLogging();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}

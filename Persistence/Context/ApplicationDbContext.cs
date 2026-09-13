using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Interceptors;

namespace Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    public readonly OutboxSaveChangesInterceptor _outboxSaveChangesInterceptor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        OutboxSaveChangesInterceptor outboxSaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _outboxSaveChangesInterceptor = outboxSaveChangesInterceptor;
    }

    static ApplicationDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Area> Areas { get; set; }
    public DbSet<CategoriaMenu> CategoriaMenus { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<CorteCaja> CorteCajas { get; set; }
    public DbSet<Credencial> Credenciales { get; set; }
    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<DescuentoAplicado> DescuentosAplicados { get; set; }
    public DbSet<DetalleCuenta> DetalleCuentas { get; set; }
    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<FoliadorSucursal> FoliadoresSucursal { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; }
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
    public DbSet<Formulario> Formularios { get; set; }
    public DbSet<CatCredencial>          CatCredenciales         { get; set; }
    public DbSet<CatEstacionesCocina>    CatEstacionesCocina     { get; set; }
    public DbSet<CatEstadoCuenta>        CatEstadosCuenta        { get; set; }
    public DbSet<CatEstadoItemKDS>       CatEstadosItemKDS       { get; set; }
    public DbSet<CatEstadoMesa>          CatEstadosMesa          { get; set; }
    public DbSet<CatEstadoPedido>        CatEstadosPedido        { get; set; }
    public DbSet<CatEstadoPedidoDetalle> CatEstadosPedidoDetalle { get; set; }
    public DbSet<CatEstadoTicketCocina>  CatEstadosTicketCocina  { get; set; }
    public DbSet<CatImpuesto>            CatImpuestos            { get; set; }
    public DbSet<CatMetodoDePago>        CatMetodosDePago        { get; set; }
    public DbSet<CatMoneda>              CatMonedas              { get; set; }
    public DbSet<CatTipoDescuento>       CatTiposDescuento       { get; set; }
    public DbSet<CatTipoPedido>          CatTiposPedido          { get; set; }
    public DbSet<CatMotivoMovimientoInventario> CatMotivosMovimientoInventario { get; set; }
    public DbSet<CatTipoAlmacen>         CatTiposAlmacen         { get; set; }
    public DbSet<CatConceptoMovimientoCaja> CatConceptosMovimientoCaja { get; set; }
    public DbSet<CatMotivoCancelacionPedido> CatMotivosCancelacionPedido { get; set; }
    public DbSet<CatCanalVenta>          CatCanalesVenta         { get; set; }
    public DbSet<CatRegimenFiscal>       CatRegimenesFiscales    { get; set; }

    // Spec 024: Candado de Supervisor (PIN 4 dígitos) y Auditoría de Cancelaciones
    public DbSet<CatMotivoCancelacion>   CatMotivosCancelacion   { get; set; }

    // Spec 014: Inventarios, Insumos, Almacenes y Kárdex
    public DbSet<UnidadMedida>           UnidadesMedida          { get; set; }
    public DbSet<FactorConversion>       FactoresConversion      { get; set; }
    public DbSet<CategoriaInsumo>        CategoriasInsumo        { get; set; }
    public DbSet<Insumo>                 Insumos                 { get; set; }
    public DbSet<Almacen>                Almacenes               { get; set; }
    public DbSet<InventarioExistencia>   InventarioExistencias   { get; set; }
    public DbSet<KardexMovimiento>       KardexMovimientos       { get; set; }
    public DbSet<TraspasoAlmacen>        TraspasosAlmacen        { get; set; }
    public DbSet<TraspasoAlmacenDetalle> TraspasoAlmacenDetalles { get; set; }

    // Spec 015: Recetas (Escandallos) y Sub-recetas
    public DbSet<Receta>                 Recetas                 { get; set; }
    public DbSet<RecetaDetalle>          RecetaDetalles          { get; set; }

    // Spec 016: Proveedores, Entradas de Compra y Facturas CFDI
    public DbSet<Proveedor>              Proveedores             { get; set; }
    public DbSet<MapeoInsumoProveedor>   MapeosInsumoProveedor   { get; set; }
    public DbSet<CompraFactura>          ComprasFactura          { get; set; }
    public DbSet<CompraFacturaDetalle>   CompraFacturaDetalles   { get; set; }

    // Spec 017: Cuentas por Pagar (CxP), Programación de Pagos y Egresos
    public DbSet<CuentaPorPagar>         CuentasPorPagar         { get; set; }
    public DbSet<PagoCuentaPorPagar>     PagosCuentaPorPagar     { get; set; }

    // Spec 020: Facturación CFDI 4.0 a Comensales, Autofacturación QR y Bolsa de Timbres
    public DbSet<EmpresaConfiguracionPAC> EmpresaConfiguracionesPAC { get; set; }
    public DbSet<EmpresaBolsaTimbres>     EmpresaBolsasTimbres      { get; set; }
    public DbSet<ConsumoTimbreHistorial>  ConsumosTimbreHistorial   { get; set; }
    public DbSet<FacturaVenta>            FacturasVenta             { get; set; }
    public DbSet<FacturaVentaDetalle>     FacturaVentaDetalles      { get; set; }

    // Spec 021: SaaS Packaging, Tiers y Feature Gating Desacoplado (infraestructura apagada
    // por defecto vía FeatureGating:Enabled = false mientras dure la etapa de demos/pilotos).
    public DbSet<CatPlanSuscripcion>      CatPlanesSuscripcion      { get; set; }
    public DbSet<EmpresaSuscripcion>      EmpresasSuscripcion       { get; set; }

    // Spec 023: Asistente de Autodiagnóstico y Auto-Recuperación de Impresoras Térmicas
    public DbSet<ConfiguracionImpresora>  ConfiguracionesImpresora  { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<FormValidation>();
        modelBuilder.Ignore<SelectFormOption>();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Spec 014: Configuración de Inventario
        modelBuilder.Entity<InventarioExistencia>()
            .HasIndex(e => new { e.IdAlmacen, e.IdInsumo })
            .IsUnique();

        modelBuilder.Entity<KardexMovimiento>()
            .HasIndex(k => new { k.IdAlmacen, k.IdInsumo, k.FechaHora });

        modelBuilder.Entity<TraspasoAlmacen>()
            .HasOne(t => t.AlmacenOrigen)
            .WithMany()
            .HasForeignKey(t => t.IdAlmacenOrigen)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TraspasoAlmacen>()
            .HasOne(t => t.AlmacenDestino)
            .WithMany()
            .HasForeignKey(t => t.IdAlmacenDestino)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TraspasoAlmacen>()
            .HasOne(t => t.UsuarioSolicita)
            .WithMany()
            .HasForeignKey(t => t.IdUsuarioSolicita)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TraspasoAlmacen>()
            .HasOne(t => t.UsuarioRecibe)
            .WithMany()
            .HasForeignKey(t => t.IdUsuarioRecibe)
            .OnDelete(DeleteBehavior.Restrict);

        // Spec 015: Configuración de Recetas
        modelBuilder.Entity<Receta>()
            .HasIndex(r => r.IdProducto);

        modelBuilder.Entity<Receta>()
            .HasIndex(r => r.IdVariante);

        modelBuilder.Entity<Receta>()
            .HasIndex(r => r.IdOpcionModificador);

        modelBuilder.Entity<Receta>()
            .HasIndex(r => r.EsSubReceta);

        modelBuilder.Entity<RecetaDetalle>()
            .HasOne(d => d.Receta)
            .WithMany(r => r.Detalles)
            .HasForeignKey(d => d.IdReceta)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecetaDetalle>()
            .HasOne(d => d.Insumo)
            .WithMany()
            .HasForeignKey(d => d.IdInsumo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecetaDetalle>()
            .HasOne(d => d.SubReceta)
            .WithMany()
            .HasForeignKey(d => d.IdSubReceta)
            .OnDelete(DeleteBehavior.Restrict);

        // Spec 016: Configuración de Proveedores, Compras y Facturas CFDI
        modelBuilder.Entity<Proveedor>()
            .HasIndex(p => new { p.IdEmpresa, p.RFC });

        modelBuilder.Entity<MapeoInsumoProveedor>()
            .HasIndex(m => new { m.IdProveedor, m.ClaveProdServ, m.DescripcionSAT });

        modelBuilder.Entity<MapeoInsumoProveedor>()
            .HasOne(m => m.Proveedor)
            .WithMany(p => p.Mapeos)
            .HasForeignKey(m => m.IdProveedor)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MapeoInsumoProveedor>()
            .HasOne(m => m.Insumo)
            .WithMany()
            .HasForeignKey(m => m.IdInsumo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompraFactura>()
            .HasIndex(c => c.UUID);

        modelBuilder.Entity<CompraFactura>()
            .HasIndex(c => new { c.IdEmpresa, c.IdSucursal, c.FechaEmision });

        modelBuilder.Entity<CompraFactura>()
            .HasOne(c => c.Proveedor)
            .WithMany(p => p.Compras)
            .HasForeignKey(c => c.IdProveedor)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompraFactura>()
            .HasOne(c => c.Almacen)
            .WithMany()
            .HasForeignKey(c => c.IdAlmacen)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompraFactura>()
            .HasOne(c => c.Sucursal)
            .WithMany()
            .HasForeignKey(c => c.IdSucursal)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompraFacturaDetalle>()
            .HasOne(d => d.CompraFactura)
            .WithMany(c => c.Detalles)
            .HasForeignKey(d => d.IdCompraFactura)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompraFacturaDetalle>()
            .HasOne(d => d.Insumo)
            .WithMany()
            .HasForeignKey(d => d.IdInsumo)
            .OnDelete(DeleteBehavior.Restrict);

        // Spec 017: Configuración de Cuentas por Pagar (CxP)
        modelBuilder.Entity<CuentaPorPagar>()
            .HasIndex(c => new { c.IdEmpresa, c.IdSucursal, c.Estado, c.FechaVencimiento });

        modelBuilder.Entity<CuentaPorPagar>()
            .HasOne(c => c.Empresa)
            .WithMany()
            .HasForeignKey(c => c.IdEmpresa)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CuentaPorPagar>()
            .HasOne(c => c.Sucursal)
            .WithMany()
            .HasForeignKey(c => c.IdSucursal)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CuentaPorPagar>()
            .HasOne(c => c.Proveedor)
            .WithMany()
            .HasForeignKey(c => c.IdProveedor)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CuentaPorPagar>()
            .HasOne(c => c.CompraFactura)
            .WithMany()
            .HasForeignKey(c => c.IdCompraFactura)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PagoCuentaPorPagar>()
            .HasOne(p => p.CuentaPorPagar)
            .WithMany(c => c.Pagos)
            .HasForeignKey(p => p.IdCuentaPorPagar)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PagoCuentaPorPagar>()
            .HasOne(p => p.MetodoPago)
            .WithMany()
            .HasForeignKey(p => p.IdMetodoPago)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PagoCuentaPorPagar>()
            .HasOne(p => p.MovimientoCaja)
            .WithMany()
            .HasForeignKey(p => p.IdMovimientoCaja)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PagoCuentaPorPagar>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor, _outboxSaveChangesInterceptor);
        optionsBuilder.EnableSensitiveDataLogging();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
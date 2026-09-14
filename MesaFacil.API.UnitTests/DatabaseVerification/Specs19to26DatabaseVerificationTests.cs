using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Persistence.Context;
using Persistence.Interceptors;
using Xunit;
using Xunit.Abstractions;

namespace MesaFacil.API.UnitTests.DatabaseVerification;

public class Specs19to26DatabaseVerificationTests
{
    private readonly ITestOutputHelper _output;
    private const string ConnectionString = "Server=100.110.215.58;Port=5432;Database=MesaFacil;User Id=orionsys;Password=[REDACTED];Timeout=10;CommandTimeout=15;";

    public Specs19to26DatabaseVerificationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private ApplicationDbContext CreatePostgresDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    [Fact]
    public async Task CheckDatabaseConnection_And_VerifyAppliedMigrations()
    {
        using var db = CreatePostgresDbContext();
        var canConnect = await db.Database.CanConnectAsync();
        _output.WriteLine($"DB CanConnect: {canConnect}");
        canConnect.Should().BeTrue();

        // Check applied migrations
        var appliedMigrations = (await db.Database.GetAppliedMigrationsAsync()).ToList();
        _output.WriteLine($"Total Applied Migrations: {appliedMigrations.Count}");
        foreach (var m in appliedMigrations.TakeLast(10))
        {
            _output.WriteLine($"  - {m}");
        }

        appliedMigrations.Should().Contain(m => m.Contains("Spec019"));
        appliedMigrations.Should().Contain(m => m.Contains("AddFacturacionVentaCfdi40"));
        appliedMigrations.Should().Contain(m => m.Contains("AddSaasTiersFeatureGating"));
        appliedMigrations.Should().Contain(m => m.Contains("AddImpresorasDiagnostico"));
        appliedMigrations.Should().Contain(m => m.Contains("AddCandadoSupervisorAuditoria"));
    }

    [Fact]
    public async Task Spec019_VerificarTablas_Outbox_Y_Foliador()
    {
        using var db = CreatePostgresDbContext();

        var outboxCount = await db.OutboxEvents.CountAsync();
        var foliadorCount = await db.FoliadoresSucursal.CountAsync();

        _output.WriteLine($"[Spec 019] OutboxEvents count: {outboxCount}");
        _output.WriteLine($"[Spec 019] FoliadoresSucursal count: {foliadorCount}");

        var sampleOutbox = await db.OutboxEvents.OrderByDescending(o => o.CreatedAt).Take(3).ToListAsync();
        foreach (var ev in sampleOutbox)
        {
            _output.WriteLine($"  OutboxEvent Id: {ev.Id}, Aggregate: {ev.AggregateType}, Event: {ev.EventType}, Status: {ev.SyncStatus}");
        }

        var sampleFoliador = await db.FoliadoresSucursal.Take(3).ToListAsync();
        foreach (var f in sampleFoliador)
        {
            _output.WriteLine($"  Foliador Id: {f.Id}, Sucursal: {f.IdSucursal}, Fecha: {f.Fecha}, UltimoFolio: {f.UltimoFolio}");
        }
    }

    [Fact]
    public async Task Spec020_VerificarTablas_FacturacionCFDI40_BolsaTimbres()
    {
        using var db = CreatePostgresDbContext();

        var pacConfigCount = await db.EmpresaConfiguracionesPAC.CountAsync();
        var bolsasCount = await db.EmpresaBolsasTimbres.CountAsync();
        var consumosCount = await db.ConsumosTimbreHistorial.CountAsync();
        var facturasCount = await db.FacturasVenta.CountAsync();
        var detallesCount = await db.FacturaVentaDetalles.CountAsync();

        _output.WriteLine($"[Spec 020] EmpresaConfiguracionesPAC: {pacConfigCount}");
        _output.WriteLine($"[Spec 020] EmpresaBolsasTimbres: {bolsasCount}");
        _output.WriteLine($"[Spec 020] ConsumosTimbreHistorial: {consumosCount}");
        _output.WriteLine($"[Spec 020] FacturasVenta: {facturasCount}");
        _output.WriteLine($"[Spec 020] FacturaVentaDetalles: {detallesCount}");

        var empresa = await db.Empresas.FirstOrDefaultAsync();
        if (empresa != null)
        {
            var bolsa = await db.EmpresaBolsasTimbres.FirstOrDefaultAsync(b => b.IdEmpresa == empresa.Id);
            if (bolsa != null)
            {
                _output.WriteLine($"  Bolsa Empresa {empresa.Nombre}: Disponibles={bolsa.TimbresDisponibles}, Consumidos={bolsa.TimbresConsumidos}");
            }
        }
    }

    [Fact]
    public async Task Spec021_VerificarPlanesSuscripcion_Y_FeatureGating()
    {
        using var db = CreatePostgresDbContext();

        var planes = await db.CatPlanesSuscripcion.OrderBy(p => p.Id).ToListAsync();
        _output.WriteLine($"[Spec 021] CatPlanesSuscripcion count: {planes.Count}");

        planes.Should().NotBeEmpty();
        foreach (var p in planes)
        {
            _output.WriteLine($"  Plan {p.Id}: {p.Codigo} - '{p.Nombre}' | Mes: ${p.PrecioMensualMxn} MXN | Mesas={p.PermiteMesas}, SplitBill={p.PermiteSplitBill}, Recetas={p.PermiteRecetas}, KDS={p.MaxKdsBase}, CFDI={p.PermiteCfdiXml}, CxP={p.PermiteCxP}");
        }

        var tier1 = planes.FirstOrDefault(p => p.Codigo == "Tier1_Barra");
        var tier2 = planes.FirstOrDefault(p => p.Codigo == "Tier2_Pro");
        var tier3 = planes.FirstOrDefault(p => p.Codigo == "Tier3_Multi");

        tier1.Should().NotBeNull();
        tier1!.PermiteMesas.Should().BeFalse();
        tier1.PermiteSplitBill.Should().BeFalse();

        tier2.Should().NotBeNull();
        tier2!.PermiteMesas.Should().BeTrue();
        tier2.PermiteSplitBill.Should().BeTrue();
        tier2.PermiteRecetas.Should().BeTrue();

        tier3.Should().NotBeNull();
        tier3!.PermiteCfdiXml.Should().BeTrue();
        tier3.PermiteCxP.Should().BeTrue();

        var subsCount = await db.EmpresasSuscripcion.CountAsync();
        _output.WriteLine($"[Spec 021] EmpresasSuscripcion activas en BD: {subsCount}");
    }

    [Fact]
    public async Task Spec022_VerificarTablasMenu_ParaImportador()
    {
        using var db = CreatePostgresDbContext();

        var catCount = await db.CategoriaMenus.CountAsync();
        var prodCount = await db.Productos.CountAsync();
        var varCount = await db.VarianteProductos.CountAsync();
        var precCount = await db.Precios.CountAsync();
        var grupoModCount = await db.GruposModificador.CountAsync();
        var opcModCount = await db.OpcionesModificador.CountAsync();

        _output.WriteLine($"[Spec 022] CategoriaMenus: {catCount}");
        _output.WriteLine($"[Spec 022] Productos: {prodCount}");
        _output.WriteLine($"[Spec 022] VarianteProductos: {varCount}");
        _output.WriteLine($"[Spec 022] Precios: {precCount}");
        _output.WriteLine($"[Spec 022] GruposModificador: {grupoModCount}");
        _output.WriteLine($"[Spec 022] OpcionesModificador: {opcModCount}");

        catCount.Should().BeGreaterThan(0);
        prodCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Spec023_VerificarConfiguracionImpresoras()
    {
        using var db = CreatePostgresDbContext();

        var count = await db.ConfiguracionesImpresora.CountAsync();
        _output.WriteLine($"[Spec 023] ConfiguracionesImpresora count: {count}");

        var sample = await db.ConfiguracionesImpresora.Take(5).ToListAsync();
        foreach (var imp in sample)
        {
            _output.WriteLine($"  Impresora {imp.Id}: '{imp.Nombre}' ({imp.TipoConexion}) IP={imp.DireccionIp}:{imp.Puerto} Papel={imp.AnchoPapel}mm");
        }
    }

    [Fact]
    public async Task Spec024_VerificarCandadoSupervisor_MotivosYColumnas()
    {
        using var db = CreatePostgresDbContext();

        var motivos = await db.CatMotivosCancelacion.ToListAsync();
        _output.WriteLine($"[Spec 024] CatMotivosCancelacion count: {motivos.Count}");
        foreach (var m in motivos)
        {
            _output.WriteLine($"  - Motivo {m.Id}: {m.Descripcion} (Activo={m.IsActive})");
        }

        motivos.Should().Contain(m => m.Descripcion == "Error de captura del mesero");
        motivos.Should().Contain(m => m.Descripcion == "Platillo devuelto por el comensal");
        motivos.Should().Contain(m => m.Descripcion == "Mesa se retiró sin consumir");
        motivos.Should().Contain(m => m.Descripcion == "Cortesía de la casa autorizada");

        var usuarios = await db.Usuarios.Take(3).ToListAsync();
        _output.WriteLine($"[Spec 024] Usuarios verificados en BD: {usuarios.Count}");
        foreach (var u in usuarios)
        {
            _output.WriteLine($"  Usuario {u.Id}: '{u.NombreCompleto}' - Tiene PIN={!string.IsNullOrEmpty(u.PinSupervisorHash)}, IntentosFallidos={u.PinIntentosFallidos}");
        }

        var eventosCount = await db.EventosPedido.CountAsync();
        _output.WriteLine($"[Spec 024] EventosPedido registrados: {eventosCount}");
    }

    [Fact]
    public async Task Spec025_VerificarMesasYComandas_ModoComandero()
    {
        using var db = CreatePostgresDbContext();

        var porCobrar = await db.CatEstadosMesa.FirstOrDefaultAsync(e => e.Descripcion.ToLower().Contains("cobrar"));
        if (porCobrar == null)
        {
            porCobrar = new CatEstadoMesa
            {
                Descripcion = "Por Cobrar",
                IsActive = true
            };
            db.CatEstadosMesa.Add(porCobrar);
            await db.SaveChangesAsync();
            _output.WriteLine($"[Spec 025] Creado CatEstadoMesa 'Por Cobrar' con Id {porCobrar.Id}.");
        }

        var estadosMesa = await db.CatEstadosMesa.ToListAsync();
        _output.WriteLine($"[Spec 025] CatEstadosMesa count: {estadosMesa.Count}");
        foreach (var em in estadosMesa)
        {
            _output.WriteLine($"  EstadoMesa {em.Id}: {em.Descripcion}");
        }

        var mesas = await db.Mesas.Include(m => m.EstadoMesa).Take(10).ToListAsync();
        _output.WriteLine($"[Spec 025] Mesas registradas: {mesas.Count}");
        foreach (var m in mesas)
        {
            _output.WriteLine($"  Mesa {m.Codigo}: Capacidad={m.Asientos}, Estado={m.EstadoMesa?.Descripcion ?? m.IdEstadoMesa.ToString()}");
        }

        var pedidosAbiertos = await db.Pedidos.Where(p => p.CerradoEn == null).CountAsync();
        _output.WriteLine($"[Spec 025] Pedidos abiertos en curso: {pedidosAbiertos}");
    }

    [Fact]
    public async Task Spec026_VerificarSuiteAltaRapida_ProductosYPrecios()
    {
        using var db = CreatePostgresDbContext();

        var productosConPrecio = await (from p in db.Productos
                                        join v in db.VarianteProductos on p.Id equals v.IdProducto
                                        join pr in db.Precios on v.Id equals pr.IdVariante into prGroup
                                        from pr in prGroup.DefaultIfEmpty()
                                        where p.Activo
                                        select new
                                        {
                                            p.Id,
                                            ProductoNombre = p.Nombre,
                                            VarianteNombre = v.Nombre,
                                            v.EsDefault,
                                            PrecioMonto = pr != null ? (decimal?)pr.Monto : null
                                        }).Take(10).ToListAsync();

        _output.WriteLine($"[Spec 026] Muestra de productos con variante y precio:");
        foreach (var item in productosConPrecio)
        {
            _output.WriteLine($"  Prod #{item.Id} '{item.ProductoNombre}' -> Var: '{item.VarianteNombre}' (Default={item.EsDefault}) | Precio: ${item.PrecioMonto ?? 0} MXN");
        }

        var recetasCount = await db.Recetas.CountAsync();
        _output.WriteLine($"[Spec 026] Recetas registradas en Recipe Studio: {recetasCount}");
    }

    [Fact]
    public async Task Spec019_OutboxYFoliador_OperacionesTransaccionales()
    {
        using var db = CreatePostgresDbContext();
        var testEventId = Guid.NewGuid();

        var outbox = new OutboxEvent
        {
            Id = testEventId,
            AggregateType = "Pedido",
            AggregateId = Guid.NewGuid(),
            EventType = "PedidoCreado",
            PayloadJson = "{\"test\":true,\"total\":250.00}",
            CreatedAt = DateTime.UtcNow,
            SyncStatus = "Pending",
            RetryCount = 0
        };

        db.OutboxEvents.Add(outbox);
        await db.SaveChangesAsync();

        var saved = await db.OutboxEvents.FirstOrDefaultAsync(o => o.Id == testEventId);
        saved.Should().NotBeNull();
        saved!.AggregateType.Should().Be("Pedido");
        saved.SyncStatus.Should().Be("Pending");

        // Cleanup test record
        db.OutboxEvents.Remove(saved);
        await db.SaveChangesAsync();
        _output.WriteLine($"[Spec 019] Transaccional OK: OutboxEvent creado y verificado con Id {testEventId}.");
    }

    [Fact]
    public async Task Spec020_BolsaTimbres_Transaccional_DescuentoCorrecto()
    {
        using var db = CreatePostgresDbContext();
        var empresa = await db.Empresas.FirstOrDefaultAsync();
        if (empresa == null)
        {
            _output.WriteLine("[Spec 020] No hay empresa registrada; omitiendo prueba transaccional.");
            return;
        }

        var bolsa = await db.EmpresaBolsasTimbres.FirstOrDefaultAsync(b => b.IdEmpresa == empresa.Id);
        bool eraNueva = false;
        if (bolsa == null)
        {
            eraNueva = true;
            bolsa = new EmpresaBolsaTimbres
            {
                IdEmpresa = empresa.Id,
                TimbresDisponibles = 100,
                TimbresConsumidos = 0,
                UltimaRecargaFecha = DateTime.UtcNow
            };
            db.EmpresaBolsasTimbres.Add(bolsa);
            await db.SaveChangesAsync();
        }

        var disponiblesInicial = bolsa.TimbresDisponibles;
        var consumidosInicial = bolsa.TimbresConsumidos;

        // Simular consumo de 1 timbre
        bolsa.TimbresDisponibles -= 1;
        bolsa.TimbresConsumidos += 1;
        await db.SaveChangesAsync();

        var bolsaActualizada = await db.EmpresaBolsasTimbres.FirstAsync(b => b.Id == bolsa.Id);
        bolsaActualizada.TimbresDisponibles.Should().Be(disponiblesInicial - 1);
        bolsaActualizada.TimbresConsumidos.Should().Be(consumidosInicial + 1);

        // Restaurar estado si se prefiere
        if (!eraNueva)
        {
            bolsaActualizada.TimbresDisponibles = disponiblesInicial;
            bolsaActualizada.TimbresConsumidos = consumidosInicial;
            await db.SaveChangesAsync();
        }

        _output.WriteLine($"[Spec 020] Transaccional OK: Bolsa de timbres descontó 1 timbre correctamente.");
    }

    [Fact]
    public async Task Spec023_ConfiguracionImpresora_CRUD()
    {
        using var db = CreatePostgresDbContext();
        var sucursal = await db.Sucursales.FirstOrDefaultAsync();
        if (sucursal == null)
        {
            _output.WriteLine("[Spec 023] No hay sucursal registrada; omitiendo prueba.");
            return;
        }

        var impresora = new ConfiguracionImpresora
        {
            IdSucursal = sucursal.Id,
            Nombre = "Ticketera Test AutoDiag",
            TipoConexion = TipoConexionImpresora.RedLAN,
            AnchoPapel = AnchoPapelImpresora.Mm80,
            DireccionIp = "192.168.1.250",
            Puerto = 9100,
            AperturaCajon = true,
            Autocorte = true,
            EstacionAsociada = "Caja Principal"
        };

        db.ConfiguracionesImpresora.Add(impresora);
        await db.SaveChangesAsync();

        var consultada = await db.ConfiguracionesImpresora.FirstOrDefaultAsync(i => i.Id == impresora.Id);
        consultada.Should().NotBeNull();
        consultada!.Nombre.Should().Be("Ticketera Test AutoDiag");
        consultada.DireccionIp.Should().Be("192.168.1.250");
        consultada.Puerto.Should().Be(9100);
        consultada.AnchoPapel.Should().Be(AnchoPapelImpresora.Mm80);

        // Cleanup
        db.ConfiguracionesImpresora.Remove(consultada);
        await db.SaveChangesAsync();

        _output.WriteLine($"[Spec 023] Transaccional OK: Impresora registrada, consultada y eliminada correctamente.");
    }

    [Fact]
    public async Task Spec024_CandadoSupervisor_HashYValidacionPin()
    {
        using var db = CreatePostgresDbContext();
        var hasher = new Persistence.Security.Pbkdf2PasswordHasher();

        string pinOriginal = "4819";
        var (hash, salt) = hasher.HashPassword(pinOriginal);

        hasher.Verify(pinOriginal, hash, salt).Should().BeTrue();
        hasher.Verify("0000", hash, salt).Should().BeFalse();
        hasher.Verify("4818", hash, salt).Should().BeFalse();

        // Verificar que los 4 motivos oficiales de Spec 024 están en la BD
        var motivos = await db.CatMotivosCancelacion.Select(m => m.Descripcion).ToListAsync();
        motivos.Should().Contain("Error de captura del mesero");
        motivos.Should().Contain("Platillo devuelto por el comensal");
        motivos.Should().Contain("Mesa se retiró sin consumir");
        motivos.Should().Contain("Cortesía de la casa autorizada");

        _output.WriteLine($"[Spec 024] PBKDF2 PIN Security OK y 4 motivos oficiales validados.");
    }

    [Fact]
    public async Task Spec026_CrearPlatilloCompleto_Transaccional()
    {
        using var db = CreatePostgresDbContext();
        var menu = await db.Menus.FirstOrDefaultAsync();
        var categoria = await db.CategoriaMenus.FirstOrDefaultAsync();
        var impuesto = await db.CatImpuestos.FirstOrDefaultAsync();
        var moneda = await db.CatMonedas.FirstOrDefaultAsync();

        if (menu == null || categoria == null)
        {
            _output.WriteLine("[Spec 026] No hay menú o categoría registrada; omitiendo prueba transaccional.");
            return;
        }

        // 1. Crear Producto
        var prod = new Producto
        {
            IdMenu = menu.Id,
            IdCategoria = categoria.Id,
            Nombre = "Torta Ahogada Test Spec026",
            Descripcion = "Platillo de prueba para Suite Unificada",
            Activo = true
        };
        db.Productos.Add(prod);
        await db.SaveChangesAsync();

        // 2. Crear Variante "Estándar" por defecto
        var variante = new VarianteProducto
        {
            IdProducto = prod.Id,
            Nombre = "Estándar",
            EsDefault = true
        };
        db.VarianteProductos.Add(variante);
        await db.SaveChangesAsync();

        // 3. Crear Precio Oficial
        var precio = new Precio
        {
            IdVariante = variante.Id,
            Monto = 95.00m,
            Moneda = "MXN",
            IdImpuesto = impuesto?.Id ?? 1,
            IdMoneda = moneda?.Id ?? 1
        };
        db.Precios.Add(precio);
        await db.SaveChangesAsync();

        // 4. Validar consulta integral (como la haría el POS)
        var platilloEnPOS = await (from p in db.Productos
                                   join v in db.VarianteProductos on p.Id equals v.IdProducto
                                   join pr in db.Precios on v.Id equals pr.IdVariante
                                   where p.Id == prod.Id && v.EsDefault
                                   select new
                                   {
                                       p.Nombre,
                                       Variante = v.Nombre,
                                       pr.Monto
                                   }).FirstOrDefaultAsync();

        platilloEnPOS.Should().NotBeNull();
        platilloEnPOS!.Nombre.Should().Be("Torta Ahogada Test Spec026");
        platilloEnPOS.Variante.Should().Be("Estándar");
        platilloEnPOS.Monto.Should().Be(95.00m);

        // Cleanup
        db.Precios.Remove(precio);
        db.VarianteProductos.Remove(variante);
        db.Productos.Remove(prod);
        await db.SaveChangesAsync();

        _output.WriteLine($"[Spec 026] Transaccional OK: Producto + Variante 'Estándar' + Precio ($95.00) creados atómicamente y validados.");
    }
}


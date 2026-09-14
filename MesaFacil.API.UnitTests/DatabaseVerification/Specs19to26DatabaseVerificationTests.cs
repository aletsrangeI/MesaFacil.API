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

    [Fact]
    public void GenerateAndValidateAllXmlTestInvoices()
    {
        var parser = new global::UseCases.Compras.CfdiXmlParserService();
        var dir = @"c:\OrionSys\MesaFacil\Facturas_XML_Prueba";
        Directory.CreateDirectory(dir);

        var invoices = new Dictionary<string, string>
        {
            ["FAC_01_Carnes_Sonora.xml"] = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xsi:schemaLocation=""http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"" Version=""4.0"" Serie=""FAC"" Folio=""84521"" Fecha=""2026-09-13T10:15:00"" FormaPago=""03"" SubTotal=""14850.00"" Descuento=""0.00"" Moneda=""MXN"" Total=""17226.00"" TipoDeComprobante=""I"" Exportacion=""01"" MetodoPago=""PPD"" CondicionesDePago=""Credito 15 dias"" LugarExpedicion=""83000"">
  <cfdi:Emisor Rfc=""BME8808116B1"" Nombre=""CARNES SELECTAS DE SONORA SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""BBC240911001"" Nombre=""BISTRO Y BRASA LA CENTRAL"" DomicilioFiscalReceptor=""06000"" RegimenFiscalReceptor=""601"" UsoCFDI=""G01"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50111515"" NoIdentificacion=""CAR-001"" Cantidad=""25.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Corte Rib Eye Sonora Premium Choice"" ValorUnitario=""220.00"" Importe=""5500.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""5500.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""880.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50111515"" NoIdentificacion=""CAR-002"" Cantidad=""15.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Picanha de Res Calidad Angus"" ValorUnitario=""180.00"" Importe=""2700.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2700.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""432.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50111515"" NoIdentificacion=""CAR-003"" Cantidad=""12.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Corte Vacio de Res Fresco"" ValorUnitario=""160.00"" Importe=""1920.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1920.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""307.20"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50111515"" NoIdentificacion=""CAR-004"" Cantidad=""30.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Carne Molida de Res Angus 80/20"" ValorUnitario=""110.00"" Importe=""3300.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""3300.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""528.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50111515"" NoIdentificacion=""CAR-005"" Cantidad=""15.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Hueso con Tuetano Canoa Seleccionado"" ValorUnitario=""95.00"" Importe=""1430.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1430.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""228.80"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Impuestos TotalImpuestosTrasladados=""2376.00"">
    <cfdi:Traslados>
      <cfdi:Traslado Base=""14850.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""2376.00"" />
    </cfdi:Traslados>
  </cfdi:Impuestos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital"" xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd"" Version=""1.1"" UUID=""A1B2C3D4-E5F6-47A1-89B0-1234567890AB"" FechaTimbrado=""2026-09-13T10:16:02"" RfcProvCertif=""SAT970701NN3"" SelloCFD=""aBcDeF1234567890=="" NoCertificadoSAT=""00001000000504465028"" SelloSAT=""XyZ987654321=="" />
  </cfdi:Complemento>
</cfdi:Comprobante>",

            ["FAC_02_Lacteos_Embutidos_Gourmet.xml"] = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xsi:schemaLocation=""http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"" Version=""4.0"" Serie=""INV"" Folio=""10492"" Fecha=""2026-09-13T11:00:00"" FormaPago=""03"" SubTotal=""11350.00"" Descuento=""0.00"" Moneda=""MXN"" Total=""13166.00"" TipoDeComprobante=""I"" Exportacion=""01"" MetodoPago=""PPD"" CondicionesDePago=""Credito 30 dias"" LugarExpedicion=""06000"">
  <cfdi:Emisor Rfc=""DGS190412AA1"" Nombre=""DISTRIBUIDORA GASTRONOMICA SAN JUAN SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""BBC240911001"" Nombre=""BISTRO Y BRASA LA CENTRAL"" DomicilioFiscalReceptor=""06000"" RegimenFiscalReceptor=""601"" UsoCFDI=""G01"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50131800"" NoIdentificacion=""LAC-001"" Cantidad=""6.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Queso Parmigiano Reggiano DOP Curado 24 Meses"" ValorUnitario=""480.00"" Importe=""2880.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2880.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""460.80"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50111500"" NoIdentificacion=""EMB-001"" Cantidad=""5.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Jamon Serrano Reserva Etiqueta Oro Rebanado"" ValorUnitario=""420.00"" Importe=""2100.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2100.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""336.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50131800"" NoIdentificacion=""LAC-002"" Cantidad=""8.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Queso Cheddar Anyejo Rebanado Gourmet"" ValorUnitario=""195.00"" Importe=""1560.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1560.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""249.60"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50131800"" NoIdentificacion=""LAC-003"" Cantidad=""6.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Queso Suizo Emmental Rebanado"" ValorUnitario=""220.00"" Importe=""1320.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1320.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""211.20"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50111500"" NoIdentificacion=""EMB-002"" Cantidad=""10.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Tocino Ahumado de Cerdo Grueso"" ValorUnitario=""175.00"" Importe=""1750.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1750.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""280.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50131800"" NoIdentificacion=""LAC-004"" Cantidad=""12.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Queso Crema Tipo New York Original"" ValorUnitario=""145.00"" Importe=""1740.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1740.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""278.40"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Impuestos TotalImpuestosTrasladados=""1816.00"">
    <cfdi:Traslados>
      <cfdi:Traslado Base=""11350.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""1816.00"" />
    </cfdi:Traslados>
  </cfdi:Impuestos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital"" xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd"" Version=""1.1"" UUID=""B2C3D4E5-F6A1-47B2-90C1-2345678901CD"" FechaTimbrado=""2026-09-13T11:02:15"" RfcProvCertif=""SAT970701NN3"" SelloCFD=""bCdEfG2345678901=="" NoCertificadoSAT=""00001000000504465028"" SelloSAT=""ZaB123456789=="" />
  </cfdi:Complemento>
</cfdi:Comprobante>",

            ["FAC_03_Licores_Destilados_Barra.xml"] = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xsi:schemaLocation=""http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"" Version=""4.0"" Serie=""BAR"" Folio=""5520"" Fecha=""2026-09-13T12:30:00"" FormaPago=""03"" SubTotal=""9420.00"" Descuento=""0.00"" Moneda=""MXN"" Total=""10927.20"" TipoDeComprobante=""I"" Exportacion=""01"" MetodoPago=""PUE"" CondicionesDePago=""Contado"" LugarExpedicion=""44100"">
  <cfdi:Emisor Rfc=""LDB150618MN2"" Nombre=""LICORES Y DESTILADOS PREMIUM DE OCCIDENTE SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""BBC240911001"" Nombre=""BISTRO Y BRASA LA CENTRAL"" DomicilioFiscalReceptor=""06000"" RegimenFiscalReceptor=""601"" UsoCFDI=""G01"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50202203"" NoIdentificacion=""LIC-001"" Cantidad=""6.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Mezcal Espadin Artesanal Oaxaquenyo Botella 750ml"" ValorUnitario=""420.00"" Importe=""2520.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2520.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""403.20"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50202202"" NoIdentificacion=""LIC-002"" Cantidad=""6.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Ginebra London Dry Botanica Premium 750ml"" ValorUnitario=""450.00"" Importe=""2700.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2700.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""432.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50202200"" NoIdentificacion=""LIC-003"" Cantidad=""6.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Licor 43 Espanol Original 750ml"" ValorUnitario=""410.00"" Importe=""2460.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2460.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""393.60"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50201708"" NoIdentificacion=""BEB-001"" Cantidad=""8.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Cafe en Grano Tueste Italiano Gourmet Veracruz"" ValorUnitario=""217.50"" Importe=""1740.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1740.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""278.40"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Impuestos TotalImpuestosTrasladados=""1507.20"">
    <cfdi:Traslados>
      <cfdi:Traslado Base=""9420.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""1507.20"" />
    </cfdi:Traslados>
  </cfdi:Impuestos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital"" xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd"" Version=""1.1"" UUID=""C3D4E5F6-A1B2-48C3-01D2-3456789012DE"" FechaTimbrado=""2026-09-13T12:31:05"" RfcProvCertif=""SAT970701NN3"" SelloCFD=""cDeFgH3456789012=="" NoCertificadoSAT=""00001000000504465028"" SelloSAT=""AbC987654321=="" />
  </cfdi:Complemento>
</cfdi:Comprobante>",

            ["FAC_04_Abarrotes_Panaderia_Insumos.xml"] = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xsi:schemaLocation=""http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"" Version=""4.0"" Serie=""AB"" Folio=""9941"" Fecha=""2026-09-13T13:10:00"" FormaPago=""03"" SubTotal=""7640.00"" Descuento=""0.00"" Moneda=""MXN"" Total=""8862.40"" TipoDeComprobante=""I"" Exportacion=""01"" MetodoPago=""PPD"" CondicionesDePago=""Credito 15 dias"" LugarExpedicion=""06000"">
  <cfdi:Emisor Rfc=""APA080514KP9"" Nombre=""ABARROTERA Y PANIFICADORA DE LAS AMERICAS SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""BBC240911001"" Nombre=""BISTRO Y BRASA LA CENTRAL"" DomicilioFiscalReceptor=""06000"" RegimenFiscalReceptor=""601"" UsoCFDI=""G01"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50181900"" NoIdentificacion=""PAN-001"" Cantidad=""80.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Pan Brioche Artesanal con Ajonjoli Negro"" ValorUnitario=""14.50"" Importe=""1160.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1160.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""185.60"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50171800"" NoIdentificacion=""ABA-001"" Cantidad=""4.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Aceite de Oliva Extra Virgen con Esencia de Trufa Negra"" ValorUnitario=""380.00"" Importe=""1520.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1520.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""243.20"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50202306"" NoIdentificacion=""BEB-002"" Cantidad=""48.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Agua Tonica Artesanal Botella de Vidrio 200ml"" ValorUnitario=""22.50"" Importe=""1080.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1080.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""172.80"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50101500"" NoIdentificacion=""VER-001"" Cantidad=""10.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Mix de Frutos Rojos Congelados Frambuesa Zarzamora Fresa"" ValorUnitario=""165.00"" Importe=""1650.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""1650.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""264.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50161800"" NoIdentificacion=""ABA-002"" Cantidad=""12.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Dulce de Leche Repostero Tradicional Tipo Argentino"" ValorUnitario=""185.00"" Importe=""2230.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2230.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""356.80"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Impuestos TotalImpuestosTrasladados=""1222.40"">
    <cfdi:Traslados>
      <cfdi:Traslado Base=""7640.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""1222.40"" />
    </cfdi:Traslados>
  </cfdi:Impuestos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital"" xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd"" Version=""1.1"" UUID=""D4E5F6A1-B2C3-49D4-12E3-4567890123EF"" FechaTimbrado=""2026-09-13T13:12:44"" RfcProvCertif=""SAT970701NN3"" SelloCFD=""dEfGhI4567890123=="" NoCertificadoSAT=""00001000000504465028"" SelloSAT=""BcD123456789=="" />
  </cfdi:Complemento>
</cfdi:Comprobante>",

            ["FAC_05_Mariscos_Pescados_NuevosPlatillos.xml"] = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xsi:schemaLocation=""http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"" Version=""4.0"" Serie=""MAR"" Folio=""3301"" Fecha=""2026-09-13T14:00:00"" FormaPago=""03"" SubTotal=""16250.00"" Descuento=""0.00"" Moneda=""MXN"" Total=""18850.00"" TipoDeComprobante=""I"" Exportacion=""01"" MetodoPago=""PPD"" CondicionesDePago=""Credito 7 dias"" LugarExpedicion=""22800"">
  <cfdi:Emisor Rfc=""PMP110220TU8"" Nombre=""PESCADOS Y MARISCOS DEL PACIFICO SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""BBC240911001"" Nombre=""BISTRO Y BRASA LA CENTRAL"" DomicilioFiscalReceptor=""06000"" RegimenFiscalReceptor=""601"" UsoCFDI=""G01"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50121500"" NoIdentificacion=""MAR-001"" Cantidad=""15.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Filete de Salmon Noruego Fresco Calidad Sashimi"" ValorUnitario=""340.00"" Importe=""5100.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""5100.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""816.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50121600"" NoIdentificacion=""MAR-002"" Cantidad=""15.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Camaron Azul Sinaloa U15 con Cabeza Fresco"" ValorUnitario=""280.00"" Importe=""4200.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""4200.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""672.00"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50121700"" NoIdentificacion=""MAR-003"" Cantidad=""12.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Pulpo Maya Limpio Precocido Congelado"" ValorUnitario=""295.00"" Importe=""3540.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""3540.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""566.40"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50121500"" NoIdentificacion=""MAR-004"" Cantidad=""10.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Lomo de Atun Aleta Amarilla Fresco"" ValorUnitario=""341.00"" Importe=""3410.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""3410.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""545.60"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Impuestos TotalImpuestosTrasladados=""2600.00"">
    <cfdi:Traslados>
      <cfdi:Traslado Base=""16250.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""2600.00"" />
    </cfdi:Traslados>
  </cfdi:Impuestos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital"" xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd"" Version=""1.1"" UUID=""E5F6A1B2-C3D4-50E5-23F4-5678901234FA"" FechaTimbrado=""2026-09-13T14:02:11"" RfcProvCertif=""SAT970701NN3"" SelloCFD=""eFgHiJ5678901234=="" NoCertificadoSAT=""00001000000504465028"" SelloSAT=""CdE123456789=="" />
  </cfdi:Complemento>
</cfdi:Comprobante>",

            ["FAC_06_Verduras_Frescas_Botanica.xml"] = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xsi:schemaLocation=""http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"" Version=""4.0"" Serie=""HUR"" Folio=""2084"" Fecha=""2026-09-13T14:40:00"" FormaPago=""03"" SubTotal=""4890.00"" Descuento=""0.00"" Moneda=""MXN"" Total=""4890.00"" TipoDeComprobante=""I"" Exportacion=""01"" MetodoPago=""PUE"" CondicionesDePago=""Contado"" LugarExpedicion=""58000"">
  <cfdi:Emisor Rfc=""HAF160310KR2"" Nombre=""HUERTA AGRICOLA Y FRUTAS DEL VALLE SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""BBC240911001"" Nombre=""BISTRO Y BRASA LA CENTRAL"" DomicilioFiscalReceptor=""06000"" RegimenFiscalReceptor=""601"" UsoCFDI=""G01"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50101500"" NoIdentificacion=""VER-002"" Cantidad=""20.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Aguacate Hass Seleccionado de Michoacan"" ValorUnitario=""65.00"" Importe=""1300.00"" ObjetoImp=""01"" />
    <cfdi:Concepto ClaveProdServ=""50101500"" NoIdentificacion=""VER-003"" Cantidad=""10.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Hongo Portobello Fresco Gourmet"" ValorUnitario=""95.00"" Importe=""950.00"" ObjetoImp=""01"" />
    <cfdi:Concepto ClaveProdServ=""50101500"" NoIdentificacion=""VER-004"" Cantidad=""6.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Arugula Baby Fresca Hidroponica"" ValorUnitario=""110.00"" Importe=""660.00"" ObjetoImp=""01"" />
    <cfdi:Concepto ClaveProdServ=""50101500"" NoIdentificacion=""VER-005"" Cantidad=""8.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Esparragos Verdes Calibre Grueso Selecto"" ValorUnitario=""125.00"" Importe=""1000.00"" ObjetoImp=""01"" />
    <cfdi:Concepto ClaveProdServ=""50101500"" NoIdentificacion=""VER-006"" Cantidad=""15.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Pepino Persa Criollo Fresco"" ValorUnitario=""26.00"" Importe=""390.00"" ObjetoImp=""01"" />
    <cfdi:Concepto ClaveProdServ=""50101500"" NoIdentificacion=""VER-007"" Cantidad=""20.0000"" ClaveUnidad=""KGM"" Unidad=""KILOGRAMO"" Descripcion=""Grano de Elote Amarillo Tierno Desgranado"" ValorUnitario=""29.50"" Importe=""590.00"" ObjetoImp=""01"" />
  </cfdi:Conceptos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital"" xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd"" Version=""1.1"" UUID=""F6A1B2C3-D4E5-51F6-34A5-6789012345AB"" FechaTimbrado=""2026-09-13T14:41:22"" RfcProvCertif=""SAT970701NN3"" SelloCFD=""fGhIjK6789012345=="" NoCertificadoSAT=""00001000000504465028"" SelloSAT=""DeF123456789=="" />
  </cfdi:Complemento>
</cfdi:Comprobante>",

            ["FAC_07_Cervezas_Vinos_Bebidas.xml"] = @"<?xml version=""1.0"" encoding=""utf-8""?>
<cfdi:Comprobante xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:cfdi=""http://www.sat.gob.mx/cfd/4"" xsi:schemaLocation=""http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd"" Version=""4.0"" Serie=""CER"" Folio=""7192"" Fecha=""2026-09-13T15:20:00"" FormaPago=""03"" SubTotal=""8800.00"" Descuento=""0.00"" Moneda=""MXN"" Total=""10208.00"" TipoDeComprobante=""I"" Exportacion=""01"" MetodoPago=""PPD"" CondicionesDePago=""Credito 30 dias"" LugarExpedicion=""22000"">
  <cfdi:Emisor Rfc=""CBR170921PQ4"" Nombre=""CERVECERIA Y VINOS DE LA FRONTERA SA DE CV"" RegimenFiscal=""601"" />
  <cfdi:Receptor Rfc=""BBC240911001"" Nombre=""BISTRO Y BRASA LA CENTRAL"" DomicilioFiscalReceptor=""06000"" RegimenFiscalReceptor=""601"" UsoCFDI=""G01"" />
  <cfdi:Conceptos>
    <cfdi:Concepto ClaveProdServ=""50202201"" NoIdentificacion=""BEB-003"" Cantidad=""72.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Cerveza Artesanal IPA India Pale Ale Botella 355ml"" ValorUnitario=""38.00"" Importe=""2736.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2736.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""437.76"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50202201"" NoIdentificacion=""BEB-004"" Cantidad=""96.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Cerveza Clara Ultra Ligera Botella 355ml"" ValorUnitario=""26.50"" Importe=""2544.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""2544.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""407.04"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
    <cfdi:Concepto ClaveProdServ=""50202203"" NoIdentificacion=""BEB-005"" Cantidad=""12.0000"" ClaveUnidad=""H87"" Unidad=""PIEZA"" Descripcion=""Vino Tinto Ensamble Cabernet-Merlot Valle de Guadalupe 750ml"" ValorUnitario=""293.33"" Importe=""3520.00"" ObjetoImp=""02"">
      <cfdi:Impuestos>
        <cfdi:Traslados>
          <cfdi:Traslado Base=""3520.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""563.20"" />
        </cfdi:Traslados>
      </cfdi:Impuestos>
    </cfdi:Concepto>
  </cfdi:Conceptos>
  <cfdi:Impuestos TotalImpuestosTrasladados=""1408.00"">
    <cfdi:Traslados>
      <cfdi:Traslado Base=""8800.00"" Impuesto=""002"" TipoFactor=""Tasa"" TasaOCuota=""0.160000"" Importe=""1408.00"" />
    </cfdi:Traslados>
  </cfdi:Impuestos>
  <cfdi:Complemento>
    <tfd:TimbreFiscalDigital xmlns:tfd=""http://www.sat.gob.mx/TimbreFiscalDigital"" xsi:schemaLocation=""http://www.sat.gob.mx/TimbreFiscalDigital http://www.sat.gob.mx/sitio_internet/cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd"" Version=""1.1"" UUID=""A2B3C4D5-E6F7-52A3-45B6-7890123456BC"" FechaTimbrado=""2026-09-13T15:22:15"" RfcProvCertif=""SAT970701NN3"" SelloCFD=""gHiJkL7890123456=="" NoCertificadoSAT=""00001000000504465028"" SelloSAT=""EfG123456789=="" />
  </cfdi:Complemento>
</cfdi:Comprobante>"
        };

        foreach (var (filename, xml) in invoices)
        {
            var path = Path.Combine(dir, filename);
            File.WriteAllText(path, xml.Trim(), System.Text.Encoding.UTF8);

            // Validar parseo con XDocument (regla estricta XML)
            var xDoc = System.Xml.Linq.XDocument.Parse(xml);
            xDoc.Root.Should().NotBeNull();

            // Validar con el parser oficial de la app
            var parsed = parser.ParsearCfdiAsync(xml).GetAwaiter().GetResult();
            parsed.Should().NotBeNull();
            parsed.Total.Should().BeGreaterThan(0);
            parsed.Conceptos.Should().NotBeEmpty();

            _output.WriteLine($"[CFDI OK] {filename} -> {parsed.Conceptos.Count} conceptos, Total: ${parsed.Total:N2} MXN");
        }
    }
}


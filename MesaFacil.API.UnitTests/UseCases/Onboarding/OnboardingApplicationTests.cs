using Common;
using Domain.Entities;
using DTO.Onboarding;
using FluentAssertions;
using Interface.Persistence;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Onboarding;
using Validator;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Onboarding;

public class OnboardingApplicationTests
{
    private static ApplicationDbContext CrearContextoEnMemoria(string? nombreBd = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nombreBd ?? $"MesaFacil_Onboarding_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    private static async Task SembrarCatalogosBaseAsync(ApplicationDbContext context)
    {
        if (!await context.CatEstadosMesa.AnyAsync())
        {
            context.CatEstadosMesa.AddRange(
                new CatEstadoMesa { Id = 1, Descripcion = "Disponible", IsActive = true },
                new CatEstadoMesa { Id = 2, Descripcion = "Ocupada", IsActive = true }
            );
        }

        if (!await context.CatMonedas.AnyAsync())
        {
            context.CatMonedas.Add(new CatMoneda { Id = 1, Descripcion = "MXN", IsActive = true });
        }

        if (!await context.CatImpuestos.AnyAsync())
        {
            context.CatImpuestos.Add(new CatImpuesto { Id = 1, Descripcion = "IVA 16%", IsActive = true });
        }

        if (!await context.CatCredenciales.AnyAsync())
        {
            context.CatCredenciales.AddRange(
                new CatCredencial { Id = 1, Descripcion = "PASSWORD", IsActive = true },
                new CatCredencial { Id = 2, Descripcion = "PIN", IsActive = true }
            );
        }

        if (!await context.Roles.AnyAsync())
        {
            context.Roles.AddRange(
                new Rol { Id = 1, Nombre = "Admin", ConcurrencyStamp = Guid.NewGuid().ToString(), IsActive = true },
                new Rol { Id = 2, Nombre = "Manager", ConcurrencyStamp = Guid.NewGuid().ToString(), IsActive = true },
                new Rol { Id = 3, Nombre = "Mesero", ConcurrencyStamp = Guid.NewGuid().ToString(), IsActive = true }
            );
        }

        await context.SaveChangesAsync();
    }

    private static (OnboardingApplication app, ApplicationDbContext context) CrearApp(string? nombreBd = null)
    {
        var context = CrearContextoEnMemoria(nombreBd);
        var mockHasher = new Mock<IPasswordHasher>();
        mockHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns(("dummy_hash", "dummy_salt"));

        var mockLogger = new Mock<IAppLogger<OnboardingApplication>>();
        var validator = new ProvisionarRestauranteValidator();

        var app = new OnboardingApplication(context, mockHasher.Object, mockLogger.Object, validator);
        return (app, context);
    }

    [Fact]
    public async Task ObtenerEstadoAsync_EmpresaVacia_RetornaNoCompletado()
    {
        var (app, context) = CrearApp();
        await SembrarCatalogosBaseAsync(context);

        var empresa = new Empresa { Nombre = "Restaurante Nuevo", IsActive = true };
        context.Empresas.Add(empresa);
        await context.SaveChangesAsync();

        var response = await app.ObtenerEstadoAsync(empresa.Id);

        response.isSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.OnboardingCompletado.Should().BeFalse();
        response.Data.TieneSucursales.Should().BeFalse();
        response.Data.TotalMesas.Should().Be(0);
        response.Data.PasoSugerido.Should().Be(1);
    }

    [Fact]
    public async Task ProvisionarRestauranteAsync_SolicitudValida_CreaEstructuraCompleta()
    {
        var (app, context) = CrearApp();
        await SembrarCatalogosBaseAsync(context);

        // Usuario Administrador inicial
        var admin = new Usuario { NombreCompleto = "Mario Dueño", Correo = "admin@parrilla.com", IsActive = true };
        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();

        var request = new ProvisionarRestauranteRequestDTO
        {
            DatosEmpresa = new DatosEmpresaDTO
            {
                Nombre = "La Parrilla de Mario",
                Rfc = "GOMM850101AA1",
                NombreSucursal = "Sucursal Matriz",
                Direccion = "Av. Hidalgo 123",
                ZonaHoraria = "America/Mexico_City",
                Moneda = "MXN",
                TasaIva = 16.0m
            },
            EstacionesCocina = new List<EstacionCocinaOnboardingDTO>
            {
                new() { Nombre = "Cocina Caliente", MinutosAmbar = 7, MinutosRojo = 12 },
                new() { Nombre = "Barra", MinutosAmbar = 4, MinutosRojo = 8 }
            },
            AreasYMesas = new List<AreaYMesasOnboardingDTO>
            {
                new() { NombreArea = "Salón Principal", Orden = 1, PrefijoMesa = "M", CantidadMesas = 6, AsientosPorMesa = 4 },
                new() { NombreArea = "Terraza", Orden = 2, PrefijoMesa = "T", CantidadMesas = 4, AsientosPorMesa = 2 }
            },
            Menu = new MenuOnboardingDTO
            {
                NombreMenu = "Menú General",
                Categorias = new List<string> { "Platos Fuertes", "Bebidas" },
                Productos = new List<ProductoOnboardingDTO>
                {
                    new() { Nombre = "Corte Ribeye", NombreCategoria = "Platos Fuertes", Precio = 350.00m, EstacionCocina = "Cocina Caliente" },
                    new() { Nombre = "Limonada Natural", NombreCategoria = "Bebidas", Precio = 45.00m, EstacionCocina = "Barra" }
                },
                GruposModificador = new List<GrupoModificadorOnboardingDTO>
                {
                    new()
                    {
                        NombreProducto = "Corte Ribeye",
                        NombreGrupo = "Término de la carne",
                        Obligatorio = true,
                        MinSeleccion = 1,
                        MaxSeleccion = 1,
                        Opciones = new List<OpcionModificadorOnboardingDTO>
                        {
                            new() { Nombre = "Término Medio", PrecioExtra = 0, EsDefault = true },
                            new() { Nombre = "Bien Cocido", PrecioExtra = 0, EsDefault = false }
                        }
                    }
                }
            },
            Personal = new List<PersonalOnboardingDTO>
            {
                new() { NombreCompleto = "Carlos Mesero", Rol = "Mesero", Pin = "1234" },
                new() { NombreCompleto = "Laura Cajera", Rol = "Manager", Pin = "5678" }
            },
            PinSupervisorAdmin = "9999",
            AbrirTurnoInicial = true,
            FondoCajaInicial = 1500.00m
        };

        var response = await app.ProvisionarRestauranteAsync(request, admin.Id, 0);

        // Aserciones de respuesta
        response.isSuccess.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.MesasCreadas.Should().Be(10); // 6 + 4
        response.Data.ProductosCreados.Should().Be(2);
        response.Data.UsuariosCreados.Should().Be(2);
        response.Data.TurnoId.Should().NotBeNull();
        response.Data.RutaRedirect.Should().Be("/ventas/pos");

        // Validaciones en Base de Datos
        var empresa = await context.Empresas.FirstOrDefaultAsync(e => e.Nombre == "La Parrilla de Mario");
        empresa.Should().NotBeNull();

        var sucursal = await context.Sucursales.FirstOrDefaultAsync(s => s.IdEmpresa == empresa!.Id);
        sucursal.Should().NotBeNull();
        sucursal!.Nombre.Should().Be("Sucursal Matriz");

        // Almacén General y Foliador
        var almacen = await context.Almacenes.FirstOrDefaultAsync(a => a.IdSucursal == sucursal.Id);
        almacen.Should().NotBeNull();
        almacen!.EsPrincipal.Should().BeTrue();

        var foliador = await context.FoliadoresSucursal.FirstOrDefaultAsync(f => f.IdSucursal == sucursal.Id);
        foliador.Should().NotBeNull();

        // Estaciones de cocina
        var estaciones = await context.EstacionesCocina.Where(e => e.IdSucursal == sucursal.Id).ToListAsync();
        estaciones.Should().HaveCount(2);

        // Mesas y Áreas
        var areas = await context.Areas.Where(a => a.IdSucursal == sucursal.Id).ToListAsync();
        areas.Should().HaveCount(2);

        var mesas = await context.Mesas.Where(m => m.IdSucursal == sucursal.Id).ToListAsync();
        mesas.Should().HaveCount(10);
        mesas.All(m => m.IdEstadoMesa == 1).Should().BeTrue(); // Todas Disponibles

        // Productos, Variantes y Precios
        var productos = await context.Productos.ToListAsync();
        productos.Should().HaveCount(2);

        var variantes = await context.VarianteProductos.ToListAsync();
        variantes.Should().HaveCount(2);
        variantes.All(v => v.Nombre == "Estándar" && v.EsDefault).Should().BeTrue();

        var precios = await context.Precios.ToListAsync();
        precios.Should().HaveCount(2);
        precios.First(p => p.Monto == 350.00m).Moneda.Should().Be("MXN");

        // Modificadores
        var grupo = await context.GruposModificador.FirstOrDefaultAsync(g => g.Nombre == "Término de la carne");
        grupo.Should().NotBeNull();
        grupo!.Obligatorio.Should().BeTrue();

        var opciones = await context.OpcionesModificador.Where(o => o.IdGrupo == grupo.Id).ToListAsync();
        opciones.Should().HaveCount(2);

        // Personal y Credenciales con PIN
        var meseros = await context.Usuarios.Where(u => u.IdEmpresa == empresa!.Id && u.NombreCompleto != "Mario Dueño").ToListAsync();
        meseros.Should().HaveCount(2);

        var credencialesPin = await context.Credenciales.Where(c => c.IdCredencial == 2).ToListAsync();
        credencialesPin.Should().HaveCount(2);

        // PIN Supervisor en Admin
        var adminDb = await context.Usuarios.FirstAsync(u => u.Id == admin.Id);
        adminDb.PinSupervisorHash.Should().Be("dummy_hash");

        // Turno Abierto
        var turno = await context.Turnos.FirstOrDefaultAsync(t => t.IdSucursal == sucursal.Id);
        turno.Should().NotBeNull();
        turno!.Cierre.Should().BeNull();
        turno.CajaInicial.Should().Be(1500.00m);
    }

    [Fact]
    public async Task ProvisionarRestauranteAsync_ValidacionFalla_SiNoHayAreas()
    {
        var (app, context) = CrearApp();
        await SembrarCatalogosBaseAsync(context);

        var requestInvalido = new ProvisionarRestauranteRequestDTO
        {
            DatosEmpresa = new DatosEmpresaDTO { Nombre = "Restaurante Sin Zonas" },
            AreasYMesas = new List<AreaYMesasOnboardingDTO>() // Vacío
        };

        var response = await app.ProvisionarRestauranteAsync(requestInvalido, 1, 0);

        response.isSuccess.Should().BeFalse();
        response.Message.Should().Contain("Errores de validación");
        response.Errors.Should().NotBeEmpty();
    }
}

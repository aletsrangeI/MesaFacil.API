using Domain.Entities;
using DTO.Seguridad;
using FluentAssertions;
using Interface.UseCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using Persistence.Interceptors;
using Persistence.Security;
using UseCases.Seguridad;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Seguridad;

/// <summary>
/// Spec 024: Candado de Supervisor (PIN 4 dígitos). Cubre: hash correcto/incorrecto,
/// bloqueo tras 3 intentos fallidos y desbloqueo tras 5 minutos (simulado moviendo
/// PinBloqueadoHasta al pasado, ya que el servicio no depende de un reloj inyectable).
/// </summary>
public class PinValidationSecurityTests
{
    private static ApplicationDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MesaFacil_PinSecurity_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    private static IConfiguration CrearConfiguracionJwt()
    {
        var datos = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "MesaFacilTests",
            ["Jwt:Audience"] = "MesaFacilTestsAudience",
            ["Jwt:SigningKey"] = "clave-de-pruebas-super-larga-para-firmar-jwt-1234567890"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(datos).Build();
    }

    private static async Task<(ApplicationDbContext db, Empresa empresa, Usuario supervisor, Pedido pedido)> PrepararEscenarioAsync(
        ApplicationDbContext db, string pinConfigurado = "1234")
    {
        var empresa = new Empresa { Nombre = "Restaurante de Prueba", Rfc = "RPR010101AA1", IsActive = true };
        db.Empresas.Add(empresa);
        await db.SaveChangesAsync();

        var rolManager = new Rol { Nombre = "Manager", IsSystem = false, IsAssignable = true, ConcurrencyStamp = Guid.NewGuid().ToString(), IsActive = true };
        db.Roles.Add(rolManager);
        await db.SaveChangesAsync();

        var hasher = new Pbkdf2PasswordHasher();
        var (hash, salt) = hasher.HashPassword(pinConfigurado);

        var supervisor = new Usuario
        {
            IdEmpresa = empresa.Id,
            NombreCompleto = "Mariana Gómez",
            Correo = "mariana@demo.local",
            IsActive = true,
            PinSupervisorHash = hash,
            PinSupervisorSalt = salt,
            PinIntentosFallidos = 0,
            PinBloqueadoHasta = null
        };
        db.Usuarios.Add(supervisor);
        await db.SaveChangesAsync();

        db.UsuarioRoles.Add(new UsuarioRol { UsuarioId = supervisor.Id, IdRol = rolManager.Id, IsActive = true });
        await db.SaveChangesAsync();

        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            IdEmpresa = empresa.Id,
            IdSucursal = 1,
            IdTipoPedido = 1,
            IdEstadoPedido = 1
        };
        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync();

        return (db, empresa, supervisor, pedido);
    }

    [Fact]
    public async Task ConfigurarPinAsync_UsuarioConRolSupervisor_GuardaHashYSalt()
    {
        using var db = CrearContexto();
        var (context, _, supervisor, _) = await PrepararEscenarioAsync(db, pinConfigurado: "0000");
        var sut = new SupervisorPinSecurityService(context, new Pbkdf2PasswordHasher(), CrearConfiguracionJwt());

        // Limpiamos el PIN pre-cargado para validar el flujo de configuración desde cero.
        supervisor.PinSupervisorHash = null;
        supervisor.PinSupervisorSalt = null;
        await context.SaveChangesAsync();

        var resultado = await sut.ConfigurarPinAsync(new ConfigurarPinRequestDTO { IdUsuario = supervisor.Id, NuevoPin = "4819" }, supervisor.Id);

        resultado.isSuccess.Should().BeTrue();
        var actualizado = await context.Usuarios.FindAsync(supervisor.Id);
        actualizado!.PinSupervisorHash.Should().NotBeNullOrEmpty();
        actualizado.PinSupervisorSalt.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("abcd")]
    public async Task ConfigurarPinAsync_ConPinInvalido_Rechaza(string pinInvalido)
    {
        using var db = CrearContexto();
        var (context, _, supervisor, _) = await PrepararEscenarioAsync(db);
        var sut = new SupervisorPinSecurityService(context, new Pbkdf2PasswordHasher(), CrearConfiguracionJwt());

        var resultado = await sut.ConfigurarPinAsync(new ConfigurarPinRequestDTO { IdUsuario = supervisor.Id, NuevoPin = pinInvalido }, supervisor.Id);

        resultado.isSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AutorizarAsync_ConPinCorrecto_RetornaAutorizadoYTokenValido()
    {
        using var db = CrearContexto();
        var (context, _, supervisor, pedido) = await PrepararEscenarioAsync(db, pinConfigurado: "1234");
        var sut = new SupervisorPinSecurityService(context, new Pbkdf2PasswordHasher(), CrearConfiguracionJwt());

        var idPedidoDetalle = Guid.NewGuid();
        var resultado = await sut.AutorizarAsync(new AutorizarSupervisorPinRequestDTO
        {
            Pin = "1234",
            AccionProtegida = "CancelarPlatilloCocina",
            IdPedido = pedido.Id,
            IdPedidoDetalle = idPedidoDetalle,
            Motivo = "Error de captura del mesero"
        });

        resultado.Autorizado.Should().BeTrue();
        resultado.SupervisorId.Should().Be(supervisor.Id);
        resultado.TokenAutorizacion.Should().NotBeNullOrEmpty();

        var tokenValido = sut.ValidarTokenAutorizacion(resultado.TokenAutorizacion, "CancelarPlatilloCocina", idPedidoDetalle, out var idSupervisorDelToken);
        tokenValido.Should().BeTrue();
        idSupervisorDelToken.Should().Be(supervisor.Id);
    }

    [Fact]
    public async Task AutorizarAsync_ConPinIncorrecto_RetornaNoAutorizado()
    {
        using var db = CrearContexto();
        var (context, _, _, pedido) = await PrepararEscenarioAsync(db, pinConfigurado: "1234");
        var sut = new SupervisorPinSecurityService(context, new Pbkdf2PasswordHasher(), CrearConfiguracionJwt());

        var resultado = await sut.AutorizarAsync(new AutorizarSupervisorPinRequestDTO
        {
            Pin = "9999",
            AccionProtegida = "CancelarPlatilloCocina",
            IdPedido = pedido.Id,
            IdPedidoDetalle = Guid.NewGuid(),
            Motivo = "Prueba"
        });

        resultado.Autorizado.Should().BeFalse();
        resultado.TokenAutorizacion.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task AutorizarAsync_TrasTresIntentosFallidos_BloqueaAlSupervisorPorCincoMinutos()
    {
        using var db = CrearContexto();
        var (context, _, supervisor, pedido) = await PrepararEscenarioAsync(db, pinConfigurado: "1234");
        var sut = new SupervisorPinSecurityService(context, new Pbkdf2PasswordHasher(), CrearConfiguracionJwt());

        var request = new AutorizarSupervisorPinRequestDTO
        {
            Pin = "0000",
            AccionProtegida = "CancelarPlatilloCocina",
            IdPedido = pedido.Id,
            IdPedidoDetalle = Guid.NewGuid(),
            Motivo = "Prueba"
        };

        // 2 intentos fallidos: aún no debe bloquear.
        await sut.AutorizarAsync(request);
        await sut.AutorizarAsync(request);
        var supervisorTrasDosIntentos = await context.Usuarios.AsNoTracking().FirstAsync(u => u.Id == supervisor.Id);
        supervisorTrasDosIntentos.PinBloqueadoHasta.Should().BeNull();

        // 3er intento fallido: bloquea 5 minutos y resetea el contador.
        var resultado = await sut.AutorizarAsync(request);

        resultado.Autorizado.Should().BeFalse();
        resultado.Bloqueado.Should().BeTrue();

        var supervisorBloqueado = await context.Usuarios.AsNoTracking().FirstAsync(u => u.Id == supervisor.Id);
        supervisorBloqueado.PinBloqueadoHasta.Should().NotBeNull();
        supervisorBloqueado.PinBloqueadoHasta!.Value.Should().BeAfter(DateTime.UtcNow);
        supervisorBloqueado.PinIntentosFallidos.Should().Be(0);

        // Mientras esté bloqueado, incluso el PIN correcto es rechazado.
        var intentoConPinCorrectoMientrasBloqueado = await sut.AutorizarAsync(new AutorizarSupervisorPinRequestDTO
        {
            Pin = "1234",
            AccionProtegida = request.AccionProtegida,
            IdPedido = request.IdPedido,
            IdPedidoDetalle = request.IdPedidoDetalle,
            Motivo = request.Motivo
        });
        intentoConPinCorrectoMientrasBloqueado.Autorizado.Should().BeFalse();
        intentoConPinCorrectoMientrasBloqueado.Bloqueado.Should().BeTrue();
    }

    [Fact]
    public async Task AutorizarAsync_DespuesDeSimularQuePasaronCincoMinutos_PermiteAutorizarDeNuevo()
    {
        using var db = CrearContexto();
        var (context, _, supervisor, pedido) = await PrepararEscenarioAsync(db, pinConfigurado: "1234");

        // Simula el estado "recién bloqueado" directamente en BD.
        supervisor.PinBloqueadoHasta = DateTime.UtcNow.AddMinutes(5);
        supervisor.PinIntentosFallidos = 0;
        await context.SaveChangesAsync();

        var sut = new SupervisorPinSecurityService(context, new Pbkdf2PasswordHasher(), CrearConfiguracionJwt());

        // Aún bloqueado: debe rechazar sin siquiera validar el PIN.
        var mientrasBloqueado = await sut.AutorizarAsync(new AutorizarSupervisorPinRequestDTO
        {
            Pin = "1234",
            AccionProtegida = "CancelarPlatilloCocina",
            IdPedido = pedido.Id,
            IdPedidoDetalle = Guid.NewGuid(),
            Motivo = "Prueba"
        });
        mientrasBloqueado.Autorizado.Should().BeFalse();
        mientrasBloqueado.Bloqueado.Should().BeTrue();

        // Simula que ya pasaron los 5 minutos de bloqueo.
        supervisor.PinBloqueadoHasta = DateTime.UtcNow.AddMinutes(-1);
        await context.SaveChangesAsync();

        var trasDesbloqueo = await sut.AutorizarAsync(new AutorizarSupervisorPinRequestDTO
        {
            Pin = "1234",
            AccionProtegida = "CancelarPlatilloCocina",
            IdPedido = pedido.Id,
            IdPedidoDetalle = Guid.NewGuid(),
            Motivo = "Prueba"
        });

        trasDesbloqueo.Autorizado.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarTokenAutorizacion_ConAccionOIdPedidoDetalleDistinto_RetornaFalse()
    {
        using var db = CrearContexto();
        var (context, _, _, pedido) = await PrepararEscenarioAsync(db, pinConfigurado: "1234");
        var sut = new SupervisorPinSecurityService(context, new Pbkdf2PasswordHasher(), CrearConfiguracionJwt());

        var idPedidoDetalle = Guid.NewGuid();
        var resultado = await sut.AutorizarAsync(new AutorizarSupervisorPinRequestDTO
        {
            Pin = "1234",
            AccionProtegida = "CancelarPlatilloCocina",
            IdPedido = pedido.Id,
            IdPedidoDetalle = idPedidoDetalle,
            Motivo = "Prueba"
        });

        sut.ValidarTokenAutorizacion(resultado.TokenAutorizacion, "DescuentoExcesivo", idPedidoDetalle, out _)
            .Should().BeFalse();

        sut.ValidarTokenAutorizacion(resultado.TokenAutorizacion, "CancelarPlatilloCocina", Guid.NewGuid(), out _)
            .Should().BeFalse();
    }
}

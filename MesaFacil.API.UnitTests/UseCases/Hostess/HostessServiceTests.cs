using Domain.Entities;
using DTO.Hostess;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Hostess;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Hostess;

public class HostessServiceTests
{
    private ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MesaFacil_Hostess_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var interceptor = new AuditableEntitySaveChangesInterceptor();
        var outboxInterceptor = new OutboxSaveChangesInterceptor();
        return new ApplicationDbContext(options, interceptor, outboxInterceptor);
    }

    [Fact]
    public async Task RegistrarEnWaitlist_CalculaMinutosEstimados_YGeneraEnlaceWhatsApp()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HostessService(context);

        var sucursal = new Sucursal { Id = 1, IdEmpresa = 1, Nombre = "Sucursal Roma", IsActive = true };
        var area = new Area { Id = 1, Nombre = "Terraza", IdSucursal = 1, IsActive = true };
        var mesa = new Mesa { Id = 10, Codigo = "T1", IdSucursal = 1, IdArea = 1, Asientos = 4, IdEstadoMesa = EstadosMesaConst.Disponible, IsActive = true };

        context.Sucursales.Add(sucursal);
        context.Areas.Add(area);
        context.Mesas.Add(mesa);
        await context.SaveChangesAsync();

        var dto = new RegistrarWaitlistDTO
        {
            IdSucursal = 1,
            NombreCliente = "Sofía Ruiz",
            TelefonoCliente = "5512345678",
            NumeroPersonas = 4,
            ZonaPreferencia = "Terraza"
        };

        // Act
        var result = await service.RegistrarEnWaitlistAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.NombreCliente.Should().Be("Sofía Ruiz");
        result.TelefonoCliente.Should().Be("5512345678");
        result.NumeroPersonas.Should().Be(4);
        result.MinutosEstimados.Should().Be(0); // Mesa disponible de inmediato
        result.Estado.Should().Be("EnEspera");
        result.EnlaceWhatsApp.Should().NotBeNullOrEmpty();
        result.EnlaceWhatsApp.Should().Contain("wa.me/525512345678");
        result.EnlaceWhatsApp.Should().Contain("Sof%C3%ADa%20Ruiz");
    }

    [Fact]
    public async Task NotificarWaitlist_ActualizaEstadoANotificado_ConTimestamp()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HostessService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Condesa", IsActive = true };
        context.Sucursales.Add(sucursal);

        var item = new FilaEsperaItem
        {
            Id = 1,
            IdEmpresa = 1,
            IdSucursal = 1,
            NombreCliente = "Carlos Slim",
            TelefonoCliente = "5598765432",
            NumeroPersonas = 2,
            Estado = "EnEspera",
            RegistradoEn = DateTime.UtcNow.AddMinutes(-20),
            MinutosEstimados = 20,
            IsActive = true
        };
        context.FilaEsperaItems.Add(item);
        await context.SaveChangesAsync();

        // Act
        var notificado = await service.NotificarWaitlistAsync(1);

        // Assert
        notificado.Estado.Should().Be("Notificado");
        notificado.NotificadoEn.Should().NotBeNull();
        notificado.MinutosTranscurridos.Should().BeGreaterThanOrEqualTo(20);

        var dbItem = await context.FilaEsperaItems.FindAsync(1);
        dbItem!.Estado.Should().Be("Notificado");
        dbItem.NotificadoEn.Should().NotBeNull();
    }

    [Fact]
    public async Task SentarWaitlist_ActualizaMesaAOcupada_YComensalASentado()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HostessService(context);

        var mesa = new Mesa { Id = 5, Codigo = "M5", IdSucursal = 1, Asientos = 4, IdEstadoMesa = EstadosMesaConst.Disponible, IsActive = true };
        var item = new FilaEsperaItem
        {
            Id = 10,
            IdEmpresa = 1,
            IdSucursal = 1,
            NombreCliente = "Ana Torres",
            TelefonoCliente = "5544332211",
            NumeroPersonas = 3,
            Estado = "EnEspera",
            IsActive = true
        };

        context.Mesas.Add(mesa);
        context.FilaEsperaItems.Add(item);
        await context.SaveChangesAsync();

        // Act
        var ok = await service.SentarWaitlistAsync(10, new SentarWaitlistDTO { IdMesa = 5 });

        // Assert
        ok.Should().BeTrue();

        var dbMesa = await context.Mesas.FindAsync(5);
        dbMesa!.IdEstadoMesa.Should().Be(EstadosMesaConst.Ocupada);

        var dbItem = await context.FilaEsperaItems.FindAsync(10);
        dbItem!.Estado.Should().Be("Sentado");
        dbItem.IdMesaAsignada.Should().Be(5);
    }

    [Fact]
    public async Task Reservaciones_CrearYConfirmarLlegada_AsignaMesaAOcupada()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HostessService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Polanco", IsActive = true };
        var mesa = new Mesa { Id = 20, Codigo = "P20", IdSucursal = 1, Asientos = 6, IdEstadoMesa = EstadosMesaConst.Disponible, IsActive = true };
        context.Sucursales.Add(sucursal);
        context.Mesas.Add(mesa);
        await context.SaveChangesAsync();

        var dtoCrear = new CrearReservaDTO
        {
            IdSucursal = 1,
            IdMesa = 20,
            NombreCliente = "Familia González",
            TelefonoCliente = "5566778899",
            FechaHoraReserva = DateTime.UtcNow.AddHours(2),
            NumeroPersonas = 5,
            AnticipoPagado = 500.00m,
            Notas = "Cumpleaños abuela, pastel sorpresa"
        };

        // Act 1: Crear Reserva
        var reserva = await service.CrearReservaAsync(dtoCrear);
        reserva.Should().NotBeNull();
        reserva.EstadoReserva.Should().Be("Confirmada");
        reserva.AnticipoPagado.Should().Be(500.00m);

        // Act 2: Confirmar Llegada y Sentar
        var confirm = await service.ConfirmarLlegadaReservaAsync(reserva.Id, new ConfirmarLlegadaReservaDTO { IdMesa = 20 });
        confirm.Should().BeTrue();

        // Assert
        var dbReserva = await context.ReservasMesa.FindAsync(reserva.Id);
        dbReserva!.EstadoReserva.Should().Be("Sentada");

        var dbMesa = await context.Mesas.FindAsync(20);
        dbMesa!.IdEstadoMesa.Should().Be(EstadosMesaConst.Ocupada);
    }

    [Fact]
    public async Task CrearReserva_ConCorreoYAliases_GuardaYRetornaCorrectamente()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HostessService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Condesa", IsActive = true };
        context.Sucursales.Add(sucursal);
        await context.SaveChangesAsync();

        var dtoCrear = new CrearReservaDTO
        {
            IdSucursal = 1,
            NombreCliente = "Alecs Dev",
            Telefono = "5511223344",
            Correo = "alecs@ejemplo.com",
            Comensales = 4,
            FechaHoraReserva = DateTime.UtcNow.AddHours(3),
            DepositoGarantia = 250m,
            Notas = "Mesa cerca de ventana"
        };

        // Act
        var reserva = await service.CrearReservaAsync(dtoCrear);

        // Assert
        reserva.Should().NotBeNull();
        reserva.NombreCliente.Should().Be("Alecs Dev");
        reserva.Telefono.Should().Be("5511223344");
        reserva.TelefonoCliente.Should().Be("5511223344");
        reserva.Correo.Should().Be("alecs@ejemplo.com");
        reserva.Comensales.Should().Be(4);
        reserva.NumeroPersonas.Should().Be(4);
        reserva.AnticipoPagado.Should().Be(250m);
        reserva.DepositoGarantia.Should().Be(250m);
        reserva.Notas.Should().Contain("Mesa cerca de ventana");
    }

    [Fact]
    public async Task DashboardSummary_RetornaMetricasConsolidadasCorrectas()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new HostessService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Matriz", IsActive = true };
        context.Sucursales.Add(sucursal);

        context.Mesas.AddRange(
            new Mesa { Id = 1, Codigo = "M1", IdSucursal = 1, Asientos = 2, IdEstadoMesa = EstadosMesaConst.Disponible, IsActive = true },
            new Mesa { Id = 2, Codigo = "M2", IdSucursal = 1, Asientos = 4, IdEstadoMesa = EstadosMesaConst.Ocupada, IsActive = true },
            new Mesa { Id = 3, Codigo = "M3", IdSucursal = 1, Asientos = 4, IdEstadoMesa = EstadosMesaConst.PidiendoCuenta, IsActive = true }
        );

        context.FilaEsperaItems.AddRange(
            new FilaEsperaItem { Id = 1, IdEmpresa = 1, IdSucursal = 1, NombreCliente = "Cliente 1", TelefonoCliente = "123", MinutosEstimados = 15, Estado = "EnEspera", IsActive = true },
            new FilaEsperaItem { Id = 2, IdEmpresa = 1, IdSucursal = 1, NombreCliente = "Cliente 2", TelefonoCliente = "456", MinutosEstimados = 25, Estado = "Notificado", IsActive = true }
        );

        context.ReservasMesa.Add(
            new ReservaMesa { Id = 1, IdEmpresa = 1, IdSucursal = 1, NombreCliente = "Reserva 1", TelefonoCliente = "789", FechaHoraReserva = DateTime.UtcNow.Date.AddHours(12), EstadoReserva = "Confirmada", IsActive = true }
        );

        await context.SaveChangesAsync();

        // Act
        var summary = await service.GetSummaryAsync(1);

        // Assert
        summary.Should().NotBeNull();
        summary.TotalEnEspera.Should().Be(1);
        summary.TotalNotificados.Should().Be(1);
        summary.MesasDisponibles.Should().Be(1);
        summary.MesasPidiendoCuenta.Should().Be(1);
        summary.ReservasHoy.Should().Be(1);
    }
}

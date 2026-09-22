using Domain.Entities;
using DTO.Suscripcion;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Suscripciones;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Suscripciones;

public class TenantStatusServiceTests
{
    private static ApplicationDbContext CrearContextoEnMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MesaFacil_TenantStatus_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    private static IConfiguration CrearConfiguracion(bool enabled = true)
    {
        var dict = new Dictionary<string, string?>
        {
            ["FeatureGating:Enabled"] = enabled.ToString(),
            ["OrionSys:SoporteWhatsApp"] = "+52 33 1111 2222",
            ["OrionSys:InternalApiKey"] = "test_orion_key"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    [Fact]
    public async Task ObtenerEstadoTenantAsync_EmpresaActiva_RetornaNoSuspendido()
    {
        using var context = CrearContextoEnMemoria();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var config = CrearConfiguracion();
        var service = new TenantStatusService(context, cache, config, NullLogger<TenantStatusService>.Instance);

        var empresa = new Empresa { Id = 10, Nombre = "Taquería El Pastor", Rfc = "PAS123456789" };
        context.Empresas.Add(empresa);

        var suscripcion = new EmpresaSuscripcion
        {
            IdEmpresa = 10,
            IdPlan = 1,
            EstadoSuscripcion = EstadoSuscripcionValores.Activa,
            FechaInicio = DateTime.UtcNow.AddMonths(-1),
            FechaFinVigencia = DateTime.UtcNow.AddMonths(1),
            EnPeriodoGracia = false
        };
        context.EmpresasSuscripcion.Add(suscripcion);
        await context.SaveChangesAsync();

        var estado = await service.ObtenerEstadoTenantAsync(10);

        estado.Should().NotBeNull();
        estado.EstaSuspendido.Should().BeFalse();
        estado.EstadoSuscripcion.Should().Be(EstadoSuscripcionValores.Activa);
    }

    [Fact]
    public async Task ObtenerEstadoTenantAsync_EmpresaSuspendida_RetornaSuspendidoConMotivoYContacto()
    {
        using var context = CrearContextoEnMemoria();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var config = CrearConfiguracion();
        var service = new TenantStatusService(context, cache, config, NullLogger<TenantStatusService>.Instance);

        var empresa = new Empresa { Id = 20, Nombre = "Café Central", Rfc = "CAF123456789" };
        context.Empresas.Add(empresa);

        var suscripcion = new EmpresaSuscripcion
        {
            IdEmpresa = 20,
            IdPlan = 1,
            EstadoSuscripcion = EstadoSuscripcionValores.Suspendida,
            MotivoSuspension = "Falta de pago periodo Agosto",
            ContactoWhatsApp = "+52 33 9999 8888",
            FechaInicio = DateTime.UtcNow.AddMonths(-2),
            FechaFinVigencia = DateTime.UtcNow.AddDays(-5),
            EnPeriodoGracia = false
        };
        context.EmpresasSuscripcion.Add(suscripcion);
        await context.SaveChangesAsync();

        var estado = await service.ObtenerEstadoTenantAsync(20);

        estado.Should().NotBeNull();
        estado.EstaSuspendido.Should().BeTrue();
        estado.EstadoSuscripcion.Should().Be(EstadoSuscripcionValores.Suspendida);
        estado.MotivoSuspension.Should().Be("Falta de pago periodo Agosto");
        estado.ContactoWhatsApp.Should().Be("+52 33 9999 8888");
    }

    [Fact]
    public async Task ActualizarEstadoDesdeHubAsync_InvalidaCacheYActualizaDB()
    {
        using var context = CrearContextoEnMemoria();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var config = CrearConfiguracion();
        var service = new TenantStatusService(context, cache, config, NullLogger<TenantStatusService>.Instance);

        var empresa = new Empresa { Id = 30, Nombre = "Pizzería Bella", Rfc = "PIZ123456789" };
        context.Empresas.Add(empresa);
        var suscripcion = new EmpresaSuscripcion
        {
            IdEmpresa = 30,
            IdPlan = 1,
            EstadoSuscripcion = EstadoSuscripcionValores.Activa,
            FechaInicio = DateTime.UtcNow.AddMonths(-1),
            FechaFinVigencia = DateTime.UtcNow.AddMonths(1)
        };
        context.EmpresasSuscripcion.Add(suscripcion);
        await context.SaveChangesAsync();

        // 1. Consultar para llenar la caché
        var inicial = await service.ObtenerEstadoTenantAsync(30);
        inicial.EstaSuspendido.Should().BeFalse();

        // 2. Hub emite Kill-Switch (suspensión)
        var req = new ActualizarEstadoLicenciaRequestDTO
        {
            EmpresaId = 30,
            NuevoEstado = EstadoSuscripcionValores.Suspendida,
            Motivo = "Corte administrativo",
            ContactoWhatsApp = "+52 33 5555 4444"
        };
        var res = await service.ActualizarEstadoDesdeHubAsync(req);
        res.Should().BeTrue();

        // 3. Consultar nuevamente: debe leer el nuevo estado invalidado sin esperar a los 5 minutos de TTL
        var actualizado = await service.ObtenerEstadoTenantAsync(30);
        actualizado.EstaSuspendido.Should().BeTrue();
        actualizado.MotivoSuspension.Should().Be("Corte administrativo");
        actualizado.ContactoWhatsApp.Should().Be("+52 33 5555 4444");
    }
}

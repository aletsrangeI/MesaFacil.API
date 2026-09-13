using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Suscripciones;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Suscripciones;

/// <summary>
/// Spec 021: valida el pilar técnico #1 del feature (Modo Demo por defecto) y la lógica de
/// evaluación de capacidades por tier + periodo de gracia cuando el gating sí está activo.
/// </summary>
public class FeatureGateServiceTests
{
    private static ApplicationDbContext CrearContextoEnMemoria()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MesaFacil_FeatureGating_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    private static IConfiguration CrearConfiguracion(bool? enabled)
    {
        var dict = new Dictionary<string, string?>();
        if (enabled.HasValue)
        {
            dict["FeatureGating:Enabled"] = enabled.Value.ToString();
        }

        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    private static async Task<(Empresa empresa, CatPlanSuscripcion planTier1, CatPlanSuscripcion planTier2, CatPlanSuscripcion planTier3)> SembrarCatalogoAsync(ApplicationDbContext context)
    {
        var empresa = new Empresa { Nombre = "Restaurante de Prueba", Rfc = "RPR010101AA1" };
        context.Empresas.Add(empresa);

        var tier1 = new CatPlanSuscripcion
        {
            Codigo = CodigosPlanSuscripcion.Tier1_Barra,
            Nombre = "Barra & Café",
            PrecioMensualMxn = 699m,
            PrecioAnualMxn = 6710.40m,
            MaxSucursales = 1,
            MaxKdsBase = 0,
            PermiteMesas = false,
            PermiteSplitBill = false,
            PermiteRecetas = false,
            PermiteCfdiXml = false,
            PermiteCxP = false,
            IsActive = true
        };
        var tier2 = new CatPlanSuscripcion
        {
            Codigo = CodigosPlanSuscripcion.Tier2_Pro,
            Nombre = "Restaurante Pro",
            PrecioMensualMxn = 1499m,
            PrecioAnualMxn = 14390.40m,
            MaxSucursales = 1,
            MaxKdsBase = 1,
            PermiteMesas = true,
            PermiteSplitBill = true,
            PermiteRecetas = true,
            PermiteCfdiXml = false,
            PermiteCxP = false,
            IsActive = true
        };
        var tier3 = new CatPlanSuscripcion
        {
            Codigo = CodigosPlanSuscripcion.Tier3_Multi,
            Nombre = "Multi-Sucursal",
            PrecioMensualMxn = 2699m,
            PrecioAnualMxn = 25910.40m,
            MaxSucursales = 0,
            MaxKdsBase = 3,
            PermiteMesas = true,
            PermiteSplitBill = true,
            PermiteRecetas = true,
            PermiteCfdiXml = true,
            PermiteCxP = true,
            IsActive = true
        };

        context.CatPlanesSuscripcion.AddRange(tier1, tier2, tier3);
        await context.SaveChangesAsync();

        return (empresa, tier1, tier2, tier3);
    }

    [Fact]
    public async Task TieneAccesoAsync_ConGatingDeshabilitado_SiemprePermiteAccesoSinImportarElPlan()
    {
        // Arrange: ninguna EmpresaSuscripcion en BD (empresa sin plan asignado en absoluto).
        using var context = CrearContextoEnMemoria();
        var sut = new FeatureGateService(context, CrearConfiguracion(enabled: false));

        // Act & Assert: aunque la empresa no exista y no tenga suscripción, con Enabled=false
        // (el valor por defecto) siempre debe permitir el acceso, sin tocar la BD.
        (await sut.TieneAccesoAsync(empresaId: 999, FeatureNames.ModuloRecetas)).Should().BeTrue();
        (await sut.TieneAccesoAsync(empresaId: 999, FeatureNames.ModuloKds)).Should().BeTrue();
        (await sut.TieneAccesoAsync(empresaId: 999, FeatureNames.ModuloCfdiXml)).Should().BeTrue();
    }

    [Fact]
    public async Task TieneAccesoAsync_ConfiguracionSinSeccionFeatureGating_SeComportaComoDeshabilitado()
    {
        // Arrange: configuración que ni siquiera define la sección FeatureGating.
        using var context = CrearContextoEnMemoria();
        var sut = new FeatureGateService(context, CrearConfiguracion(enabled: null));

        (await sut.TieneAccesoAsync(empresaId: 1, FeatureNames.ModuloRecetas)).Should().BeTrue();
    }

    [Fact]
    public async Task TieneAccesoAsync_ConGatingHabilitado_Tier1NoTieneAccesoARecetasNiKds()
    {
        using var context = CrearContextoEnMemoria();
        var (empresa, tier1, _, _) = await SembrarCatalogoAsync(context);

        context.EmpresasSuscripcion.Add(new EmpresaSuscripcion
        {
            IdEmpresa = empresa.Id,
            IdPlan = tier1.Id,
            EsPagoAnual = false,
            FechaInicio = DateTime.UtcNow.AddDays(-10),
            FechaFinVigencia = DateTime.UtcNow.AddDays(20),
            EstadoSuscripcion = EstadoSuscripcionValores.Activa,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var sut = new FeatureGateService(context, CrearConfiguracion(enabled: true));

        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloRecetas)).Should().BeFalse();
        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloKds)).Should().BeFalse();
        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloSplitBill)).Should().BeFalse();
    }

    [Theory]
    [InlineData(CodigosPlanSuscripcion.Tier2_Pro)]
    [InlineData(CodigosPlanSuscripcion.Tier3_Multi)]
    public async Task TieneAccesoAsync_ConGatingHabilitado_Tier2YTier3TienenAccesoARecetasYKds(string codigoPlan)
    {
        using var context = CrearContextoEnMemoria();
        var (empresa, _, tier2, tier3) = await SembrarCatalogoAsync(context);
        var plan = codigoPlan == CodigosPlanSuscripcion.Tier2_Pro ? tier2 : tier3;

        context.EmpresasSuscripcion.Add(new EmpresaSuscripcion
        {
            IdEmpresa = empresa.Id,
            IdPlan = plan.Id,
            EsPagoAnual = false,
            FechaInicio = DateTime.UtcNow.AddDays(-10),
            FechaFinVigencia = DateTime.UtcNow.AddDays(20),
            EstadoSuscripcion = EstadoSuscripcionValores.Activa,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var sut = new FeatureGateService(context, CrearConfiguracion(enabled: true));

        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloRecetas)).Should().BeTrue();
        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloKds)).Should().BeTrue();
    }

    [Fact]
    public async Task TieneAccesoAsync_SuscripcionVencidaHace3DiasEnPeriodoDeGracia_SiguePermitiendoAcceso()
    {
        using var context = CrearContextoEnMemoria();
        var (empresa, _, tier2, _) = await SembrarCatalogoAsync(context);

        context.EmpresasSuscripcion.Add(new EmpresaSuscripcion
        {
            IdEmpresa = empresa.Id,
            IdPlan = tier2.Id,
            EsPagoAnual = false,
            FechaInicio = DateTime.UtcNow.AddMonths(-2),
            FechaFinVigencia = DateTime.UtcNow.AddDays(-3), // Vencida hace 3 días naturales
            EstadoSuscripcion = EstadoSuscripcionValores.EnGracia,
            EnPeriodoGracia = true,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var sut = new FeatureGateService(context, CrearConfiguracion(enabled: true));

        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloRecetas)).Should().BeTrue(
            "el vencimiento entra en un periodo de gracia operativo y nunca debe bloquear a mitad de servicio");
    }

    [Fact]
    public async Task TieneAccesoAsync_SuscripcionVencidaHace10DiasSinPeriodoDeGraciaActivo_NiegaElAcceso()
    {
        using var context = CrearContextoEnMemoria();
        var (empresa, _, tier2, _) = await SembrarCatalogoAsync(context);

        context.EmpresasSuscripcion.Add(new EmpresaSuscripcion
        {
            IdEmpresa = empresa.Id,
            IdPlan = tier2.Id,
            EsPagoAnual = false,
            FechaInicio = DateTime.UtcNow.AddMonths(-3),
            FechaFinVigencia = DateTime.UtcNow.AddDays(-10), // Vencida hace 10 días naturales
            EstadoSuscripcion = EstadoSuscripcionValores.Suspendida,
            EnPeriodoGracia = false,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var sut = new FeatureGateService(context, CrearConfiguracion(enabled: true));

        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloRecetas)).Should().BeFalse();
    }

    [Fact]
    public async Task TieneAccesoAsync_ConGatingHabilitadoYSinSuscripcionAsignada_NiegaElAcceso()
    {
        using var context = CrearContextoEnMemoria();
        var (empresa, _, _, _) = await SembrarCatalogoAsync(context);

        var sut = new FeatureGateService(context, CrearConfiguracion(enabled: true));

        (await sut.TieneAccesoAsync(empresa.Id, FeatureNames.ModuloRecetas)).Should().BeFalse();
    }
}

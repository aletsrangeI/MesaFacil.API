using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Auditoria;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Auditoria;

/// <summary>
/// Spec 024, sección 2.4: cálculo exacto de
/// %Cancelaciones = TotalCancelado / (VentaNeta + TotalCancelado) * 100,
/// y el semáforo: Verde (&lt; 1.0%), Ámbar (1.0% - 2.0%), Rojo (&gt; 2.0%, superaUmbralAlerta).
/// </summary>
public class CancelacionRatioTests
{
    private static ApplicationDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MesaFacil_CancelacionRatio_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    /// <summary>
    /// Crea un Turno abierto en una Sucursal nueva, con ventas (Pagos) y cancelaciones
    /// (EventoPedido "PlatilloCancelado") por los montos indicados, todo dentro de la ventana
    /// del turno para que el cálculo del servicio los tome en cuenta.
    /// </summary>
    private static async Task<(ApplicationDbContext db, Turno turno)> PrepararTurnoAsync(
        ApplicationDbContext db, decimal ventaNeta, decimal totalCancelado, int cantidadEventosCancelacion = 1)
    {
        var empresa = new Empresa { Nombre = "Restaurante de Prueba", Rfc = "RPR010101AA1", IsActive = true };
        db.Empresas.Add(empresa);
        await db.SaveChangesAsync();

        var sucursal = new Sucursal { IdEmpresa = empresa.Id, Nombre = "Sucursal Centro", IsActive = true };
        db.Sucursales.Add(sucursal);
        await db.SaveChangesAsync();

        var apertura = DateTime.UtcNow.AddHours(-6);
        var turno = new Turno { IdUsuario = 1, IdSucursal = sucursal.Id, Apertura = apertura, CajaInicial = 0m };
        db.Turnos.Add(turno);
        await db.SaveChangesAsync();

        var mesa = new Mesa { IdSucursal = sucursal.Id, Codigo = "Mesa 4", IdEstadoMesa = 1, IsActive = true };
        db.Mesas.Add(mesa);
        await db.SaveChangesAsync();

        var pedido = new Pedido
        {
            IdEmpresa = empresa.Id,
            IdSucursal = sucursal.Id,
            IdMesa = mesa.Id,
            IdTipoPedido = 1,
            IdEstadoPedido = 1,
            AbiertoEn = apertura.AddMinutes(5)
        };
        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync();

        if (ventaNeta > 0)
        {
            var cuenta = new Cuenta { IdPedido = pedido.Id, Subtotal = ventaNeta, Total = ventaNeta, IdEstadoCuenta = 1 };
            db.Cuentas.Add(cuenta);
            await db.SaveChangesAsync();

            db.Pagos.Add(new Pago
            {
                IdCuenta = cuenta.Id,
                Monto = ventaNeta,
                PagadoEn = apertura.AddHours(1),
                IdMetodoDePago = 1,
                IsActive = true
            });
            await db.SaveChangesAsync();
        }

        if (totalCancelado > 0)
        {
            var montoPorEvento = Math.Round(totalCancelado / cantidadEventosCancelacion, 2);
            for (int i = 0; i < cantidadEventosCancelacion; i++)
            {
                // El último evento absorbe el residuo de redondeo para que la suma cuadre exacto.
                var monto = i == cantidadEventosCancelacion - 1
                    ? totalCancelado - montoPorEvento * (cantidadEventosCancelacion - 1)
                    : montoPorEvento;

                db.EventosPedido.Add(new EventoPedido
                {
                    IdPedido = pedido.Id,
                    IdUsuario = 10,
                    IdUsuarioSupervisor = 3,
                    TipoEvento = "PlatilloCancelado",
                    MontoCancelado = monto,
                    Payload = "{\"ProductoNombre\":\"Corte Rib Eye 400g\",\"Motivo\":\"Error de captura del mesero\"}",
                    IsActive = true,
                    CreatedAt = apertura.AddHours(2).AddMinutes(i)
                });
            }
            await db.SaveChangesAsync();
        }

        return (db, turno);
    }

    [Fact]
    public async Task ObtenerResumenTurnoAsync_ReplicaElEjemploExactoDelSpec_2_74PorCientoYAlertaRoja()
    {
        // Spec 024, sección 3 (GET /api/auditoria/cancelaciones-turno): totalVentasTurno=18450.00,
        // totalCancelacionesTurno=520.00 => porcentajeCancelaciones=2.74, superaUmbralAlerta=true.
        using var db = CrearContexto();
        var (context, turno) = await PrepararTurnoAsync(db, ventaNeta: 18450.00m, totalCancelado: 520.00m);
        var sut = new AuditoriaCancelacionesService(context);

        var response = await sut.ObtenerResumenTurnoAsync(turno.Id);

        response.isSuccess.Should().BeTrue();
        response.Data!.TotalVentasTurno.Should().Be(18450.00m);
        response.Data.TotalCancelacionesTurno.Should().Be(520.00m);
        response.Data.PorcentajeCancelaciones.Should().Be(2.74m);
        response.Data.SuperaUmbralAlerta.Should().BeTrue();
        response.Data.TotalEventos.Should().Be(1);
        response.Data.Desglose.Should().ContainSingle();
        response.Data.Desglose[0].Mesa.Should().Be("Mesa 4");
        response.Data.Desglose[0].Platillo.Should().Be("Corte Rib Eye 400g");
        response.Data.Desglose[0].Motivo.Should().Be("Error de captura del mesero");
    }

    [Fact]
    public async Task ObtenerResumenTurnoAsync_MenosDeUnoPorCiento_SemaforoVerdeSinAlerta()
    {
        // 50 / (10000 + 50) * 100 = 0.4975% -> 0.50%, redondeado. Verde: < 1.0%.
        using var db = CrearContexto();
        var (context, turno) = await PrepararTurnoAsync(db, ventaNeta: 10000m, totalCancelado: 50m);
        var sut = new AuditoriaCancelacionesService(context);

        var response = await sut.ObtenerResumenTurnoAsync(turno.Id);

        response.Data!.PorcentajeCancelaciones.Should().Be(0.50m);
        response.Data.PorcentajeCancelaciones.Should().BeLessThan(1.0m);
        response.Data.SuperaUmbralAlerta.Should().BeFalse();
    }

    [Fact]
    public async Task ObtenerResumenTurnoAsync_EntreUnoYDosPorCiento_SemaforoAmbarSinAlerta()
    {
        // 150 / (9850 + 150) * 100 = 1.5%. Ámbar: 1.0% - 2.0%, no dispara la alerta roja.
        using var db = CrearContexto();
        var (context, turno) = await PrepararTurnoAsync(db, ventaNeta: 9850m, totalCancelado: 150m, cantidadEventosCancelacion: 3);
        var sut = new AuditoriaCancelacionesService(context);

        var response = await sut.ObtenerResumenTurnoAsync(turno.Id);

        response.Data!.PorcentajeCancelaciones.Should().Be(1.5m);
        response.Data.PorcentajeCancelaciones.Should().BeInRange(1.0m, 2.0m);
        response.Data.SuperaUmbralAlerta.Should().BeFalse();
        response.Data.TotalEventos.Should().Be(3);
    }

    [Fact]
    public async Task ObtenerResumenTurnoAsync_MasDeDosPorCiento_SemaforoRojoConAlerta()
    {
        // 300 / (7000 + 300) * 100 = 4.1095... -> 4.11%. Rojo: > 2.0%, superaUmbralAlerta = true.
        using var db = CrearContexto();
        var (context, turno) = await PrepararTurnoAsync(db, ventaNeta: 7000m, totalCancelado: 300m);
        var sut = new AuditoriaCancelacionesService(context);

        var response = await sut.ObtenerResumenTurnoAsync(turno.Id);

        response.Data!.PorcentajeCancelaciones.Should().Be(4.11m);
        response.Data.PorcentajeCancelaciones.Should().BeGreaterThan(2.0m);
        response.Data.SuperaUmbralAlerta.Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerResumenTurnoAsync_SinVentasNiCancelaciones_RetornaCeroSinAlerta()
    {
        using var db = CrearContexto();
        var (context, turno) = await PrepararTurnoAsync(db, ventaNeta: 0m, totalCancelado: 0m);
        var sut = new AuditoriaCancelacionesService(context);

        var response = await sut.ObtenerResumenTurnoAsync(turno.Id);

        response.Data!.PorcentajeCancelaciones.Should().Be(0m);
        response.Data.SuperaUmbralAlerta.Should().BeFalse();
        response.Data.TotalEventos.Should().Be(0);
    }

    [Fact]
    public async Task ObtenerMotivosAsync_RetornaLosCuatroMotivosSembrados()
    {
        // Nota: AuditableEntitySaveChangesInterceptor fuerza IsActive=true en toda entidad Added
        // (comportamiento existente y compartido por todo el proyecto), por lo que este test
        // siembra únicamente los 4 motivos del spec en vez de intentar probar el filtro de
        // inactivos aquí (ya cubierto genéricamente en otros catálogos Cat*).
        using var db = CrearContexto();
        db.CatMotivosCancelacion.AddRange(
            new CatMotivoCancelacion { Descripcion = "Error de captura del mesero" },
            new CatMotivoCancelacion { Descripcion = "Platillo devuelto por el comensal" },
            new CatMotivoCancelacion { Descripcion = "Mesa se retiró sin consumir" },
            new CatMotivoCancelacion { Descripcion = "Cortesía de la casa autorizada" }
        );
        await db.SaveChangesAsync();

        var sut = new AuditoriaCancelacionesService(db);
        var response = await sut.ObtenerMotivosAsync();

        response.isSuccess.Should().BeTrue();
        response.Data.Should().HaveCount(4);
        response.Data!.Select(m => m.Descripcion).Should().Contain("Cortesía de la casa autorizada");
    }
}

using Domain.Entities;
using DTO.CxP;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.CxP;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.CxP;

public class CxPServiceTests
{
    private ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MesaFacil_CxP_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var interceptor = new AuditableEntitySaveChangesInterceptor();
        var outboxInterceptor = new OutboxSaveChangesInterceptor();
        var context = new ApplicationDbContext(options, interceptor, outboxInterceptor);
        return context;
    }

    [Fact]
    public async Task ConsistenciaSaldoInsoluto_AbonoParcialYTotal_ActualizaEstadosCorrectamente()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new CxPService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Sucursal Matriz", IsActive = true };
        var proveedor = new Proveedor { Id = 1, RFC = "CAR951128AA1", RazonSocial = "Carnes del Norte SA de CV", DiasCredito = 15, IsActive = true };
        var metodoEfectivo = new CatMetodoDePago { Id = 1, Descripcion = "Efectivo", IsActive = true };

        context.Sucursales.Add(sucursal);
        context.Proveedores.Add(proveedor);
        context.CatMetodosDePago.Add(metodoEfectivo);

        var cuenta = new CuentaPorPagar
        {
            Id = 1,
            IdEmpresa = 1,
            IdSucursal = 1,
            IdProveedor = 1,
            MontoTotal = 10000.00m,
            SaldoInsoluto = 10000.00m,
            FechaEmision = DateTime.UtcNow,
            FechaVencimiento = DateTime.UtcNow.AddDays(15),
            Estado = "Pendiente",
            IsActive = true
        };
        context.CuentasPorPagar.Add(cuenta);
        await context.SaveChangesAsync();

        // Act 1: Registrar abono parcial de $4,000.00
        var dto1 = new RegistrarPagoCxPDTO
        {
            IdCuentaPorPagar = 1,
            Monto = 4000.00m,
            IdMetodoPago = 1,
            PagarDesdeCajaChica = false
        };

        var res1 = await service.RegistrarAbonoAsync(dto1, idUsuario: 1);

        // Assert 1
        res1.isSuccess.Should().BeTrue();
        var ctaDb1 = await context.CuentasPorPagar.FindAsync(1);
        ctaDb1.Should().NotBeNull();
        ctaDb1!.SaldoInsoluto.Should().Be(6000.00m);
        ctaDb1.Estado.Should().Be("Abonada");

        // Act 2: Registrar abono de $6,000.00 (liquidación completa)
        var dto2 = new RegistrarPagoCxPDTO
        {
            IdCuentaPorPagar = 1,
            Monto = 6000.00m,
            IdMetodoPago = 1,
            PagarDesdeCajaChica = false
        };

        var res2 = await service.RegistrarAbonoAsync(dto2, idUsuario: 1);

        // Assert 2
        res2.isSuccess.Should().BeTrue();
        var ctaDb2 = await context.CuentasPorPagar.FindAsync(1);
        ctaDb2!.SaldoInsoluto.Should().Be(0.00m);
        ctaDb2.Estado.Should().Be("Pagada");

        // Act 3: Intentar tercer abono de $100.00 sobre cuenta liquidada
        var dto3 = new RegistrarPagoCxPDTO
        {
            IdCuentaPorPagar = 1,
            Monto = 100.00m,
            IdMetodoPago = 1
        };

        var res3 = await service.RegistrarAbonoAsync(dto3, idUsuario: 1);

        // Assert 3: Rechazo por sobrepago
        res3.isSuccess.Should().BeFalse();
        res3.Message.Should().Contain("liquidada");
    }

    [Fact]
    public async Task RegistrarAbono_MontoMayorASaldoInsoluto_RechazaOperacion()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new CxPService(context);

        var proveedor = new Proveedor { Id = 1, RFC = "PROV010101AA1", RazonSocial = "Proveedor Prueba", IsActive = true };
        context.Proveedores.Add(proveedor);

        var cuenta = new CuentaPorPagar
        {
            IdEmpresa = 1,
            IdSucursal = 1,
            IdProveedor = 1,
            MontoTotal = 5000.00m,
            SaldoInsoluto = 2500.00m,
            FechaEmision = DateTime.UtcNow,
            FechaVencimiento = DateTime.UtcNow.AddDays(10),
            Estado = "Abonada",
            IsActive = true
        };
        context.CuentasPorPagar.Add(cuenta);
        await context.SaveChangesAsync();

        // Act: Intentar abonar $3,000.00 cuando el saldo es $2,500.00
        var dto = new RegistrarPagoCxPDTO
        {
            IdCuentaPorPagar = cuenta.Id,
            Monto = 3000.00m,
            IdMetodoPago = 1
        };

        var res = await service.RegistrarAbonoAsync(dto, idUsuario: 1);

        // Assert
        res.isSuccess.Should().BeFalse();
        res.Message.Should().Contain("excede el saldo insoluto");
        cuenta.SaldoInsoluto.Should().Be(2500.00m);
    }

    [Fact]
    public async Task RegistrarAbono_DesdeCajaChica_CreaMovimientoCajaTipoEgresoEnTurnoActivo()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new CxPService(context);

        var sucursal = new Sucursal { Id = 2, Nombre = "Sucursal Norte", IsActive = true };
        var proveedor = new Proveedor { Id = 2, RFC = "VER010101XYZ", RazonSocial = "Verduras del Campo SPR de RL", IsActive = true };
        var metodoEfectivo = new CatMetodoDePago { Id = 1, Descripcion = "Efectivo", IsActive = true };
        var turno = new Turno
        {
            Id = 5,
            IdSucursal = 2,
            IdUsuario = 10,
            Apertura = DateTime.UtcNow.AddHours(-3),
            Cierre = null, // Turno ACTIVO
            CajaInicial = 1000.00m,
            IsActive = true
        };

        context.Sucursales.Add(sucursal);
        context.Proveedores.Add(proveedor);
        context.CatMetodosDePago.Add(metodoEfectivo);
        context.Turnos.Add(turno);

        var compra = new CompraFactura
        {
            Id = 50,
            Folio = "F-9988",
            Serie = "A",
            FechaEmision = DateTime.UtcNow,
            Total = 1500.00m,
            IsActive = true
        };
        context.ComprasFactura.Add(compra);

        var cuenta = new CuentaPorPagar
        {
            Id = 20,
            IdEmpresa = 1,
            IdSucursal = 2,
            IdProveedor = 2,
            IdCompraFactura = 50,
            MontoTotal = 1500.00m,
            SaldoInsoluto = 1500.00m,
            FechaEmision = DateTime.UtcNow,
            FechaVencimiento = DateTime.UtcNow.AddDays(7),
            Estado = "Pendiente",
            IsActive = true
        };
        context.CuentasPorPagar.Add(cuenta);
        await context.SaveChangesAsync();

        // Act: Abonar $500.00 desde caja chica
        var dto = new RegistrarPagoCxPDTO
        {
            IdCuentaPorPagar = 20,
            Monto = 500.00m,
            IdMetodoPago = 1,
            PagarDesdeCajaChica = true
        };

        var res = await service.RegistrarAbonoAsync(dto, idUsuario: 10);

        // Assert
        res.isSuccess.Should().BeTrue();
        res.Data.Should().NotBeNull();
        res.Data!.IdMovimientoCaja.Should().NotBeNull();

        // Verificar que el MovimientoCaja existe en base de datos
        var mov = await context.MovimientosCaja.FindAsync(res.Data.IdMovimientoCaja!.Value);
        mov.Should().NotBeNull();
        mov!.IdTurno.Should().Be(5);
        mov.Tipo.Should().Be("Egreso");
        mov.Monto.Should().Be(500.00m);
        mov.Nota.Should().Contain("Pago Proveedor: Verduras del Campo SPR de RL");
        mov.Nota.Should().Contain("AF-9988");
    }

    [Fact]
    public async Task ReporteAntiguedadSaldos_ClasificaCuentasEnLos5BucketsTemporales()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new CxPService(context);

        var hoy = DateTime.UtcNow.Date;
        var prov = new Proveedor { Id = 3, RFC = "DIS101010ABC", RazonSocial = "Distribuidora Mayorista SA", IsActive = true };
        context.Proveedores.Add(prov);

        // 1. Al corriente (vence en 10 días) => $1,000.00
        context.CuentasPorPagar.Add(new CuentaPorPagar
        {
            Id = 101,
            IdEmpresa = 1,
            IdSucursal = 1,
            IdProveedor = 3,
            MontoTotal = 1000m,
            SaldoInsoluto = 1000m,
            FechaEmision = hoy.AddDays(-5),
            FechaVencimiento = hoy.AddDays(10),
            Estado = "Pendiente",
            IsActive = true
        });

        // 2. 1-15 días vencido (venció hace 5 días) => $2,000.00
        context.CuentasPorPagar.Add(new CuentaPorPagar
        {
            Id = 102,
            IdEmpresa = 1,
            IdSucursal = 1,
            IdProveedor = 3,
            MontoTotal = 2000m,
            SaldoInsoluto = 2000m,
            FechaEmision = hoy.AddDays(-20),
            FechaVencimiento = hoy.AddDays(-5),
            Estado = "Pendiente",
            IsActive = true
        });

        // 3. 16-30 días vencido (venció hace 20 días) => $3,000.00
        context.CuentasPorPagar.Add(new CuentaPorPagar
        {
            Id = 103,
            IdEmpresa = 1,
            IdSucursal = 1,
            IdProveedor = 3,
            MontoTotal = 3000m,
            SaldoInsoluto = 3000m,
            FechaEmision = hoy.AddDays(-40),
            FechaVencimiento = hoy.AddDays(-20),
            Estado = "Pendiente",
            IsActive = true
        });

        // 4. 31-60 días vencido (venció hace 45 días) => $4,000.00
        context.CuentasPorPagar.Add(new CuentaPorPagar
        {
            Id = 104,
            IdEmpresa = 1,
            IdSucursal = 1,
            IdProveedor = 3,
            MontoTotal = 4000m,
            SaldoInsoluto = 4000m,
            FechaEmision = hoy.AddDays(-70),
            FechaVencimiento = hoy.AddDays(-45),
            Estado = "Pendiente",
            IsActive = true
        });

        // 5. >60 días vencido (venció hace 90 días) => $5,000.00
        context.CuentasPorPagar.Add(new CuentaPorPagar
        {
            Id = 105,
            IdEmpresa = 1,
            IdSucursal = 1,
            IdProveedor = 3,
            MontoTotal = 5000m,
            SaldoInsoluto = 5000m,
            FechaEmision = hoy.AddDays(-120),
            FechaVencimiento = hoy.AddDays(-90),
            Estado = "Pendiente",
            IsActive = true
        });

        await context.SaveChangesAsync();

        // Act
        var res = await service.ObtenerReporteAntiguedadSaldosAsync(null);

        // Assert
        res.isSuccess.Should().BeTrue();
        res.Data.Should().NotBeNull();
        var totales = res.Data!.Totales;

        totales.AlCorriente.Should().Be(1000m);
        totales.De1A15.Should().Be(2000m);
        totales.De16A30.Should().Be(3000m);
        totales.De31A60.Should().Be(4000m);
        totales.MasDe60.Should().Be(5000m);
        totales.Total.Should().Be(15000m);
    }
}

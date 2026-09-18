using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Analitica;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Analitica;

public class MenuEngineeringServiceTests
{
    private ApplicationDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MesaFacil_Analitica_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var interceptor = new AuditableEntitySaveChangesInterceptor();
        var outboxInterceptor = new OutboxSaveChangesInterceptor();
        return new ApplicationDbContext(options, interceptor, outboxInterceptor);
    }

    [Fact]
    public async Task ClasificacionCuadrantes_DatasetControl_AsignaExactamenteEstrellaCaballoPuzzlePerro()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new MenuEngineeringService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Matriz", IsActive = true };
        var categoria = new CategoriaMenu { Id = 1, Nombre = "Platillos Fuertes", IsActive = true };
        context.Sucursales.Add(sucursal);
        context.CategoriaMenus.Add(categoria);

        // 4 Productos
        var pEstrella = new Producto { Id = 1, Nombre = "Ribeye Prime", IdCategoria = 1, Activo = true };
        var pCaballo = new Producto { Id = 2, Nombre = "Hamburguesa Clásica", IdCategoria = 1, Activo = true };
        var pPuzzle = new Producto { Id = 3, Nombre = "Carpaccio Trufado", IdCategoria = 1, Activo = true };
        var pPerro = new Producto { Id = 4, Nombre = "Sopa Fría", IdCategoria = 1, Activo = true };
        context.Productos.AddRange(pEstrella, pCaballo, pPuzzle, pPerro);

        // Recetas con costos unitarios:
        // Ribeye: Costo $50.00
        context.Recetas.Add(new Receta { Id = 1, IdProducto = 1, Nombre = "Receta Ribeye", CostoEstimadoUnitario = 50.00m, IsActive = true });
        // Hamburguesa: Costo $80.00
        context.Recetas.Add(new Receta { Id = 2, IdProducto = 2, Nombre = "Receta Burger", CostoEstimadoUnitario = 80.00m, IsActive = true });
        // Carpaccio: Costo $100.00
        context.Recetas.Add(new Receta { Id = 3, IdProducto = 3, Nombre = "Receta Carpaccio", CostoEstimadoUnitario = 100.00m, IsActive = true });
        // Sopa Fría: Costo $45.00
        context.Recetas.Add(new Receta { Id = 4, IdProducto = 4, Nombre = "Receta Sopa", CostoEstimadoUnitario = 45.00m, IsActive = true });

        // Pedido con ventas:
        // Ribeye: PVP $200 (Margen $150), 100 unidades (Alto Margen, Alta Popularidad -> Estrella)
        // Hamburguesa: PVP $100 (Margen $20), 100 unidades (Bajo Margen, Alta Popularidad -> Caballo)
        // Carpaccio: PVP $300 (Margen $200), 10 unidades (Alto Margen, Baja Popularidad -> Puzzle)
        // Sopa Fría: PVP $50 (Margen $5), 10 unidades (Bajo Margen, Baja Popularidad -> Perro)
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            IdSucursal = 1,
            AbiertoEn = DateTime.UtcNow.AddDays(-5),
            IdEstadoPedido = 5, // Cerrado
            IsActive = true
        };
        context.Pedidos.Add(pedido);

        context.PedidoDetalles.AddRange(
            new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedido.Id, IdProducto = 1, ProductoNombre = "Ribeye Prime", Cantidad = 100, PrecioUnitario = 200.00m, IdEstadoPedidoDetalle = 1, Cancelado = false },
            new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedido.Id, IdProducto = 2, ProductoNombre = "Hamburguesa Clásica", Cantidad = 100, PrecioUnitario = 100.00m, IdEstadoPedidoDetalle = 1, Cancelado = false },
            new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedido.Id, IdProducto = 3, ProductoNombre = "Carpaccio Trufado", Cantidad = 10, PrecioUnitario = 300.00m, IdEstadoPedidoDetalle = 1, Cancelado = false },
            new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedido.Id, IdProducto = 4, ProductoNombre = "Sopa Fría", Cantidad = 10, PrecioUnitario = 50.00m, IdEstadoPedidoDetalle = 1, Cancelado = false }
        );

        await context.SaveChangesAsync();

        // Act
        var reporte = await service.ObtenerReporteIngenieriaMenuAsync(
            idSucursal: 1,
            fechaInicio: DateTime.UtcNow.AddDays(-10),
            fechaFin: DateTime.UtcNow);

        // Assert
        reporte.Should().NotBeNull();
        reporte.TotalUnidadesVendidas.Should().Be(220);
        reporte.Items.Should().HaveCount(4);

        // Kasavana & Smith Check:
        // Total Profit = (150*100) + (20*100) + (200*10) + (5*10) = 15000 + 2000 + 2000 + 50 = 19050.
        // Margen Promedio Ponderado = 19050 / 220 = 86.59
        reporte.MargenContribucionPromedio.Should().Be(86.59m);

        // Umbral Popularidad = 0.70 * (220 / 4) = 38.5
        reporte.UmbralPopularidadUnidades.Should().Be(38.5m);

        var itemEstrella = reporte.Items.First(x => x.IdProducto == 1);
        itemEstrella.Cuadrante.Should().Be("Estrella");
        itemEstrella.MargenContribucion.Should().Be(150.00m);
        itemEstrella.UnidadesVendidas.Should().Be(100);

        var itemCaballo = reporte.Items.First(x => x.IdProducto == 2);
        itemCaballo.Cuadrante.Should().Be("CaballoBatalla");
        itemCaballo.MargenContribucion.Should().Be(20.00m);
        itemCaballo.UnidadesVendidas.Should().Be(100);

        var itemPuzzle = reporte.Items.First(x => x.IdProducto == 3);
        itemPuzzle.Cuadrante.Should().Be("Puzzle");
        itemPuzzle.MargenContribucion.Should().Be(200.00m);
        itemPuzzle.UnidadesVendidas.Should().Be(10);

        var itemPerro = reporte.Items.First(x => x.IdProducto == 4);
        itemPerro.Cuadrante.Should().Be("Perro");
        itemPerro.MargenContribucion.Should().Be(5.00m);
        itemPerro.UnidadesVendidas.Should().Be(10);

        // KPIs
        reporte.Kpis.CantidadEstrellas.Should().Be(1);
        reporte.Kpis.CantidadCaballos.Should().Be(1);
        reporte.Kpis.CantidadPuzzles.Should().Be(1);
        reporte.Kpis.CantidadPerros.Should().Be(1);
        reporte.Kpis.PlatilloMasRentable.Should().Be("Carpaccio Trufado");
        reporte.Kpis.PlatilloMasVendido.Should().Be("Ribeye Prime"); // o Hamburguesa (ambos 100)
    }

    [Fact]
    public async Task ExclusionCancelaciones_DetallesYPedidosCancelados_NoSeSumanEnVentas()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new MenuEngineeringService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Matriz", IsActive = true };
        var categoria = new CategoriaMenu { Id = 1, Nombre = "Bebidas", IsActive = true };
        context.Sucursales.Add(sucursal);
        context.CategoriaMenus.Add(categoria);

        var prod = new Producto { Id = 10, Nombre = "Cerveza Artesanal", IdCategoria = 1, Activo = true };
        context.Productos.Add(prod);
        context.Recetas.Add(new Receta { Id = 10, IdProducto = 10, Nombre = "Receta Cerveza", CostoEstimadoUnitario = 20.00m, IsActive = true });

        var pedidoValido = new Pedido { Id = Guid.NewGuid(), IdSucursal = 1, AbiertoEn = DateTime.UtcNow.AddDays(-1), IdEstadoPedido = 5, IsActive = true };
        var pedidoCancelado = new Pedido { Id = Guid.NewGuid(), IdSucursal = 1, AbiertoEn = DateTime.UtcNow.AddDays(-1), IdEstadoPedido = 6, IsActive = true }; // Cancelado
        context.Pedidos.AddRange(pedidoValido, pedidoCancelado);

        // 5 válidas en pedido válido
        context.PedidoDetalles.Add(new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedidoValido.Id, IdProducto = 10, ProductoNombre = "Cerveza Artesanal", Cantidad = 5, PrecioUnitario = 80m, IdEstadoPedidoDetalle = 1, Cancelado = false });
        // 3 canceladas dentro de pedido válido
        context.PedidoDetalles.Add(new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedidoValido.Id, IdProducto = 10, ProductoNombre = "Cerveza Artesanal", Cantidad = 3, PrecioUnitario = 80m, IdEstadoPedidoDetalle = 5, Cancelado = true });
        // 10 en pedido cancelado
        context.PedidoDetalles.Add(new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedidoCancelado.Id, IdProducto = 10, ProductoNombre = "Cerveza Artesanal", Cantidad = 10, PrecioUnitario = 80m, IdEstadoPedidoDetalle = 1, Cancelado = false });

        await context.SaveChangesAsync();

        // Act
        var reporte = await service.ObtenerReporteIngenieriaMenuAsync(1, DateTime.UtcNow.AddDays(-3), DateTime.UtcNow);

        // Assert: Solo deben contabilizarse las 5 unidades válidas
        reporte.TotalUnidadesVendidas.Should().Be(5);
        reporte.TotalVentas.Should().Be(400.00m);
    }

    [Fact]
    public async Task ProductosSinReceta_SeListanEnPendientesDeCosteo_SinRomperCalculos()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new MenuEngineeringService(context);

        var sucursal = new Sucursal { Id = 1, Nombre = "Matriz", IsActive = true };
        var categoria = new CategoriaMenu { Id = 1, Nombre = "Comida", IsActive = true };
        context.Sucursales.Add(sucursal);
        context.CategoriaMenus.Add(categoria);

        var pCosteado = new Producto { Id = 20, Nombre = "Pizza Margherita", IdCategoria = 1, Activo = true };
        var pSinReceta = new Producto { Id = 21, Nombre = "Platillo Nuevo Secreto", IdCategoria = 1, Activo = true };
        context.Productos.AddRange(pCosteado, pSinReceta);

        context.Recetas.Add(new Receta { Id = 20, IdProducto = 20, Nombre = "Receta Pizza", CostoEstimadoUnitario = 40.00m, IsActive = true });
        // pSinReceta no tiene Receta en la BD

        var pedido = new Pedido { Id = Guid.NewGuid(), IdSucursal = 1, AbiertoEn = DateTime.UtcNow.AddDays(-2), IdEstadoPedido = 5, IsActive = true };
        context.Pedidos.Add(pedido);

        context.PedidoDetalles.Add(new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedido.Id, IdProducto = 20, ProductoNombre = "Pizza Margherita", Cantidad = 10, PrecioUnitario = 150m, IdEstadoPedidoDetalle = 1, Cancelado = false });
        context.PedidoDetalles.Add(new PedidoDetalle { Id = Guid.NewGuid(), IdPedido = pedido.Id, IdProducto = 21, ProductoNombre = "Platillo Nuevo Secreto", Cantidad = 4, PrecioUnitario = 200m, IdEstadoPedidoDetalle = 1, Cancelado = false });

        await context.SaveChangesAsync();

        // Act
        var reporte = await service.ObtenerReporteIngenieriaMenuAsync(1, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow);

        // Assert
        reporte.Items.Should().HaveCount(1);
        reporte.Items.First().IdProducto.Should().Be(20);

        reporte.PendientesDeCosteo.Should().HaveCount(1);
        reporte.PendientesDeCosteo.First().IdProducto.Should().Be(21);
        reporte.PendientesDeCosteo.First().NombreProducto.Should().Be("Platillo Nuevo Secreto");
        reporte.PendientesDeCosteo.First().UnidadesVendidas.Should().Be(4);
        reporte.Kpis.CantidadSinCosteo.Should().Be(1);
    }

    [Fact]
    public async Task PeriodoSinVentas_RetornaReporteVacioSeguro()
    {
        // Arrange
        using var context = CreateInMemoryDbContext();
        var service = new MenuEngineeringService(context);

        // Act
        var reporte = await service.ObtenerReporteIngenieriaMenuAsync(1, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

        // Assert
        reporte.Should().NotBeNull();
        reporte.TotalUnidadesVendidas.Should().Be(0);
        reporte.MargenContribucionPromedio.Should().Be(0m);
        reporte.UmbralPopularidadUnidades.Should().Be(0m);
        reporte.Items.Should().BeEmpty();
        reporte.PendientesDeCosteo.Should().BeEmpty();
    }
}

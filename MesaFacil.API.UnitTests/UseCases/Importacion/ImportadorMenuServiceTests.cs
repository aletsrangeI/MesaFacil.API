using ClosedXML.Excel;
using Domain.Entities;
using DTO.Importacion;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Persistence.Context;
using Persistence.Interceptors;
using UseCases.Importacion;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Importacion;

/// <summary>
/// Spec 022: pruebas del Importador Inteligente de Menú y Catálogos (Excel / CSV).
/// Cubre: parseo con conteo de nuevos vs. existentes, detección de errores
/// (precio negativo, categoría vacía, nombre duplicado), modo Merge (actualiza existente)
/// y modo Overwrite (desactiva sin borrar físicamente).
/// </summary>
public class ImportadorMenuServiceTests
{
    private static ApplicationDbContext CrearContextoEnMemoria(string? nombreBd = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: nombreBd ?? $"MesaFacil_Importador_Test_{Guid.NewGuid()}")
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApplicationDbContext(options, new AuditableEntitySaveChangesInterceptor(), new OutboxSaveChangesInterceptor());
    }

    private static async Task<(ApplicationDbContext context, Sucursal sucursal)> CrearSucursalAsync(string? nombreBd = null)
    {
        var context = CrearContextoEnMemoria(nombreBd);

        var empresa = new Empresa { Nombre = "Restaurante de Prueba", Rfc = "RPR010101AA1", IsActive = true };
        context.Empresas.Add(empresa);
        await context.SaveChangesAsync();

        var sucursal = new Sucursal { IdEmpresa = empresa.Id, Nombre = "Sucursal Centro", IsActive = true };
        context.Sucursales.Add(sucursal);
        await context.SaveChangesAsync();

        context.CatMonedas.Add(new CatMoneda { Descripcion = "MXN", IsActive = true });
        context.CatImpuestos.Add(new CatImpuesto { Descripcion = "IVA 16%", IsActive = true });
        await context.SaveChangesAsync();

        return (context, sucursal);
    }

    private static byte[] ConstruirXlsxDeMuestra(bool incluirErrores = false)
    {
        using var workbook = new XLWorkbook();
        var hojaMenu = workbook.Worksheets.Add("Menú y Productos");
        string[] encabezados = { "Categoria", "CodigoInterno", "Producto", "Descripcion", "PrecioVenta", "EstacionCocina", "AplicaInventario", "GruposModificadores" };
        for (int i = 0; i < encabezados.Length; i++) hojaMenu.Cell(1, i + 1).Value = encabezados[i];

        int fila = 2;
        void Agregar(string categoria, string codigo, string producto, string descripcion, object precio, string estacion, string aplicaInv, string grupos)
        {
            hojaMenu.Cell(fila, 1).Value = categoria;
            hojaMenu.Cell(fila, 2).Value = codigo;
            hojaMenu.Cell(fila, 3).Value = producto;
            hojaMenu.Cell(fila, 4).Value = descripcion;
            hojaMenu.Cell(fila, 5).Value = precio is double d ? d : 0;
            if (precio is string s) hojaMenu.Cell(fila, 5).Value = s;
            hojaMenu.Cell(fila, 6).Value = estacion;
            hojaMenu.Cell(fila, 7).Value = aplicaInv;
            hojaMenu.Cell(fila, 8).Value = grupos;
            fila++;
        }

        Agregar("Hamburguesas Artesanales", "HAM-01", "Hamburguesa BBQ Tocino", "Con tocino y BBQ", 165.00, "Parrilla", "SI", "Término de Carne");
        Agregar("Bebidas y Cervezas", "BEB-01", "Cerveza Corona 355ml", "Cerveza clara", 0.00, "Barra", "SI", "");

        if (incluirErrores)
        {
            Agregar("", "X-01", "Producto Sin Categoria", "desc", 50.00, "", "NO", "");
            Agregar("Postres", "POS-01", "Flan Napolitano", "desc", -20.00, "", "NO", "");
            Agregar("Hamburguesas Artesanales", "HAM-02", "Hamburguesa BBQ Tocino", "Duplicado", 170.00, "Parrilla", "SI", "");
        }

        var hojaMod = workbook.Worksheets.Add("Modificadores y Opciones");
        string[] encMod = { "Grupo", "EsObligatorio", "MaxSeleccion", "Opcion", "PrecioExtra" };
        for (int i = 0; i < encMod.Length; i++) hojaMod.Cell(1, i + 1).Value = encMod[i];
        hojaMod.Cell(2, 1).Value = "Término de Carne";
        hojaMod.Cell(2, 2).Value = "SI";
        hojaMod.Cell(2, 3).Value = 1;
        hojaMod.Cell(2, 4).Value = "Término Medio";
        hojaMod.Cell(2, 5).Value = 0.00;
        hojaMod.Cell(3, 1).Value = "Término de Carne";
        hojaMod.Cell(3, 2).Value = "SI";
        hojaMod.Cell(3, 3).Value = 1;
        hojaMod.Cell(3, 4).Value = "Tres Cuartos";
        hojaMod.Cell(3, 5).Value = 0.00;

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    [Fact]
    public async Task PreviewAsync_ArchivoValido_CuentaCategoriasYProductosNuevos()
    {
        var (context, sucursal) = await CrearSucursalAsync();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ImportadorMenuService(context, cache);

        var contenido = ConstruirXlsxDeMuestra();
        using var stream = new MemoryStream(contenido);

        var response = await service.PreviewAsync(stream, "menu.xlsx", ModosImportacionMenu.Merge, sucursal.Id);

        response.isSuccess.Should().BeTrue();
        response.Data!.EsValido.Should().BeTrue();
        response.Data.CategoriasNuevas.Should().Be(2);
        response.Data.ProductosNuevos.Should().Be(2);
        response.Data.ProductosActualizar.Should().Be(0);
        response.Data.GruposModificadoresDetectados.Should().Be(1);
        response.Data.Advertencias.Should().ContainSingle(a => a.Mensaje.Contains("cortesía"));
    }

    [Fact]
    public async Task PreviewAsync_ArchivoConErrores_DetectaCategoriaVaciaPrecioNegativoYDuplicado()
    {
        var (context, sucursal) = await CrearSucursalAsync();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ImportadorMenuService(context, cache);

        var contenido = ConstruirXlsxDeMuestra(incluirErrores: true);
        using var stream = new MemoryStream(contenido);

        var response = await service.PreviewAsync(stream, "menu.xlsx", ModosImportacionMenu.Merge, sucursal.Id);

        response.isSuccess.Should().BeTrue();
        response.Data!.EsValido.Should().BeFalse();
        response.Data.Errores.Should().Contain(e => e.Columna == "Categoria" && e.Mensaje.Contains("no puede estar vacía"));
        response.Data.Errores.Should().Contain(e => e.Columna == "PrecioVenta" && e.Mensaje.Contains("negativo"));
        response.Data.Errores.Should().Contain(e => e.Columna == "Producto" && e.Mensaje.Contains("duplicado"));
    }

    [Fact]
    public async Task ConfirmarAsync_ModoMerge_CreaProductosYActualizaPrecioDeExistente()
    {
        var (context, sucursal) = await CrearSucursalAsync();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ImportadorMenuService(context, cache);

        // Primera importación: crea el catálogo desde cero.
        var contenido1 = ConstruirXlsxDeMuestra();
        using (var stream1 = new MemoryStream(contenido1))
        {
            var preview1 = await service.PreviewAsync(stream1, "menu.xlsx", ModosImportacionMenu.Merge, sucursal.Id);
            preview1.isSuccess.Should().BeTrue();
            var confirmar1 = await service.ConfirmarAsync(preview1.Data!.TokenPreview, ModosImportacionMenu.Merge, sucursal.Id);
            confirmar1.isSuccess.Should().BeTrue();
            confirmar1.Data!.ProductosCreados.Should().Be(2);
            confirmar1.Data.CategoriasCreadas.Should().Be(2);
            confirmar1.Data.GruposModificadoresCreados.Should().Be(1);
            confirmar1.Data.OpcionesModificadoresCreadas.Should().Be(2);
        }

        // Segunda importación (Merge): mismo producto con precio nuevo -> debe actualizar, no duplicar.
        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Menú y Productos");
        string[] encabezados = { "Categoria", "CodigoInterno", "Producto", "Descripcion", "PrecioVenta", "EstacionCocina", "AplicaInventario", "GruposModificadores" };
        for (int i = 0; i < encabezados.Length; i++) hoja.Cell(1, i + 1).Value = encabezados[i];
        hoja.Cell(2, 1).Value = "Hamburguesas Artesanales";
        hoja.Cell(2, 3).Value = "Hamburguesa BBQ Tocino";
        hoja.Cell(2, 5).Value = 199.00;
        hoja.Cell(2, 7).Value = "SI";
        using var ms2 = new MemoryStream();
        workbook.SaveAs(ms2);
        var contenido2 = ms2.ToArray();

        using var stream2 = new MemoryStream(contenido2);
        var preview2 = await service.PreviewAsync(stream2, "menu2.xlsx", ModosImportacionMenu.Merge, sucursal.Id);
        preview2.Data!.ProductosActualizar.Should().Be(1);
        preview2.Data.ProductosNuevos.Should().Be(0);

        var confirmar2 = await service.ConfirmarAsync(preview2.Data.TokenPreview, ModosImportacionMenu.Merge, sucursal.Id);
        confirmar2.isSuccess.Should().BeTrue();
        confirmar2.Data!.ProductosActualizados.Should().Be(1);
        confirmar2.Data.ProductosCreados.Should().Be(0);

        var productoActualizado = await context.Productos
            .Include(p => p.Variantes).ThenInclude(v => v.Precios)
            .FirstAsync(p => p.Nombre == "Hamburguesa BBQ Tocino");
        productoActualizado.Activo.Should().BeTrue();
        productoActualizado.Variantes.Single().Precios.Single().Monto.Should().Be(199.00m);

        // Debe seguir existiendo un solo producto con ese nombre (no se duplicó).
        var totalConEseNombre = await context.Productos.CountAsync(p => p.Nombre == "Hamburguesa BBQ Tocino");
        totalConEseNombre.Should().Be(1);
    }

    [Fact]
    public async Task ConfirmarAsync_ModoOverwrite_DesactivaProductosPreviosSinBorrarlosFisicamente()
    {
        var (context, sucursal) = await CrearSucursalAsync();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ImportadorMenuService(context, cache);

        // Carga inicial en Merge.
        var contenido1 = ConstruirXlsxDeMuestra();
        using (var stream1 = new MemoryStream(contenido1))
        {
            var preview1 = await service.PreviewAsync(stream1, "menu.xlsx", ModosImportacionMenu.Merge, sucursal.Id);
            var confirmar1 = await service.ConfirmarAsync(preview1.Data!.TokenPreview, ModosImportacionMenu.Merge, sucursal.Id);
            confirmar1.isSuccess.Should().BeTrue();
        }

        var idProductoOriginal = await context.Productos
            .Where(p => p.Nombre == "Cerveza Corona 355ml")
            .Select(p => p.Id)
            .FirstAsync();

        // Nueva carta completamente distinta en modo Overwrite.
        using var workbook = new XLWorkbook();
        var hoja = workbook.Worksheets.Add("Menú y Productos");
        string[] encabezados = { "Categoria", "CodigoInterno", "Producto", "Descripcion", "PrecioVenta", "EstacionCocina", "AplicaInventario", "GruposModificadores" };
        for (int i = 0; i < encabezados.Length; i++) hoja.Cell(1, i + 1).Value = encabezados[i];
        hoja.Cell(2, 1).Value = "Ensaladas";
        hoja.Cell(2, 3).Value = "Ensalada César";
        hoja.Cell(2, 5).Value = 120.00;
        hoja.Cell(2, 7).Value = "NO";
        using var ms2 = new MemoryStream();
        workbook.SaveAs(ms2);
        var contenido2 = ms2.ToArray();

        using var stream2 = new MemoryStream(contenido2);
        var preview2 = await service.PreviewAsync(stream2, "menu-nuevo.xlsx", ModosImportacionMenu.Overwrite, sucursal.Id);
        var confirmar2 = await service.ConfirmarAsync(preview2.Data!.TokenPreview, ModosImportacionMenu.Overwrite, sucursal.Id);

        confirmar2.isSuccess.Should().BeTrue();
        confirmar2.Data!.ProductosCreados.Should().Be(1);

        // El producto anterior sigue existiendo en la BD (protege ventas históricas) pero desactivado.
        var productoOriginal = await context.Productos.FindAsync(idProductoOriginal);
        productoOriginal.Should().NotBeNull();
        productoOriginal!.Activo.Should().BeFalse();
        productoOriginal.IsActive.Should().BeFalse();

        var nuevo = await context.Productos.FirstAsync(p => p.Nombre == "Ensalada César");
        nuevo.Activo.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmarAsync_TokenInexistenteOExpirado_RegresaError()
    {
        var (context, sucursal) = await CrearSucursalAsync();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ImportadorMenuService(context, cache);

        var response = await service.ConfirmarAsync(Guid.NewGuid(), ModosImportacionMenu.Merge, sucursal.Id);

        response.isSuccess.Should().BeFalse();
        response.Message.Should().Contain("expiró");
    }

    [Fact]
    public void GenerarPlantilla_DevuelveXlsxConDosPestanasYEncabezadosEsperados()
    {
        var bytes = PlantillaMenuGeneratorService.Generar();
        bytes.Should().NotBeEmpty();

        using var ms = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(ms);

        workbook.Worksheets.Count.Should().Be(2);
        workbook.Worksheets.Any(w => w.Name.Contains("Menú")).Should().BeTrue();
        workbook.Worksheets.Any(w => w.Name.Contains("Modificadores")).Should().BeTrue();
    }
}

using System.Text;
using FluentAssertions;
using UseCases.Importacion;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Importacion;

/// <summary>
/// Spec 022: pruebas puras del parser (sin base de datos) para CSV con codificación Latin1
/// y separador punto y coma, verificando el respeto de acentos/ñ y la limitación documentada
/// de que un CSV solo trae la pestaña de Menú y Productos.
/// </summary>
public class MenuExcelParserServiceTests
{
    [Fact]
    public void Parse_CsvConPuntoYComaYLatin1_RespetaAcentosYEnie()
    {
        var csv = "Categoria;CodigoInterno;Producto;Descripcion;PrecioVenta;EstacionCocina;AplicaInventario;GruposModificadores\n" +
                  "Botanas;BOT-01;Niño Envuelto en Toña;Especialidad de la casa;89.50;Cocina Caliente;SI;\n";

        var bytes = Encoding.Latin1.GetBytes(csv);

        var resultado = MenuExcelParserService.Parse(bytes, "menu.csv");

        resultado.Errores.Should().BeEmpty();
        resultado.EsCsvSinModificadores.Should().BeTrue();
        resultado.Productos.Should().ContainSingle();
        resultado.Productos[0].Producto.Should().Be("Niño Envuelto en Toña");
        resultado.Productos[0].Categoria.Should().Be("Botanas");
        resultado.Productos[0].PrecioVenta.Should().Be(89.50m);
    }

    [Fact]
    public void Parse_CsvConFormatoInvalidoDePrecio_GeneraError()
    {
        var csv = "Categoria,Producto,PrecioVenta\nBebidas,Agua Mineral,no-es-un-precio\n";
        var bytes = Encoding.UTF8.GetBytes(csv);

        var resultado = MenuExcelParserService.Parse(bytes, "menu.csv");

        resultado.Errores.Should().Contain(e => e.Columna == "PrecioVenta" && e.Mensaje.Contains("no es un precio"));
    }

    [Fact]
    public void Parse_ExtensionNoSoportada_GeneraErrorGeneral()
    {
        var resultado = MenuExcelParserService.Parse(new byte[] { 1, 2, 3 }, "menu.pdf");

        resultado.Errores.Should().ContainSingle(e => e.Mensaje.Contains("no soportado"));
    }

    [Fact]
    public void GenerarArchivoMenuCompletoPrueba_CubreTodosLosCasos()
    {
        using var workbook = new ClosedXML.Excel.XLWorkbook();

        // 1. Pestaña Menú y Productos
        var hojaMenu = workbook.Worksheets.Add("Menú y Productos");
        var encabezadosMenu = new[]
        {
            "Categoria", "CodigoInterno", "Producto", "Descripcion", "PrecioVenta",
            "EstacionCocina", "AplicaInventario", "GruposModificadores"
        };
        for (int i = 0; i < encabezadosMenu.Length; i++)
            hojaMenu.Cell(1, i + 1).Value = encabezadosMenu[i];
        hojaMenu.Row(1).Style.Font.Bold = true;

        var productos = new[]
        {
            ("Entradas y Botanas", "BOT-01", "Guacamole Tradicional con Totopos", "Aguacate criollo con pico de gallo y totopos horneados", 95.00, "Cocina Fría", "SI", "Nivel de Picante; Totopos Extra"),
            ("Entradas y Botanas", "BOT-02", "Totopos y Salsas de la Casa", "Cortesía de bienvenida de la casa", 0.00, "Cocina Fría", "SI", "Salsas Extra"),
            ("Entradas y Botanas", "BOT-03", "Queso Fundido con Chistorra", "Queso Gouda fundido al sartén con chistorra artesanal", 135.00, "Cocina Caliente", "SI", "Tipo de Tortilla"),
            ("Cortes y Parrilla", "COR-01", "Rib Eye Sonora 400g", "Corte con marmoleo fino Choice a las brasas", 420.00, "Parrilla", "SI", "Término de Carne; Guarnición"),
            ("Cortes y Parrilla", "COR-02", "Arrachera Norteña 300g", "Marinada con receta de la casa, chiles toreados", 310.00, "Parrilla", "SI", "Término de Carne; Guarnición; Tipo de Tortilla"),
            ("Hamburguesas Gourmet", "HAM-01", "Hamburguesa BBQ Tocino", "Carne 100% angus, tocino ahumado, queso cheddar, salsa BBQ", 175.00, "Parrilla", "SI", "Término de Carne; Tipo de Pan; Extras Hamburguesa"),
            ("Hamburguesas Gourmet", "HAM-02", "Hamburguesa Vegetariana Portobello", "Hongo portobello marinado a la plancha con queso de cabra", 155.00, "Cocina Caliente", "SI", "Tipo de Pan; Extras Hamburguesa"),
            ("Pizzas a la Leña", "PIZ-01", "Pizza Margarita Artesanal", "Salsa pomodoro, mozzarella fresca, albahaca y aceite de oliva", 190.00, "Parrilla", "SI", "Tipo de Orilla"),
            ("Pizzas a la Leña", "PIZ-02", "Pizza Cuatro Quesos", "Mozzarella, parmesano, gorgonzola y queso de cabra", 225.00, "Parrilla", "SI", "Tipo de Orilla"),
            ("Bebidas y Cervezas", "BEB-01", "Cerveza Corona Extra 355ml", "Cerveza clara mexicana en botella", 55.00, "Barra", "SI", ""),
            ("Bebidas y Cervezas", "BEB-02", "Limonada Mineral Natural", "Jugo de limón natural, jarabe y agua mineral", 48.00, "Barra", "SI", "Endulzante Bebida"),
            ("Bebidas y Cervezas", "BEB-03", "Café Americano Gourmet", "Granos de altura chiapanecos recién molidos", 40.00, "Barra", "SI", "Tipo de Leche"),
            ("Postres Artesanales", "POS-01", "Volcán de Chocolate Oaxaqueño", "Centro fundido con helado artesanal de vainilla", 110.00, "Cocina Caliente", "SI", "Sabor de Helado"),
            ("Postres Artesanales", "POS-02", "Cheesecake de Frutos Rojos", "Cremoso estilo New York con mermelada rústica", 95.00, "Cocina Fría", "SI", "")
        };

        for (int r = 0; r < productos.Length; r++)
        {
            var p = productos[r];
            int fila = r + 2;
            hojaMenu.Cell(fila, 1).Value = p.Item1;
            hojaMenu.Cell(fila, 2).Value = p.Item2;
            hojaMenu.Cell(fila, 3).Value = p.Item3;
            hojaMenu.Cell(fila, 4).Value = p.Item4;
            hojaMenu.Cell(fila, 5).Value = p.Item5;
            hojaMenu.Cell(fila, 6).Value = p.Item6;
            hojaMenu.Cell(fila, 7).Value = p.Item7;
            hojaMenu.Cell(fila, 8).Value = p.Item8;
        }
        hojaMenu.Columns().AdjustToContents();

        // 2. Pestaña Modificadores y Opciones
        var hojaModificadores = workbook.Worksheets.Add("Modificadores y Opciones");
        var encabezadosModificadores = new[] { "Grupo", "EsObligatorio", "MaxSeleccion", "Opcion", "PrecioExtra" };
        for (int i = 0; i < encabezadosModificadores.Length; i++)
            hojaModificadores.Cell(1, i + 1).Value = encabezadosModificadores[i];
        hojaModificadores.Row(1).Style.Font.Bold = true;

        var modificadores = new[]
        {
            ("Término de Carne", "SI", 1, "Término Medio", 0.00),
            ("Término de Carne", "SI", 1, "Tres Cuartos", 0.00),
            ("Término de Carne", "SI", 1, "Bien Cocido", 0.00),
            ("Término de Carne", "SI", 1, "Término Rojo (Sellado)", 0.00),
            ("Guarnición", "SI", 1, "Papas a la Francesa", 0.00),
            ("Guarnición", "SI", 1, "Ensalada Verde Mixta", 0.00),
            ("Guarnición", "SI", 1, "Puré de Papa Rústico", 0.00),
            ("Guarnición", "SI", 1, "Espárragos Asados al Grill", 35.00),
            ("Tipo de Tortilla", "SI", 1, "Tortillas de Maíz Azul", 0.00),
            ("Tipo de Tortilla", "SI", 1, "Tortillas de Harina", 0.00),
            ("Tipo de Pan", "SI", 1, "Pan Brioche Artesanal", 0.00),
            ("Tipo de Pan", "SI", 1, "Pan Integral de Granos", 0.00),
            ("Tipo de Pan", "SI", 1, "Pan Pretzel con Mantequilla", 18.00),
            ("Extras Hamburguesa", "NO", 3, "Doble Carne Angus 150g", 55.00),
            ("Extras Hamburguesa", "NO", 3, "Queso Cheddar Extra", 20.00),
            ("Extras Hamburguesa", "NO", 3, "Tocino Crujiente Ahumado", 25.00),
            ("Extras Hamburguesa", "NO", 3, "Huevo Estrellado", 18.00),
            ("Extras Hamburguesa", "NO", 3, "Cebolla Caramelizada", 15.00),
            ("Tipo de Orilla", "SI", 1, "Orilla Delgada Tradicional", 0.00),
            ("Tipo de Orilla", "SI", 1, "Orilla Rellena de Queso Crema", 45.00),
            ("Tipo de Leche", "NO", 1, "Leche Entera", 0.00),
            ("Tipo de Leche", "NO", 1, "Leche Deslactosada Light", 0.00),
            ("Tipo de Leche", "NO", 1, "Leche de Almendras", 15.00),
            ("Tipo de Leche", "NO", 1, "Leche de Avena", 15.00),
            ("Endulzante Bebida", "NO", 1, "Sin Azúcar", 0.00),
            ("Endulzante Bebida", "NO", 1, "Azúcar Estándar", 0.00),
            ("Endulzante Bebida", "NO", 1, "Miel de Agave Orgánica", 10.00),
            ("Sabor de Helado", "SI", 1, "Helado de Vainilla de Papantla", 0.00),
            ("Sabor de Helado", "SI", 1, "Helado de Frutos Rojos", 0.00),
            ("Sabor de Helado", "SI", 1, "Helado de Dulce de Leche", 12.00),
            ("Nivel de Picante", "SI", 1, "No Picante (Sin Chile)", 0.00),
            ("Nivel de Picante", "SI", 1, "Picante Medio (Serrano)", 0.00),
            ("Nivel de Picante", "SI", 1, "Muy Picante (Habanero)", 0.00),
            ("Totopos Extra", "NO", 1, "Porción Extra de Totopos", 25.00),
            ("Salsas Extra", "NO", 2, "Salsa Macha con Ajonjolí", 15.00),
            ("Salsas Extra", "NO", 2, "Salsa Verde Tatemada", 0.00)
        };

        for (int r = 0; r < modificadores.Length; r++)
        {
            var m = modificadores[r];
            int fila = r + 2;
            hojaModificadores.Cell(fila, 1).Value = m.Item1;
            hojaModificadores.Cell(fila, 2).Value = m.Item2;
            hojaModificadores.Cell(fila, 3).Value = m.Item3;
            hojaModificadores.Cell(fila, 4).Value = m.Item4;
            hojaModificadores.Cell(fila, 5).Value = m.Item5;
        }
        hojaModificadores.Columns().AdjustToContents();

        // Guardar archivo en la raíz del proyecto para que el usuario pueda usarlo directamente
        var rutaDestino = @"c:\OrionSys\MesaFacil\Menu_Completo_Prueba_MesaFacil.xlsx";
        workbook.SaveAs(rutaDestino);

        // Validar que el parser oficial de MesaFácil lo lea perfectamente
        var bytes = System.IO.File.ReadAllBytes(rutaDestino);
        var parsed = MenuExcelParserService.Parse(bytes, "Menu_Completo_Prueba_MesaFacil.xlsx");

        parsed.Errores.Should().BeEmpty();
        parsed.Productos.Should().HaveCount(14);
        parsed.Modificadores.Should().HaveCount(36);
        // Debe tener 1 advertencia por el producto de cortesía ($0.00)
        parsed.Advertencias.Should().Contain(a => a.Mensaje.Contains("0.00"));
    }
}

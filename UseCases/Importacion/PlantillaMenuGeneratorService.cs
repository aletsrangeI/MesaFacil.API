using ClosedXML.Excel;

namespace UseCases.Importacion;

/// <summary>
/// Spec 022 - Sección 2.1: genera el archivo Plantilla_Menu_MesaFacil.xlsx oficial,
/// con las 2 pestañas y encabezados exactos que espera MenuExcelParserService, más
/// 1-2 filas de ejemplo para guiar al restaurantero.
/// </summary>
public static class PlantillaMenuGeneratorService
{
    public const string NombreArchivo = "Plantilla_Menu_MesaFacil.xlsx";

    public static byte[] Generar()
    {
        using var workbook = new XLWorkbook();

        var hojaMenu = workbook.Worksheets.Add("Menú y Productos");
        var encabezadosMenu = new[]
        {
            "Categoria", "CodigoInterno", "Producto", "Descripcion", "PrecioVenta",
            "EstacionCocina", "AplicaInventario", "GruposModificadores"
        };
        for (int i = 0; i < encabezadosMenu.Length; i++)
            hojaMenu.Cell(1, i + 1).Value = encabezadosMenu[i];
        hojaMenu.Row(1).Style.Font.Bold = true;

        hojaMenu.Cell(2, 1).Value = "Hamburguesas Artesanales";
        hojaMenu.Cell(2, 2).Value = "HAM-01";
        hojaMenu.Cell(2, 3).Value = "Hamburguesa BBQ Tocino";
        hojaMenu.Cell(2, 4).Value = "Carne 100% angus, tocino ahumado, queso cheddar, salsa BBQ";
        hojaMenu.Cell(2, 5).Value = 165.00;
        hojaMenu.Cell(2, 6).Value = "Parrilla";
        hojaMenu.Cell(2, 7).Value = "SI";
        hojaMenu.Cell(2, 8).Value = "Término de Carne; Tipo de Pan";

        hojaMenu.Cell(3, 1).Value = "Bebidas y Cervezas";
        hojaMenu.Cell(3, 2).Value = "BEB-01";
        hojaMenu.Cell(3, 3).Value = "Cerveza Corona 355ml";
        hojaMenu.Cell(3, 4).Value = "Cerveza clara en botella";
        hojaMenu.Cell(3, 5).Value = 55.00;
        hojaMenu.Cell(3, 6).Value = "Barra";
        hojaMenu.Cell(3, 7).Value = "SI";
        hojaMenu.Cell(3, 8).Value = "";

        hojaMenu.Columns().AdjustToContents();

        var hojaModificadores = workbook.Worksheets.Add("Modificadores y Opciones");
        var encabezadosModificadores = new[] { "Grupo", "EsObligatorio", "MaxSeleccion", "Opcion", "PrecioExtra" };
        for (int i = 0; i < encabezadosModificadores.Length; i++)
            hojaModificadores.Cell(1, i + 1).Value = encabezadosModificadores[i];
        hojaModificadores.Row(1).Style.Font.Bold = true;

        hojaModificadores.Cell(2, 1).Value = "Término de Carne";
        hojaModificadores.Cell(2, 2).Value = "SI";
        hojaModificadores.Cell(2, 3).Value = 1;
        hojaModificadores.Cell(2, 4).Value = "Término Medio";
        hojaModificadores.Cell(2, 5).Value = 0.00;

        hojaModificadores.Cell(3, 1).Value = "Término de Carne";
        hojaModificadores.Cell(3, 2).Value = "SI";
        hojaModificadores.Cell(3, 3).Value = 1;
        hojaModificadores.Cell(3, 4).Value = "Tres Cuartos";
        hojaModificadores.Cell(3, 5).Value = 0.00;

        hojaModificadores.Cell(4, 1).Value = "Tipo de Pan";
        hojaModificadores.Cell(4, 2).Value = "NO";
        hojaModificadores.Cell(4, 3).Value = 1;
        hojaModificadores.Cell(4, 4).Value = "Pan Doble Queso";
        hojaModificadores.Cell(4, 5).Value = 25.00;

        hojaModificadores.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}

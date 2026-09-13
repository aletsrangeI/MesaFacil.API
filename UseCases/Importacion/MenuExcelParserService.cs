using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using DTO.Importacion;

namespace UseCases.Importacion;

/// <summary>
/// Spec 022: parseo y validación semántica de la plantilla de importación de menú.
/// Soporta .xlsx (2 pestañas: "Menú y Productos" y "Modificadores y Opciones", vía ClosedXML)
/// y .csv (una sola "pestaña" equivalente a Menú y Productos - limitación documentada del formato,
/// ya que un CSV no tiene concepto de múltiples hojas).
/// </summary>
public static class MenuExcelParserService
{
    private static readonly string[] EncabezadosProductos =
        { "Categoria", "CodigoInterno", "Producto", "Descripcion", "PrecioVenta", "EstacionCocina", "AplicaInventario", "GruposModificadores" };

    private static readonly string[] EncabezadosModificadores =
        { "Grupo", "EsObligatorio", "MaxSeleccion", "Opcion", "PrecioExtra" };

    public static ResultadoParseoMenu Parse(byte[] contenido, string nombreArchivo)
    {
        var extension = Path.GetExtension(nombreArchivo)?.Trim().ToLowerInvariant() ?? string.Empty;

        return extension switch
        {
            ".csv" => ParseCsv(contenido),
            ".xlsx" or ".xlsm" => ParseXlsx(contenido),
            _ => new ResultadoParseoMenu
            {
                Errores = { new ImportacionIncidenciaDTO { Fila = 0, Mensaje = $"Formato de archivo no soportado: '{extension}'. Usa .xlsx o .csv." } }
            }
        };
    }

    #region XLSX

    private static ResultadoParseoMenu ParseXlsx(byte[] contenido)
    {
        var resultado = new ResultadoParseoMenu();

        using var stream = new MemoryStream(contenido);
        using var workbook = new XLWorkbook(stream);

        var hojaProductos = BuscarHoja(workbook, "menú", "menu", "productos");
        var hojaModificadores = BuscarHoja(workbook, "modificador", "opciones");

        if (hojaProductos == null)
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO
            {
                Fila = 0,
                Mensaje = "No se encontró la pestaña 'Menú y Productos' en el archivo."
            });
            return resultado;
        }

        var mapaColumnas = MapearEncabezados(hojaProductos.FirstRowUsed(), EncabezadosProductos);
        foreach (var fila in hojaProductos.RowsUsed().Skip(1))
        {
            var celdas = new Func<string, string?>(col =>
                mapaColumnas.TryGetValue(col, out var idx) ? fila.Cell(idx).GetString().Trim() : null);

            if (celdas("Categoria") == null && celdas("Producto") == null)
                continue; // fila totalmente vacía

            ProcesarFilaProducto(resultado, fila.RowNumber(), celdas);
        }

        if (hojaModificadores != null)
        {
            var mapaMod = MapearEncabezados(hojaModificadores.FirstRowUsed(), EncabezadosModificadores);
            foreach (var fila in hojaModificadores.RowsUsed().Skip(1))
            {
                var celdas = new Func<string, string?>(col =>
                    mapaMod.TryGetValue(col, out var idx) ? fila.Cell(idx).GetString().Trim() : null);

                if (string.IsNullOrWhiteSpace(celdas("Grupo")) && string.IsNullOrWhiteSpace(celdas("Opcion")))
                    continue;

                ProcesarFilaModificador(resultado, fila.RowNumber(), celdas);
            }
        }

        ValidarDuplicados(resultado);
        return resultado;
    }

    private static IXLWorksheet? BuscarHoja(XLWorkbook workbook, params string[] fragmentos)
    {
        foreach (var ws in workbook.Worksheets)
        {
            var nombreNormalizado = QuitarAcentos(ws.Name).ToLowerInvariant();
            if (fragmentos.Any(f => nombreNormalizado.Contains(QuitarAcentos(f).ToLowerInvariant())))
                return ws;
        }
        return null;
    }

    private static Dictionary<string, int> MapearEncabezados(IXLRow? headerRow, string[] esperados)
    {
        var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (headerRow == null) return mapa;

        var lastCol = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;
        for (int c = 1; c <= lastCol; c++)
        {
            var valor = headerRow.Cell(c).GetString().Trim();
            var match = esperados.FirstOrDefault(e => string.Equals(QuitarAcentos(e), QuitarAcentos(valor), StringComparison.OrdinalIgnoreCase));
            if (match != null)
                mapa[match] = c;
        }
        return mapa;
    }

    #endregion

    #region CSV

    private static ResultadoParseoMenu ParseCsv(byte[] contenido)
    {
        var resultado = new ResultadoParseoMenu { EsCsvSinModificadores = true };
        resultado.Advertencias.Add(new ImportacionIncidenciaDTO
        {
            Fila = 0,
            Mensaje = "Los archivos .csv solo soportan la pestaña 'Menú y Productos'. Para importar Grupos y Opciones de modificadores usa el formato .xlsx."
        });

        var texto = DecodificarBytes(contenido);
        var lineas = texto.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n')
            .Where(l => l.Length > 0)
            .ToList();

        if (lineas.Count == 0)
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = 0, Mensaje = "El archivo CSV está vacío." });
            return resultado;
        }

        char delimitador = DetectarDelimitador(lineas[0]);
        var encabezados = DividirLineaCsv(lineas[0], delimitador);
        var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < encabezados.Count; i++)
        {
            var match = EncabezadosProductos.FirstOrDefault(e => string.Equals(QuitarAcentos(e), QuitarAcentos(encabezados[i]), StringComparison.OrdinalIgnoreCase));
            if (match != null) mapa[match] = i;
        }

        for (int i = 1; i < lineas.Count; i++)
        {
            var numeroFila = i + 1; // 1-based, fila 1 = encabezado
            var valores = DividirLineaCsv(lineas[i], delimitador);

            string? Obtener(string col) =>
                mapa.TryGetValue(col, out var idx) && idx < valores.Count ? valores[idx].Trim() : null;

            if (string.IsNullOrWhiteSpace(Obtener("Categoria")) && string.IsNullOrWhiteSpace(Obtener("Producto")))
                continue;

            ProcesarFilaProducto(resultado, numeroFila, Obtener);
        }

        ValidarDuplicados(resultado);
        return resultado;
    }

    private static string DecodificarBytes(byte[] bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

        try
        {
            var utf8Estricto = new UTF8Encoding(false, throwOnInvalidBytes: true);
            return utf8Estricto.GetString(bytes);
        }
        catch (DecoderFallbackException)
        {
            // Fallback a Latin1 / Windows-1252 para archivos guardados desde Excel en Windows con acentos.
            return Encoding.Latin1.GetString(bytes);
        }
    }

    private static char DetectarDelimitador(string headerLine)
    {
        int comas = headerLine.Count(c => c == ',');
        int puntoYComa = headerLine.Count(c => c == ';');
        return puntoYComa > comas ? ';' : ',';
    }

    private static List<string> DividirLineaCsv(string linea, char delimitador)
    {
        var campos = new List<string>();
        var actual = new StringBuilder();
        bool dentroComillas = false;

        for (int i = 0; i < linea.Length; i++)
        {
            char c = linea[i];
            if (c == '"')
            {
                if (dentroComillas && i + 1 < linea.Length && linea[i + 1] == '"')
                {
                    actual.Append('"');
                    i++;
                }
                else
                {
                    dentroComillas = !dentroComillas;
                }
            }
            else if (c == delimitador && !dentroComillas)
            {
                campos.Add(actual.ToString());
                actual.Clear();
            }
            else
            {
                actual.Append(c);
            }
        }
        campos.Add(actual.ToString());
        return campos;
    }

    #endregion

    #region Validación de renglones

    private static void ProcesarFilaProducto(ResultadoParseoMenu resultado, int fila, Func<string, string?> obtener)
    {
        var categoria = obtener("Categoria") ?? string.Empty;
        var producto = obtener("Producto") ?? string.Empty;
        var precioTexto = obtener("PrecioVenta") ?? string.Empty;

        bool filaValida = true;

        if (string.IsNullOrWhiteSpace(categoria))
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "Categoria", Mensaje = "La categoría no puede estar vacía." });
            filaValida = false;
        }

        if (string.IsNullOrWhiteSpace(producto))
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "Producto", Mensaje = "El nombre del producto es obligatorio." });
            filaValida = false;
        }

        decimal precio = 0m;
        if (string.IsNullOrWhiteSpace(precioTexto))
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "PrecioVenta", Mensaje = "El precio de venta es obligatorio." });
            filaValida = false;
        }
        else if (!decimal.TryParse(precioTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out precio) &&
                 !decimal.TryParse(precioTexto, NumberStyles.Number, CultureInfo.GetCultureInfo("es-MX"), out precio))
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "PrecioVenta", Mensaje = $"'{precioTexto}' no es un precio numérico válido." });
            filaValida = false;
        }
        else if (precio < 0)
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "PrecioVenta", Mensaje = "El precio no puede ser negativo." });
            filaValida = false;
        }
        else if (precio == 0)
        {
            resultado.Advertencias.Add(new ImportacionIncidenciaDTO
            {
                Fila = fila,
                Columna = "PrecioVenta",
                Mensaje = $"El producto '{producto}' tiene precio 0.00 (se asumirá cortesía)."
            });
        }

        if (!filaValida)
            return;

        var grupos = (obtener("GruposModificadores") ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .ToList();

        resultado.Productos.Add(new FilaProductoImportada
        {
            Fila = fila,
            Categoria = categoria.Trim(),
            CodigoInterno = string.IsNullOrWhiteSpace(obtener("CodigoInterno")) ? null : obtener("CodigoInterno")!.Trim(),
            Producto = producto.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(obtener("Descripcion")) ? null : obtener("Descripcion")!.Trim(),
            PrecioVenta = precio,
            EstacionCocina = string.IsNullOrWhiteSpace(obtener("EstacionCocina")) ? null : obtener("EstacionCocina")!.Trim(),
            AplicaInventario = EsAfirmativo(obtener("AplicaInventario")),
            GruposModificadores = grupos
        });
    }

    private static void ProcesarFilaModificador(ResultadoParseoMenu resultado, int fila, Func<string, string?> obtener)
    {
        var grupo = obtener("Grupo") ?? string.Empty;
        var opcion = obtener("Opcion") ?? string.Empty;
        bool filaValida = true;

        if (string.IsNullOrWhiteSpace(grupo))
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "Grupo", Mensaje = "El nombre del grupo de modificadores es obligatorio." });
            filaValida = false;
        }

        if (string.IsNullOrWhiteSpace(opcion))
        {
            resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "Opcion", Mensaje = "El nombre de la opción es obligatorio." });
            filaValida = false;
        }

        decimal precioExtra = 0m;
        var precioExtraTexto = obtener("PrecioExtra");
        if (!string.IsNullOrWhiteSpace(precioExtraTexto))
        {
            if (!decimal.TryParse(precioExtraTexto, NumberStyles.Number, CultureInfo.InvariantCulture, out precioExtra))
            {
                resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "PrecioExtra", Mensaje = $"'{precioExtraTexto}' no es un precio numérico válido." });
                filaValida = false;
            }
            else if (precioExtra < 0)
            {
                resultado.Errores.Add(new ImportacionIncidenciaDTO { Fila = fila, Columna = "PrecioExtra", Mensaje = "El precio extra no puede ser negativo." });
                filaValida = false;
            }
        }

        int maxSeleccion = 1;
        var maxSeleccionTexto = obtener("MaxSeleccion");
        if (!string.IsNullOrWhiteSpace(maxSeleccionTexto) && int.TryParse(maxSeleccionTexto, out var parsedMax) && parsedMax > 0)
            maxSeleccion = parsedMax;

        if (!filaValida)
            return;

        resultado.Modificadores.Add(new FilaModificadorImportada
        {
            Fila = fila,
            Grupo = grupo.Trim(),
            EsObligatorio = EsAfirmativo(obtener("EsObligatorio")),
            MaxSeleccion = maxSeleccion,
            Opcion = opcion.Trim(),
            PrecioExtra = precioExtra
        });
    }

    private static void ValidarDuplicados(ResultadoParseoMenu resultado)
    {
        var vistos = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var fila in resultado.Productos)
        {
            var clave = fila.Producto.Trim();
            if (vistos.ContainsKey(clave))
            {
                resultado.Errores.Add(new ImportacionIncidenciaDTO
                {
                    Fila = fila.Fila,
                    Columna = "Producto",
                    Mensaje = $"Nombre de producto duplicado dentro del archivo: '{fila.Producto}' (ya aparece en la fila {vistos[clave]})."
                });
            }
            else
            {
                vistos[clave] = fila.Fila;
            }
        }
    }

    private static bool EsAfirmativo(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        var v = valor.Trim().ToUpperInvariant();
        return v is "SI" or "SÍ" or "S" or "YES" or "Y" or "TRUE" or "1";
    }

    private static string QuitarAcentos(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalizado)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
            if (categoria != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    #endregion
}

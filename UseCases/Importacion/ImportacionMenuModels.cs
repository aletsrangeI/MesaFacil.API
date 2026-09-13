namespace UseCases.Importacion;

/// <summary>
/// Spec 022: representación interna (post-parseo) de un renglón de la pestaña
/// "Menú y Productos" de la plantilla de importación.
/// </summary>
public class FilaProductoImportada
{
    public int Fila { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string? CodigoInterno { get; set; }
    public string Producto { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioVenta { get; set; }
    public string? EstacionCocina { get; set; }
    public bool AplicaInventario { get; set; }
    public List<string> GruposModificadores { get; set; } = new();
}

/// <summary>
/// Spec 022: representación interna de un renglón de la pestaña "Modificadores y Opciones".
/// </summary>
public class FilaModificadorImportada
{
    public int Fila { get; set; }
    public string Grupo { get; set; } = string.Empty;
    public bool EsObligatorio { get; set; }
    public int MaxSeleccion { get; set; } = 1;
    public string Opcion { get; set; } = string.Empty;
    public decimal PrecioExtra { get; set; }
}

/// <summary>
/// Resultado crudo del parseo de un archivo (xlsx o csv), antes de comparar contra la base de datos.
/// </summary>
public class ResultadoParseoMenu
{
    public List<FilaProductoImportada> Productos { get; set; } = new();
    public List<FilaModificadorImportada> Modificadores { get; set; } = new();
    public List<DTO.Importacion.ImportacionIncidenciaDTO> Errores { get; set; } = new();
    public List<DTO.Importacion.ImportacionIncidenciaDTO> Advertencias { get; set; } = new();

    /// <summary>
    /// True cuando el archivo era .csv: solo se puede leer la pestaña de Menú y Productos
    /// (limitación documentada en Spec 022 - un CSV no tiene "pestañas").
    /// </summary>
    public bool EsCsvSinModificadores { get; set; }
}

/// <summary>
/// Snapshot guardado en IMemoryCache durante los 15 minutos de vida del token de preview,
/// para que ConfirmarAsync no tenga que pedir que se vuelva a subir el archivo.
/// </summary>
public class ImportacionMenuCacheEntry
{
    public int SucursalId { get; set; }
    public string Modo { get; set; } = DTO.Importacion.ModosImportacionMenu.Merge;
    public ResultadoParseoMenu Datos { get; set; } = new();
    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
}

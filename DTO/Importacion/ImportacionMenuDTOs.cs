namespace DTO.Importacion;

/// <summary>
/// Spec 022: modo de importación del catálogo de menú.
/// Merge = conserva lo existente y agrega/actualiza por nombre.
/// Overwrite = da de baja (soft-delete) el catálogo actual de la sucursal antes de insertar el nuevo.
/// </summary>
public static class ModosImportacionMenu
{
    public const string Merge = "Merge";
    public const string Overwrite = "Overwrite";
}

public class ImportacionIncidenciaDTO
{
    public int Fila { get; set; }
    public string? Columna { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta del endpoint de preview: analisis de calidad del archivo sin tocar la base de datos.
/// </summary>
public class ImportarMenuPreviewResponseDTO
{
    public Guid TokenPreview { get; set; }
    public int TotalRenglones { get; set; }
    public int CategoriasNuevas { get; set; }
    public int CategoriasExistentes { get; set; }
    public int ProductosNuevos { get; set; }
    public int ProductosActualizar { get; set; }
    public int GruposModificadoresDetectados { get; set; }
    public int OpcionesModificadoresDetectadas { get; set; }
    public bool EsValido { get; set; }
    public List<ImportacionIncidenciaDTO> Errores { get; set; } = new();
    public List<ImportacionIncidenciaDTO> Advertencias { get; set; } = new();
}

public class ConfirmarImportacionMenuRequestDTO
{
    public Guid TokenPreview { get; set; }
    public string Modo { get; set; } = ModosImportacionMenu.Merge;
    public int SucursalId { get; set; }
}

public class ConfirmarImportacionMenuResponseDTO
{
    public int CategoriasCreadas { get; set; }
    public int ProductosCreados { get; set; }
    public int ProductosActualizados { get; set; }
    public int ProductosDesactivados { get; set; }
    public int GruposModificadoresCreados { get; set; }
    public int OpcionesModificadoresCreadas { get; set; }
}

namespace DTO.Receta;

public class RecetaDetalleDTO
{
    public int Id { get; set; }
    public int IdReceta { get; set; }
    public int? IdInsumo { get; set; }
    public string? InsumoCodigo { get; set; }
    public string? InsumoNombre { get; set; }
    public int? IdSubReceta { get; set; }
    public string? SubRecetaNombre { get; set; }
    public decimal Cantidad { get; set; }
    public int IdUnidadMedida { get; set; }
    public string UnidadMedidaCodigo { get; set; } = string.Empty;
    public string UnidadMedidaNombre { get; set; } = string.Empty;
    public decimal PorcentajeMermaEsperada { get; set; }
    public decimal CostoUnitarioInsumo { get; set; }
    public decimal CostoCalculado { get; set; }
    public decimal ParticipacionPct { get; set; }
}

public class RecetaDTO
{
    public int Id { get; set; }
    public int? IdProducto { get; set; }
    public string? ProductoNombre { get; set; }
    public int? IdVariante { get; set; }
    public string? VarianteNombre { get; set; }
    public int? IdOpcionModificador { get; set; }
    public string? OpcionModificadorNombre { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool EsSubReceta { get; set; }
    public decimal Rendimiento { get; set; }
    public int IdUnidadMedidaRendimiento { get; set; }
    public string UnidadMedidaRendimientoCodigo { get; set; } = string.Empty;
    public string UnidadMedidaRendimientoNombre { get; set; } = string.Empty;
    public decimal CostoEstimadoUnitario { get; set; }
    public decimal CostoTotalLote { get; set; }
    public decimal? PrecioVentaActual { get; set; }
    public decimal? MargenBrutoMonto { get; set; }
    public decimal? MargenBrutoPct { get; set; }
    public decimal? FoodCostPct { get; set; }
    public string NivelSaludMargen { get; set; } = "Optimo"; // Optimo, Ajustado, Critico
    public bool IsActive { get; set; }
    public List<RecetaDetalleDTO> Detalles { get; set; } = new();
}

public class CrearRecetaDetalleDTO
{
    public int? IdInsumo { get; set; }
    public int? IdSubReceta { get; set; }
    public decimal Cantidad { get; set; }
    public int IdUnidadMedida { get; set; }
    public decimal PorcentajeMermaEsperada { get; set; } = 0m;
}

public class CrearRecetaDTO
{
    public int? IdProducto { get; set; }
    public int? IdVariante { get; set; }
    public int? IdOpcionModificador { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool EsSubReceta { get; set; } = false;
    public decimal Rendimiento { get; set; } = 1m;
    public int IdUnidadMedidaRendimiento { get; set; }
    public decimal? PrecioVentaActual { get; set; }
    public List<CrearRecetaDetalleDTO> Detalles { get; set; } = new();
}

public class ActualizarRecetaDTO : CrearRecetaDTO
{
    public bool IsActive { get; set; } = true;
}

public class SimulacionCosteoItemDTO
{
    public int? IdInsumo { get; set; }
    public int? IdSubReceta { get; set; }
    public decimal Cantidad { get; set; }
    public int IdUnidadMedida { get; set; }
    public decimal PorcentajeMermaEsperada { get; set; } = 0m;
}

public class SimulacionCosteoRequestDTO
{
    public decimal Rendimiento { get; set; } = 1m;
    public int IdUnidadMedidaRendimiento { get; set; }
    public decimal? PrecioVenta { get; set; }
    public decimal? MargenObjetivoPct { get; set; }
    public List<SimulacionCosteoItemDTO> Detalles { get; set; } = new();
}

public class SimulacionIngredienteResultDTO
{
    public int? IdInsumo { get; set; }
    public int? IdSubReceta { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal CostoUnitario { get; set; }
    public decimal CantidadEquivalenteBase { get; set; }
    public decimal MermaPct { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal ParticipacionCostoPct { get; set; }
}

public class SimulacionCosteoResponseDTO
{
    public decimal CostoTotalLote { get; set; }
    public decimal CostoPorcion { get; set; }
    public decimal FoodCostPct { get; set; }
    public decimal MargenBrutoPct { get; set; }
    public decimal MargenBrutoMonto { get; set; }
    public decimal PrecioVentaCalculado { get; set; }
    public decimal PrecioVentaConIva { get; set; }
    public string NivelSaludMargen { get; set; } = "Optimo";
    public List<SimulacionIngredienteResultDTO> Desglose { get; set; } = new();
}

public class SubRecetaSimpleDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Rendimiento { get; set; }
    public int IdUnidadMedidaRendimiento { get; set; }
    public string UnidadMedidaCodigo { get; set; } = string.Empty;
    public string UnidadMedidaNombre { get; set; } = string.Empty;
    public decimal CostoEstimadoUnitario { get; set; }
}

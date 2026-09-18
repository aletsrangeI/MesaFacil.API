namespace DTO.Analitica;

public class ItemIngenieriaMenuDTO
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal PrecioVentaPromedio { get; set; }
    public decimal CostoReceta { get; set; }
    public decimal MargenContribucion { get; set; } // PVP - Costo
    public decimal FoodCostPct { get; set; }
    public int UnidadesVendidas { get; set; }
    public decimal PorcentajePopularidad { get; set; } // % sobre total unidades vendidas
    public decimal IngresoTotal { get; set; }
    public decimal UtilidadTotal { get; set; }
    public string Cuadrante { get; set; } = string.Empty; // "Estrella", "CaballoBatalla", "Puzzle", "Perro"
    public string RecomendacionAccion { get; set; } = string.Empty;
}

public class ItemSinCosteoDTO
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int UnidadesVendidas { get; set; }
    public decimal IngresoTotal { get; set; }
    public decimal PrecioVentaPromedio { get; set; }
}

public class MenuEngineeringKpisDTO
{
    public decimal FoodCostPromedioGeneral { get; set; }
    public decimal MargenPromedio { get; set; }
    public string PlatilloMasRentable { get; set; } = string.Empty;
    public decimal MargenPlatilloMasRentable { get; set; }
    public string PlatilloMasVendido { get; set; } = string.Empty;
    public int UnidadesPlatilloMasVendido { get; set; }
    public int CantidadEstrellas { get; set; }
    public int CantidadCaballos { get; set; }
    public int CantidadPuzzles { get; set; }
    public int CantidadPerros { get; set; }
    public int CantidadSinCosteo { get; set; }
}

public class MenuEngineeringReportDTO
{
    public MenuEngineeringKpisDTO Kpis { get; set; } = new();
    public decimal MargenContribucionPromedio { get; set; }
    public decimal UmbralPopularidadUnidades { get; set; }
    public int TotalUnidadesVendidas { get; set; }
    public decimal TotalVentas { get; set; }
    public decimal TotalUtilidadBruta { get; set; }
    public decimal FoodCostPromedioPonderadoPct { get; set; }
    public List<ItemIngenieriaMenuDTO> Items { get; set; } = new();
    public List<ItemSinCosteoDTO> PendientesDeCosteo { get; set; } = new();
}

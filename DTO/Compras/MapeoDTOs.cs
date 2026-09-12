namespace DTO.Compras;

public class MapeoInsumoProveedorDTO
{
    public int Id { get; set; }
    public int IdProveedor { get; set; }
    public string? ProveedorNombre { get; set; }
    public string DescripcionSAT { get; set; } = string.Empty;
    public string ClaveProdServ { get; set; } = string.Empty;
    public string? UnidadSAT { get; set; }
    public int IdInsumo { get; set; }
    public string? InsumoCodigo { get; set; }
    public string? InsumoNombre { get; set; }
    public string? UnidadMedidaNombre { get; set; }
    public decimal FactorConversion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaUltimaCompra { get; set; }
}

public class CrearOActualizarMapeoDTO
{
    public int IdProveedor { get; set; }
    public string DescripcionSAT { get; set; } = string.Empty;
    public string ClaveProdServ { get; set; } = string.Empty;
    public string? UnidadSAT { get; set; }
    public int IdInsumo { get; set; }
    public decimal FactorConversion { get; set; } = 1.0m;
}

namespace DTO.Inventario;

public class UnidadMedidaDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CategoriaInsumoDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool IsActive { get; set; }
}

public class InsumoDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int IdCategoriaInsumo { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public int IdUnidadMedidaBase { get; set; }
    public string UnidadMedidaCodigo { get; set; } = string.Empty;
    public string UnidadMedidaNombre { get; set; } = string.Empty;
    public decimal CostoPromedio { get; set; }
    public decimal UltimoCosto { get; set; }
    public decimal StockMinimo { get; set; }
    public decimal StockMaximo { get; set; }
    public bool EsCritico { get; set; }
    public bool IsActive { get; set; }
    public decimal StockTotalConsolidado { get; set; }
    public decimal ValorizadoConsolidado { get; set; }
}

public class CrearInsumoDTO
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int IdCategoriaInsumo { get; set; }
    public int IdUnidadMedidaBase { get; set; }
    public decimal CostoInicial { get; set; }
    public decimal StockMinimo { get; set; }
    public decimal StockMaximo { get; set; }
    public bool EsCritico { get; set; }
    public int? IdAlmacenInicial { get; set; }
    public decimal StockInicial { get; set; }
}

public class ActualizarInsumoDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int IdCategoriaInsumo { get; set; }
    public int IdUnidadMedidaBase { get; set; }
    public decimal StockMinimo { get; set; }
    public decimal StockMaximo { get; set; }
    public bool EsCritico { get; set; }
    public bool IsActive { get; set; }
}

public class AlmacenDTO
{
    public int Id { get; set; }
    public int IdSucursal { get; set; }
    public string SucursalNombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string TipoAlmacen { get; set; } = string.Empty;
    public bool EsPrincipal { get; set; }
    public bool IsActive { get; set; }
    public int TotalInsumos { get; set; }
    public decimal ValorizadoTotal { get; set; }
}

public class CrearAlmacenDTO
{
    public int IdSucursal { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string TipoAlmacen { get; set; } = "General";
    public bool EsPrincipal { get; set; }
}

public class ExistenciaDTO
{
    public int Id { get; set; }
    public int IdAlmacen { get; set; }
    public string AlmacenNombre { get; set; } = string.Empty;
    public int IdSucursal { get; set; }
    public string SucursalNombre { get; set; } = string.Empty;
    public int IdInsumo { get; set; }
    public string InsumoCodigo { get; set; } = string.Empty;
    public string InsumoNombre { get; set; } = string.Empty;
    public string CategoriaNombre { get; set; } = string.Empty;
    public string UnidadMedidaCodigo { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
    public decimal StockMinimo { get; set; }
    public decimal StockMaximo { get; set; }
    public decimal CostoPromedio { get; set; }
    public decimal Valorizado { get; set; }
    public bool EsCritico { get; set; }
    public string EstadoStock { get; set; } = "Normal"; // Normal, Bajo, Agotado, SobreInventario
    public DateTime FechaUltimoMovimiento { get; set; }
}

public class MovimientoManualDTO
{
    public int IdAlmacen { get; set; }
    public int IdInsumo { get; set; }
    public string TipoMovimiento { get; set; } = "EntradaManual"; // EntradaManual, SalidaMerma, AjusteInventario
    public string? Submotivo { get; set; } // Caducidad, Caída/Accidente, Descomposición, Degustación/Cortesía, CompraCajaChica, AjusteFisico, etc.
    public decimal Cantidad { get; set; }
    public decimal? CostoUnitario { get; set; }
    public string? DocumentoReferencia { get; set; }
    public string? Observaciones { get; set; }
}

public class KardexMovimientoItemDTO
{
    public int Id { get; set; }
    public int IdAlmacen { get; set; }
    public string AlmacenNombre { get; set; } = string.Empty;
    public int IdInsumo { get; set; }
    public string InsumoCodigo { get; set; } = string.Empty;
    public string InsumoNombre { get; set; } = string.Empty;
    public string UnidadMedidaCodigo { get; set; } = string.Empty;
    public string TipoMovimiento { get; set; } = string.Empty;
    public string? Submotivo { get; set; }
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoNuevo { get; set; }
    public decimal CostoPromedioResultante { get; set; }
    public string? DocumentoReferencia { get; set; }
    public string? Observaciones { get; set; }
    public string? UsuarioNombre { get; set; }
    public DateTime FechaHora { get; set; }
}

public class KardexReporteDTO
{
    public int InsumoId { get; set; }
    public string InsumoCodigo { get; set; } = string.Empty;
    public string InsumoNombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public int? AlmacenId { get; set; }
    public string? AlmacenNombre { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal TotalEntradas { get; set; }
    public decimal TotalSalidas { get; set; }
    public decimal SaldoFinal { get; set; }
    public decimal CostoPromedioFinal { get; set; }
    public decimal ValorizadoFinal { get; set; }
    public List<KardexMovimientoItemDTO> Movimientos { get; set; } = new();
}

public class TraspasoItemDTO
{
    public int IdInsumo { get; set; }
    public string InsumoNombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal StockDisponible { get; set; }
}

public class TraspasoCrearDTO
{
    public int IdAlmacenOrigen { get; set; }
    public int IdAlmacenDestino { get; set; }
    public string? Observaciones { get; set; }
    public List<TraspasoItemDTO> Items { get; set; } = new();
}

public class TraspasoResumenDTO
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public int IdAlmacenOrigen { get; set; }
    public string AlmacenOrigenNombre { get; set; } = string.Empty;
    public int IdAlmacenDestino { get; set; }
    public string AlmacenDestinoNombre { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string UsuarioSolicitaNombre { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string? Observaciones { get; set; }
    public int TotalItems { get; set; }
    public List<TraspasoItemDTO> Detalles { get; set; } = new();
}

public class ConteoFisicoItemDTO
{
    public int IdInsumo { get; set; }
    public string InsumoCodigo { get; set; } = string.Empty;
    public string InsumoNombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal StockTeorico { get; set; }
    public decimal StockFisico { get; set; }
    public decimal Discrepancia { get; set; } // StockFisico - StockTeorico
    public decimal CostoPromedio { get; set; }
    public decimal ImpactoMonetario { get; set; } // Discrepancia * CostoPromedio
    public bool EsCritico { get; set; }
}

public class LoteConteoFisicoDTO
{
    public int IdAlmacen { get; set; }
    public string? Observaciones { get; set; }
    public List<ConteoFisicoItemDTO> Conteos { get; set; } = new();
}

public class SucursalSimpleDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class TipoAlmacenDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class MotivoInventarioDTO
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string TipoMovimiento { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class CatalogosBaseInventarioDTO
{
    public List<UnidadMedidaDTO> UnidadesMedida { get; set; } = new();
    public List<CategoriaInsumoDTO> Categorias { get; set; } = new();
    public List<AlmacenDTO> Almacenes { get; set; } = new();
    public List<SucursalSimpleDTO> Sucursales { get; set; } = new();
    public List<TipoAlmacenDTO> TiposAlmacen { get; set; } = new();
    public List<MotivoInventarioDTO> MotivosMovimiento { get; set; } = new();
}

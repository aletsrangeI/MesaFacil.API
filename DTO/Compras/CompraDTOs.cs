namespace DTO.Compras;

public class CompraFacturaDetalleDTO
{
    public int Id { get; set; }
    public int IdCompraFactura { get; set; }
    public int IdInsumo { get; set; }
    public string? InsumoCodigo { get; set; }
    public string InsumoNombre { get; set; } = string.Empty;
    public string? ClaveProdServ { get; set; }
    public string? DescripcionOriginal { get; set; }
    public string? UnidadSAT { get; set; }
    public decimal Cantidad { get; set; }
    public int? IdUnidadMedida { get; set; }
    public string? UnidadMedidaNombre { get; set; }
    public decimal FactorConversion { get; set; }
    public decimal CantidadInsumo { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal Descuento { get; set; }
    public decimal TasaIVA { get; set; }
    public decimal ImporteIVA { get; set; }
    public decimal TasaIEPS { get; set; }
    public decimal ImporteIEPS { get; set; }
    public decimal ImporteTotal { get; set; }
}

public class CompraFacturaDTO
{
    public int Id { get; set; }
    public int IdEmpresa { get; set; }
    public int IdSucursal { get; set; }
    public string SucursalNombre { get; set; } = string.Empty;
    public int IdAlmacen { get; set; }
    public string AlmacenNombre { get; set; } = string.Empty;
    public int IdProveedor { get; set; }
    public string ProveedorRFC { get; set; } = string.Empty;
    public string ProveedorRazonSocial { get; set; } = string.Empty;
    public string? UUID { get; set; }
    public string? Serie { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaRecepcion { get; set; }
    public bool EsCredito { get; set; }
    public int DiasCredito { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TotalDescuento { get; set; }
    public decimal TotalIVA { get; set; }
    public decimal TotalIEPS { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = "Borrador"; // Borrador, Aplicada, Cancelada
    public string? RutaArchivoXML { get; set; }
    public string? RutaArchivoPDF { get; set; }
    public string? Observaciones { get; set; }
    public int? IdUsuario { get; set; }
    public string? UsuarioNombre { get; set; }

    public List<CompraFacturaDetalleDTO> Detalles { get; set; } = new();
}

public class RegistrarCompraDetalleDTO
{
    public int IdInsumo { get; set; }
    public string? ClaveProdServ { get; set; }
    public string? DescripcionOriginal { get; set; }
    public string? UnidadSAT { get; set; }
    public decimal Cantidad { get; set; }
    public int? IdUnidadMedida { get; set; }
    public decimal FactorConversion { get; set; } = 1.0m;
    public decimal CostoUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal Descuento { get; set; } = 0;
    public decimal TasaIVA { get; set; } = 0.16m;
    public decimal ImporteIVA { get; set; }
    public decimal TasaIEPS { get; set; } = 0;
    public decimal ImporteIEPS { get; set; }
    public decimal ImporteTotal { get; set; }
}

public class RegistrarCompraDTO
{
    public int IdEmpresa { get; set; } = 1;
    public int IdSucursal { get; set; }
    public int IdAlmacen { get; set; }
    public int IdProveedor { get; set; }

    // Opcional si el proveedor aún no existe y se envía en caliente
    public CrearProveedorDTO? ProveedorNuevo { get; set; }

    public string? UUID { get; set; }
    public string? Serie { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public bool EsCredito { get; set; } = false;
    public int DiasCredito { get; set; } = 0;
    public DateTime? FechaVencimiento { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TotalDescuento { get; set; } = 0;
    public decimal TotalIVA { get; set; }
    public decimal TotalIEPS { get; set; } = 0;
    public decimal Total { get; set; }

    public string? Observaciones { get; set; }
    public string? XmlContent { get; set; }

    /// <summary>
    /// Si es true, ingresa directamente la mercancía al almacén, genera kárdex y recalcula CPP.
    /// Si es false, queda como Borrador.
    /// </summary>
    public bool AplicarDirecto { get; set; } = true;

    /// <summary>
    /// Si es true, guarda las asociaciones de conceptos SAT con insumos en MapeosInsumoProveedor para futuras compras.
    /// </summary>
    public bool GuardarMapeos { get; set; } = true;

    public List<RegistrarCompraDetalleDTO> Detalles { get; set; } = new();
}

public class CompraFiltroDTO
{
    public int? IdSucursal { get; set; }
    public int? IdAlmacen { get; set; }
    public int? IdProveedor { get; set; }
    public string? Estado { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Buscar { get; set; }
}

public class CompraItemResumenDTO
{
    public int Id { get; set; }
    public string? UUID { get; set; }
    public string? Serie { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaRecepcion { get; set; }
    public string SucursalNombre { get; set; } = string.Empty;
    public string AlmacenNombre { get; set; } = string.Empty;
    public string ProveedorRFC { get; set; } = string.Empty;
    public string ProveedorRazonSocial { get; set; } = string.Empty;
    public bool EsCredito { get; set; }
    public int DiasCredito { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public int CantidadPartidas { get; set; }
}

public class ResumenKpisComprasDTO
{
    public decimal TotalComprasMes { get; set; }
    public int TotalFacturasMes { get; set; }
    public decimal TotalCreditoPendiente { get; set; }
    public int FacturasPendientesPago { get; set; }
    public int FacturasAplicadas { get; set; }
    public int FacturasBorrador { get; set; }
}

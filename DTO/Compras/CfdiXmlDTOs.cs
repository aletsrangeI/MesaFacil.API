namespace DTO.Compras;

public class CfdiXmlParseRequestDTO
{
    public string XmlContent { get; set; } = string.Empty;
    public string? NombreArchivo { get; set; }
}

public class CfdiConceptoDTO
{
    public int Renglon { get; set; }
    public string ClaveProdServ { get; set; } = string.Empty;
    public string? NoIdentificacion { get; set; }
    public decimal Cantidad { get; set; }
    public string? ClaveUnidad { get; set; }
    public string? Unidad { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal ValorUnitario { get; set; }
    public decimal Importe { get; set; }
    public decimal Descuento { get; set; }

    public decimal TasaIVA { get; set; }
    public decimal ImporteIVA { get; set; }
    public decimal TasaIEPS { get; set; }
    public decimal ImporteIEPS { get; set; }
    public decimal ImporteTotal { get; set; }

    // Campos de sugerencia y empatado inteligente
    public int? IdInsumoSugerido { get; set; }
    public string? InsumoNombreSugerido { get; set; }
    public string? InsumoCodigoSugerido { get; set; }
    public string? UnidadMedidaBaseSugerida { get; set; }
    public decimal FactorConversionSugerido { get; set; } = 1.0m;
    public bool SugeridoPorMapeo { get; set; } = false;
    public int? IdMapeoExistente { get; set; }
}

public class CfdiParseResultDTO
{
    public string? UUID { get; set; }
    public string? Serie { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string? FormaPago { get; set; }
    public string? MetodoPago { get; set; }
    public string? CondicionesDePago { get; set; }
    public string Moneda { get; set; } = "MXN";

    // Emisor (Proveedor)
    public string RfcEmisor { get; set; } = string.Empty;
    public string NombreEmisor { get; set; } = string.Empty;
    public string? RegimenFiscalEmisor { get; set; }

    // Receptor (Restaurante / Empresa)
    public string RfcReceptor { get; set; } = string.Empty;
    public string NombreReceptor { get; set; } = string.Empty;

    // Totales
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal TotalIVA { get; set; }
    public decimal TotalIEPS { get; set; }
    public decimal Total { get; set; }

    // Detección de proveedor
    public int? ProveedorExistenteId { get; set; }
    public string? ProveedorExistenteNombre { get; set; }
    public int? ProveedorExistenteDiasCredito { get; set; }
    public bool EsProveedorNuevo { get; set; }

    // Detección de duplicidad de UUID fiscal
    public bool FacturaYaExiste { get; set; }
    public int? FacturaExistenteId { get; set; }
    public string? FacturaExistenteEstado { get; set; }
    public string? MensajeValidacion { get; set; }

    // Conceptos
    public List<CfdiConceptoDTO> Conceptos { get; set; } = new();
}

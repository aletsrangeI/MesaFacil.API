namespace Domain.Entities;

/// <summary>
/// Spec 023: tipo de interfaz física de la impresora térmica.
/// </summary>
public enum TipoConexionImpresora
{
    /// <summary>Impresora Ethernet/LAN con IP fija, puerto 9100 (protocolo ESC/POS crudo por socket).</summary>
    RedLAN = 0,

    /// <summary>Impresora conectada por USB al equipo local (spooler de Windows).</summary>
    USBLocal = 1,

    /// <summary>Fallback: impresión vía diálogo del navegador (window.print), sin backend.</summary>
    NavegadorDialogo = 2
}

/// <summary>
/// Spec 023: ancho de papel térmico soportado por la impresora, en milímetros.
/// </summary>
public enum AnchoPapelImpresora
{
    Mm58 = 58,
    Mm80 = 80
}

/// <summary>
/// Spec 023: configuración de una impresora térmica física registrada en una sucursal
/// (ticketera de caja, comandera de cocina/barra, etc.), usada por el motor de
/// autodiagnóstico y por el envío de tickets de prueba ESC/POS.
/// </summary>
public class ConfiguracionImpresora : BaseAuditableEntity
{
    public int IdSucursal { get; set; }
    public string? Nombre { get; set; }
    public TipoConexionImpresora TipoConexion { get; set; } = TipoConexionImpresora.RedLAN;
    public AnchoPapelImpresora AnchoPapel { get; set; } = AnchoPapelImpresora.Mm80;
    public string? DireccionIp { get; set; }
    public int Puerto { get; set; } = 9100;
    public bool AperturaCajon { get; set; }
    public bool Autocorte { get; set; } = true;

    /// <summary>
    /// Estación de cocina/caja opcional a la que se asocia esta impresora (ej. "Parrilla",
    /// "Caja"). Se guarda como texto libre para no acoplar el diagnóstico de hardware a la
    /// entidad operativa EstacionCocina.
    /// </summary>
    public string? EstacionAsociada { get; set; }

    public Sucursal Sucursal { get; set; } = null!;
}

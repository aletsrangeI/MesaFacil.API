using System;

namespace Domain.Entities;

public class Precio : BaseAuditableEntity // Asume que hereda IdPrecio
{
    public int IdVariante { get; set; }
    public decimal Monto { get; set; }
    
    // Se mantiene como acceso rápido/snapshot, aunque ya tienes el catálogo
    public string Moneda { get; set; } = "MXN"; 

    // [CORREGIDO] - Catálogos tipados en lugar de genéricos
    public int IdImpuesto { get; set; }
    public int IdMoneda { get; set; }

    public DateTime? ValidoDesde { get; set; }
    public DateTime? ValidoHasta { get; set; }
    
    public string? Dias { get; set; }
    public string? Horario { get; set; }

    // ==========================================
    // PROPIEDADES DE NAVEGACIÓN
    // ==========================================
    public VarianteProducto Variante { get; set; } = null!;
    
    // [NUEVAS] - Relaciones con los catálogos específicos
    public CatImpuesto Impuesto { get; set; } = null!;
    public CatMoneda CatMoneda { get; set; } = null!;
}
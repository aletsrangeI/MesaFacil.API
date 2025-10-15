namespace Domain.Entities;

public class Precio : BaseAuditableEntity
{
    public int IdVariante { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "MXN";

    public int ImpuestoCatalogId { get; set; }
    public int ImpuestoItemId { get; set; }

    public DateTime? ValidoDesde { get; set; }
    public DateTime? ValidoHasta { get; set; }
    public string? Dias { get; set; }
    public string? Horario { get; set; }

    public VarianteProducto Variante { get; set; } = null!;
    public CatalogItem ImpuestoItem { get; set; } = null!;
}
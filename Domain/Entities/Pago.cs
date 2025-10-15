namespace Domain.Entities;

public class Pago : BaseAuditableEntity
{
    public int IdCuenta { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "MXN";
    public decimal Propina { get; set; }
    public DateTime PagadoEn { get; set; }
    public string? Referencia { get; set; }
    public int? RecibidoPor { get; set; }

    public int MetodoCatalogId { get; set; }
    public int MetodoItemId { get; set; }

    public Cuenta Cuenta { get; set; } = null!;
    public Usuario? RecibidoPorUsuario { get; set; }
    public CatalogItem MetodoItem { get; set; } = null!;
}
namespace Domain.Entities;

public class DescuentoAplicado : BaseAuditableEntity
{
    public int IdCuenta { get; set; }

    public int TipoCatalogId { get; set; }
    public int TipoItemId { get; set; }

    public decimal Valor { get; set; }
    public string? Alcance { get; set; }
    public string? Condiciones { get; set; }

    public Cuenta Cuenta { get; set; } = null!;
    public CatalogItem TipoItem { get; set; } = null!;
}
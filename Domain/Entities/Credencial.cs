namespace Domain.Entities;

public class Credencial : BaseAuditableEntity
{
    public int IdUsuario { get; set; }

    public int TipoCatalogId { get; set; }
    public int TipoItemId { get; set; }

    public string Hash { get; set; } = null!;
    public string? Salt { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public CatalogItem TipoItem { get; set; } = null!;
}
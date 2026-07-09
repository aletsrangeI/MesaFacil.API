namespace Domain.Entities;

public class CatEstadoCuenta : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; }
}
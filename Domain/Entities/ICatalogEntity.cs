namespace Domain.Entities;

/// <summary>
/// Contrato mínimo que deben implementar todas las entidades de catálogo simple.
/// Cualquier entidad Cat* que herede de BaseAuditableEntity e implemente esta interfaz
/// puede participar en el flujo CRUD genérico.
/// </summary>
public interface ICatalogEntity
{
    string Descripcion { get; set; }
}

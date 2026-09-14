namespace Domain.Entities;

/// <summary>
/// Spec 024: catálogo de motivos obligatorios para autorizar (con PIN de supervisor) la
/// cancelación de un platillo ya enviado a cocina. Distinto de CatMotivoCancelacionPedido
/// (spec de Delivery/anulación completa de pedido, motivos como "Repartidor accidentado"),
/// que pertenece a un dominio de negocio diferente.
/// </summary>
public class CatMotivoCancelacion : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
}

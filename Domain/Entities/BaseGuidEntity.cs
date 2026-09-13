using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Base entity for operational/transactional aggregates that require globally-unique,
/// client- or edge-generatable identifiers (UUIDv7) instead of database-assigned integers.
/// Introduced by Spec 019 (Offline Resilience / Edge-Cloud) so Pedido, PedidoDetalle,
/// PedidoAsiento, PedidoModificador, TicketCocina, Pago y MovimientoCaja can be created
/// locally (Edge Node) without colliding with records created in the Cloud or in another branch.
/// </summary>
public abstract class BaseGuidEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public bool IsActive { get; set; }
}

public abstract class BaseAuditableGuidEntity : BaseGuidEntity
{
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}

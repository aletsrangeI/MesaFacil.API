namespace Domain.Entities;

public class EventoPedido : BaseAuditableEntity // Id propio permanece int; FK a Pedido es Guid (Spec 019)
{
    public Guid IdPedido { get; set; }
    public int? IdUsuario { get; set; }
    public string? TipoEvento { get; set; }
    public string? Payload { get; set; }

    // ==========================================
    // Spec 024: Candado de Supervisor y Auditoría de Cancelaciones
    // ==========================================
    // IdUsuario (arriba) sigue representando al usuario que generó/disparó el evento (p.ej. el
    // mesero). IdUsuarioSupervisor es quien autorizó con su PIN (Gerente/Administrador).
    // Detalles adicionales (IdPedidoDetalle, motivo, producto, mesa) se guardan en Payload (JSON)
    // para no expandir el esquema más allá de los 3 campos que pide tasks.md.
    public int? IdUsuarioSupervisor { get; set; }
    public decimal? MontoCancelado { get; set; }
    public decimal? PorcentajeDescuento { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Usuario? Usuario { get; set; }
    public Usuario? UsuarioSupervisor { get; set; }
}
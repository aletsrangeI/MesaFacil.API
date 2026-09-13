namespace DTO.Seguridad;

/// <summary>
/// Spec 024, sección 3. Nota: idPedido es Guid (no int como en el draft original del spec)
/// porque Pedido.Id ya es Guid/UUIDv7 desde spec 019.
/// </summary>
public class AutorizarSupervisorPinRequestDTO
{
    public string Pin { get; set; } = string.Empty;

    /// <summary>"CancelarPlatilloCocina", "DescuentoExcesivo" ó "CancelarCuenta".</summary>
    public string AccionProtegida { get; set; } = string.Empty;

    public Guid IdPedido { get; set; }
    public Guid? IdPedidoDetalle { get; set; }
    public string Motivo { get; set; } = string.Empty;
}

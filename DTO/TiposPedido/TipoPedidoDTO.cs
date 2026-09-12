namespace DTO.TiposPedido;

public class TipoPedidoDTO
{
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public bool IsComedor { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

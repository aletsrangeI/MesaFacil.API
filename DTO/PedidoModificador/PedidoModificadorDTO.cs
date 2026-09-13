namespace DTO.PedidoModificador;

public class PedidoModificadorDTO
{
    public Guid Id { get; set; }
    public Guid IdDetalle { get; set; }
    public int IdOpcion { get; set; }
    public decimal PrecioExtra { get; set; }
}
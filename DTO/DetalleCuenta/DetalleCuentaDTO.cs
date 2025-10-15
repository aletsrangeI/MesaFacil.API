namespace DTO.DetalleCuenta;

public class DetalleCuentaDTO
{
    public int Id { get; set; }
    public int IdCuenta { get; set; }
    public string TipoOrigen { get; set; }
    public int? IdOrigen { get; set; }
    public string? Descripcion { get; set; }
    public decimal Monto { get; set; }
}

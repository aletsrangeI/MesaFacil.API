namespace DTO.Mesa;

public class MesaDTO
{
    public int Id { get; set; }
    public int IdSucursal { get; set; }
    public int? IdArea { get; set; }
    public string Codigo { get; set; }
    public int Asientos { get; set; }
    public int IdEstadoMesa { get; set; }
}

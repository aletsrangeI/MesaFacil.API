namespace DTO.OpcionModificador;

public class OpcionModificadorDTO
{
    public int Id { get; set; }
    public int IdGrupo { get; set; }
    public string? Nombre { get; set; }
    public decimal PrecioExtra { get; set; }
    public bool EsDefault { get; set; }
    public bool Activo { get; set; }
}

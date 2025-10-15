namespace DTO.Credencial;

public class CredencialDTO
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public int TipoCatalogId { get; set; }
    public int TipoItemId { get; set; }
    public string Hash { get; set; }
    public string? Salt { get; set; }
    public DateTime? ActualizadoEn { get; set; }
}

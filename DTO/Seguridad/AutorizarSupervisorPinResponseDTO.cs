namespace DTO.Seguridad;

public class AutorizarSupervisorPinResponseDTO
{
    public bool Autorizado { get; set; }
    public int? SupervisorId { get; set; }
    public string? NombreSupervisor { get; set; }
    public string? TokenAutorizacion { get; set; }
    public string? Mensaje { get; set; }
    public bool Bloqueado { get; set; }
    public DateTime? BloqueadoHastaUtc { get; set; }
}

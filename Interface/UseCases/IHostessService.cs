using DTO.Hostess;

namespace Interface.UseCases;

public interface IHostessService
{
    // Waitlist (Fila de espera digital)
    Task<IEnumerable<FilaEsperaItemDTO>> GetWaitlistAsync(int idSucursal);
    Task<FilaEsperaItemDTO> RegistrarEnWaitlistAsync(RegistrarWaitlistDTO dto);
    Task<FilaEsperaItemDTO> NotificarWaitlistAsync(int id);
    Task<bool> SentarWaitlistAsync(int id, SentarWaitlistDTO dto);
    Task<bool> CancelarWaitlistAsync(int id);
    Task<int> CalcularMinutosEstimadosAsync(int idSucursal, int numeroPersonas, string? zonaPreferencia = null);

    // Reservaciones
    Task<IEnumerable<ReservaMesaDTO>> GetReservasAsync(int idSucursal, DateTime fecha);
    Task<ReservaMesaDTO> CrearReservaAsync(CrearReservaDTO dto);
    Task<bool> ConfirmarLlegadaReservaAsync(int id, ConfirmarLlegadaReservaDTO dto);
    Task<bool> CancelarReservaAsync(int id, string? motivo = null);

    // Dashboard resumen
    Task<HostessDashboardSummaryDTO> GetSummaryAsync(int idSucursal);
}

using Common;
using DTO.Auditoria;

namespace Interface.UseCases;

/// <summary>Spec 024: monitor de auditoría y umbral del 2% de cancelaciones por turno.</summary>
public interface IAuditoriaCancelacionesService
{
    Task<Response<ResumenCancelacionesTurnoDTO>> ObtenerResumenTurnoAsync(int idTurno, CancellationToken ct = default);

    Task<Response<List<MotivoCancelacionDTO>>> ObtenerMotivosAsync(CancellationToken ct = default);
}

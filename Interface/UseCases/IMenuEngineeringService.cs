using DTO.Analitica;

namespace Interface.UseCases;

public interface IMenuEngineeringService
{
    Task<MenuEngineeringReportDTO> ObtenerReporteIngenieriaMenuAsync(
        int? idSucursal,
        DateTime fechaInicio,
        DateTime fechaFin,
        int? idCategoria = null);
}

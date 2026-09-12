using Common;
using DTO.CxP;

namespace UseCases.CxP;

public interface ICxPService
{
    Task<Response<List<CuentaPorPagarItemDTO>>> ObtenerListadoAsync(FiltroCxPDTO filtro);
    Task<Response<CuentaPorPagarDTO>> ObtenerPorIdAsync(int id);
    Task<Response<ResumenKpisCxPDTO>> ObtenerKpisAsync(int? idSucursal);
    Task<Response<PagoCuentaPorPagarDTO>> RegistrarAbonoAsync(RegistrarPagoCxPDTO dto, int? idUsuario);
    Task<Response<bool>> CancelarCuentaPorPagarAsync(int id, int? idUsuario, string? motivo);
    Task<Response<ReporteAntiguedadSaldosDTO>> ObtenerReporteAntiguedadSaldosAsync(int? idSucursal);
    Task<Response<EstadoCuentaProveedorDTO>> ObtenerEstadoCuentaProveedorAsync(int idProveedor, int? idSucursal);
    Task<Response<List<TurnoActivoDTO>>> ObtenerTurnosActivosAsync(int idSucursal);
}

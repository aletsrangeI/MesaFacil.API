using Common;
using DTO.Cuenta;

namespace Interface.UseCases;

public interface ICuentaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(CuentaDTO dto);
    Response<bool> Update(CuentaDTO dto);
    Response<bool> Delete(int id);
    Response<CuentaDTO> Get(int id);
    Response<IEnumerable<CuentaDTO>> GetAll();
    ResponsePagination<IEnumerable<CuentaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(CuentaDTO dto);
    Task<Response<bool>> UpdateAsync(CuentaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<CuentaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<CuentaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<CuentaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    Task<Response<CuentaDTO>> GenerarCuentaAsync(Guid idPedido);
    Response<CuentaDTO> GenerarCuenta(Guid idPedido);

    #endregion
}
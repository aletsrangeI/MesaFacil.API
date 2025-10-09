using Common;
using DTO.MovimientoCaja;

namespace Interface.UseCases;

public interface IMovimientoCajaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(MovimientoCajaDTO dto);
    Response<bool> Update(MovimientoCajaDTO dto);
    Response<bool> Delete(int id);
    Response<MovimientoCajaDTO> Get(int id);
    Response<IEnumerable<MovimientoCajaDTO>> GetAll();
    ResponsePagination<IEnumerable<MovimientoCajaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(MovimientoCajaDTO dto);
    Task<Response<bool>> UpdateAsync(MovimientoCajaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<MovimientoCajaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<MovimientoCajaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<MovimientoCajaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
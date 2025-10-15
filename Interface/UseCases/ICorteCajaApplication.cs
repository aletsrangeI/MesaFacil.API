using Common;
using DTO.CorteCaja;

namespace Interface.UseCases;

public interface ICorteCajaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(CorteCajaDTO dto);
    Response<bool> Update(CorteCajaDTO dto);
    Response<bool> Delete(int id);
    Response<CorteCajaDTO> Get(int id);
    Response<IEnumerable<CorteCajaDTO>> GetAll();
    ResponsePagination<IEnumerable<CorteCajaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(CorteCajaDTO dto);
    Task<Response<bool>> UpdateAsync(CorteCajaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<CorteCajaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<CorteCajaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<CorteCajaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
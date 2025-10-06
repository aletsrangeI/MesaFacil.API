using Common;
using DTO.Area;

namespace Interface.UseCases;

public interface IAreaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(AreaDTO dto);
    Response<bool> Update(AreaDTO dto);
    Response<bool> Delete(int id);
    Response<AreaDTO> Get(int id);
    Response<IEnumerable<AreaDTO>> GetAll();
    ResponsePagination<IEnumerable<AreaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(AreaDTO dto);
    Task<Response<bool>> UpdateAsync(AreaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<AreaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<AreaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<AreaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
using Common;
using DTO.Menu;

namespace Interface.UseCases;

public interface IMenuApplication
{
    #region Metodos sincronos

    Response<bool> Insert(MenuDTO dto);
    Response<bool> Update(MenuDTO dto);
    Response<bool> Delete(int id);
    Response<MenuDTO> Get(int id);
    Response<IEnumerable<MenuDTO>> GetAll();
    ResponsePagination<IEnumerable<MenuDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(MenuDTO dto);
    Task<Response<bool>> UpdateAsync(MenuDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<MenuDTO>> GetAsync(int id);
    Task<Response<IEnumerable<MenuDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<MenuDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
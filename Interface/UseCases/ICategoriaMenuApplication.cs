using Common;
using DTO.CategoriaMenu;

namespace Interface.UseCases;

public interface ICategoriaMenuApplication
{
    #region Metodos sincronos

    Response<bool> Insert(CategoriaMenuDTO dto);
    Response<bool> Update(CategoriaMenuDTO dto);
    Response<bool> Delete(int id);
    Response<CategoriaMenuDTO> Get(int id);
    Response<IEnumerable<CategoriaMenuDTO>> GetAll();
    ResponsePagination<IEnumerable<CategoriaMenuDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(CategoriaMenuDTO dto);
    Task<Response<bool>> UpdateAsync(CategoriaMenuDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<CategoriaMenuDTO>> GetAsync(int id);
    Task<Response<IEnumerable<CategoriaMenuDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<CategoriaMenuDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
using Common;
using DTO.CatCredencial;

namespace Interface.UseCases;

public interface ICatCredencialApplication
{
    #region Metodos sincronos

    Response<bool> Insert(CatCredencialDTO dto);
    Response<bool> Update(CatCredencialDTO dto);
    Response<bool> Delete(int id);
    Response<CatCredencialDTO> Get(int id);
    Response<IEnumerable<CatCredencialDTO>> GetAll();
    ResponsePagination<IEnumerable<CatCredencialDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(CatCredencialDTO dto);
    Task<Response<bool>> UpdateAsync(CatCredencialDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<CatCredencialDTO>> GetAsync(int id);
    Task<Response<IEnumerable<CatCredencialDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<CatCredencialDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
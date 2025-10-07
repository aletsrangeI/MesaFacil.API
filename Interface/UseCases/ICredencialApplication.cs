using Common;
using DTO.Credencial;

namespace Interface.UseCases;

public interface ICredencialApplication
{
    #region Metodos sincronos

    Response<bool> Insert(CredencialDTO dto);
    Response<bool> Update(CredencialDTO dto);
    Response<bool> Delete(int id);
    Response<CredencialDTO> Get(int id);
    Response<IEnumerable<CredencialDTO>> GetAll();
    ResponsePagination<IEnumerable<CredencialDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(CredencialDTO dto);
    Task<Response<bool>> UpdateAsync(CredencialDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<CredencialDTO>> GetAsync(int id);
    Task<Response<IEnumerable<CredencialDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<CredencialDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
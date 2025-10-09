using Common;
using DTO.OpcionModificador;

namespace Interface.UseCases;

public interface IOpcionModificadorApplication
{
    #region Metodos sincronos

    Response<bool> Insert(OpcionModificadorDTO dto);
    Response<bool> Update(OpcionModificadorDTO dto);
    Response<bool> Delete(int id);
    Response<OpcionModificadorDTO> Get(int id);
    Response<IEnumerable<OpcionModificadorDTO>> GetAll();
    ResponsePagination<IEnumerable<OpcionModificadorDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(OpcionModificadorDTO dto);
    Task<Response<bool>> UpdateAsync(OpcionModificadorDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<OpcionModificadorDTO>> GetAsync(int id);
    Task<Response<IEnumerable<OpcionModificadorDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<OpcionModificadorDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
using Common;
using DTO.GrupoModificador;

namespace Interface.UseCases;

public interface IGrupoModificadorApplication
{
    #region Metodos sincronos

    Response<bool> Insert(GrupoModificadorDTO dto);
    Response<bool> Update(GrupoModificadorDTO dto);
    Response<bool> Delete(int id);
    Response<GrupoModificadorDTO> Get(int id);
    Response<IEnumerable<GrupoModificadorDTO>> GetAll();
    ResponsePagination<IEnumerable<GrupoModificadorDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(GrupoModificadorDTO dto);
    Task<Response<bool>> UpdateAsync(GrupoModificadorDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<GrupoModificadorDTO>> GetAsync(int id);
    Task<Response<IEnumerable<GrupoModificadorDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<GrupoModificadorDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
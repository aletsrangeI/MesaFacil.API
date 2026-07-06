using Common;
using DTO.Formulario;

namespace Interface.UseCases;

public interface IFormularioApplication
{
    #region Metodos sincronos

    Response<bool> Insert(FormularioDTO dto);
    Response<bool> Update(FormularioDTO dto);
    Response<bool> Delete(int id);
    Response<FormularioDTO> Get(int id);
    Response<IEnumerable<FormularioDTO>> GetAll();
    ResponsePagination<IEnumerable<FormularioDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(FormularioDTO dto);
    Task<Response<bool>> UpdateAsync(FormularioDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<FormularioDTO>> GetAsync(int id);
    Task<Response<IEnumerable<FormularioDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<FormularioDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
using Common;
using DTO.Empresa;

namespace Interface.UseCases;

public interface IEmpresaApplication
{
    #region Metodos sincronos

    Response<bool> Insert(EmpresaDTO dto);
    Response<bool> Update(EmpresaDTO dto);
    Response<bool> Delete(int id);
    Response<EmpresaDTO> Get(int id);
    Response<IEnumerable<EmpresaDTO>> GetAll();
    ResponsePagination<IEnumerable<EmpresaDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(EmpresaDTO dto);
    Task<Response<bool>> UpdateAsync(EmpresaDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<EmpresaDTO>> GetAsync(int id);
    Task<Response<IEnumerable<EmpresaDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<EmpresaDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
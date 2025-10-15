using Common;
using DTO.DescuentoAplicado;

namespace Interface.UseCases;

public interface IDescuentoAplicadoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(DescuentoAplicadoDTO dto);
    Response<bool> Update(DescuentoAplicadoDTO dto);
    Response<bool> Delete(int id);
    Response<DescuentoAplicadoDTO> Get(int id);
    Response<IEnumerable<DescuentoAplicadoDTO>> GetAll();
    ResponsePagination<IEnumerable<DescuentoAplicadoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(DescuentoAplicadoDTO dto);
    Task<Response<bool>> UpdateAsync(DescuentoAplicadoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<DescuentoAplicadoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<DescuentoAplicadoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<DescuentoAplicadoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
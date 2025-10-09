using Common;
using DTO.VarianteProducto;

namespace Interface.UseCases;

public interface IVarianteProductoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(VarianteProductoDTO dto);
    Response<bool> Update(VarianteProductoDTO dto);
    Response<bool> Delete(int id);
    Response<VarianteProductoDTO> Get(int id);
    Response<IEnumerable<VarianteProductoDTO>> GetAll();
    ResponsePagination<IEnumerable<VarianteProductoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(VarianteProductoDTO dto);
    Task<Response<bool>> UpdateAsync(VarianteProductoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<VarianteProductoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<VarianteProductoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<VarianteProductoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
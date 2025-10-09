using Common;
using DTO.Producto;

namespace Interface.UseCases;

public interface IProductoApplication
{
    #region Metodos sincronos

    Response<bool> Insert(ProductoDTO dto);
    Response<bool> Update(ProductoDTO dto);
    Response<bool> Delete(int id);
    Response<ProductoDTO> Get(int id);
    Response<IEnumerable<ProductoDTO>> GetAll();
    ResponsePagination<IEnumerable<ProductoDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(ProductoDTO dto);
    Task<Response<bool>> UpdateAsync(ProductoDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<ProductoDTO>> GetAsync(int id);
    Task<Response<IEnumerable<ProductoDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<ProductoDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
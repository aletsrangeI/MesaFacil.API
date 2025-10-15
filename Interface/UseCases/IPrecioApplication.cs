using Common;
using DTO.Precio;

namespace Interface.UseCases;

public interface IPrecioApplication
{
    #region Metodos sincronos

    Response<bool> Insert(PrecioDTO dto);
    Response<bool> Update(PrecioDTO dto);
    Response<bool> Delete(int id);
    Response<PrecioDTO> Get(int id);
    Response<IEnumerable<PrecioDTO>> GetAll();
    ResponsePagination<IEnumerable<PrecioDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(PrecioDTO dto);
    Task<Response<bool>> UpdateAsync(PrecioDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<PrecioDTO>> GetAsync(int id);
    Task<Response<IEnumerable<PrecioDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<PrecioDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
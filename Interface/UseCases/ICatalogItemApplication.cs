using Common;
using DTO.CatalogItem;

namespace Interface.UseCases;

public interface ICatalogItemApplication
{
    #region Metodos sincronos

    Response<bool> Insert(CatalogItemDTO dto);
    Response<bool> Update(CatalogItemDTO dto);
    Response<bool> Delete(int id);
    Response<CatalogItemDTO> Get(int id);
    Response<IEnumerable<CatalogItemDTO>> GetAll();
    ResponsePagination<IEnumerable<CatalogItemDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(CatalogItemDTO dto);
    Task<Response<bool>> UpdateAsync(CatalogItemDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<CatalogItemDTO>> GetAsync(int id);
    Task<Response<IEnumerable<CatalogItemDTO>>> GetAllAsync();
    Task<ResponsePagination<IEnumerable<CatalogItemDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>> CountAsync();

    #endregion
}
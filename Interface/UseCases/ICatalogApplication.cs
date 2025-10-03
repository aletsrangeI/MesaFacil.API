using Common;
using DTO.Catalog;

namespace Interface.UseCases;

public interface ICatalogApplication
{
    #region Metodos sincronos

    Response<bool> Insert(CatalogDTO Catalogo);
    Response<bool> Update(CatalogDTO Catalogo);
    Response<bool> Delete(int id);
    Response<CatalogDTO> Get(int id);
    Response<IEnumerable<CatalogDTO>> GetAll();
    ResponsePagination<IEnumerable<CatalogDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int> Count();

    #endregion

    #region Metodos asincronos

    Task<Response<bool>> InsertAsync(CatalogDTO Catalogo);

    Task<Response<bool>> UpdateAsync(CatalogDTO Catalogo);

    Task<Response<bool>> DeleteAsync(int id);

    Task<Response<CatalogDTO>> GetAsync(int id);

    Task<Response<IEnumerable<CatalogDTO>>> GetAllAsync();

    Task<ResponsePagination<IEnumerable<CatalogDTO>>> GetAllWithPaginationAsync(int page, int pageSize);

    Task<Response<int>> CountAsync();

    #endregion
}
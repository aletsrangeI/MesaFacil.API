using Common;
using DTO.GenericCatalog;

namespace Interface.UseCases;

/// <summary>
/// Contrato CRUD genérico para todos los catálogos simples (Cat*).
/// Una sola implementación genérica atiende todos los catálogos.
/// </summary>
public interface IGenericCatalogApplication
{
    #region Métodos síncronos

    Response<bool>                              Insert(GenericCatalogDTO dto);
    Response<bool>                              Update(GenericCatalogDTO dto);
    Response<bool>                              Delete(int id);
    Response<GenericCatalogDTO>                 Get(int id);
    Response<IEnumerable<GenericCatalogDTO>>    GetAll();
    ResponsePagination<IEnumerable<GenericCatalogDTO>> GetAllWithPagination(int page, int pageSize);
    Response<int>                               Count();

    #endregion

    #region Métodos asíncronos

    Task<Response<bool>>                              InsertAsync(GenericCatalogDTO dto);
    Task<Response<bool>>                              UpdateAsync(GenericCatalogDTO dto);
    Task<Response<bool>>                              DeleteAsync(int id);
    Task<Response<GenericCatalogDTO>>                 GetAsync(int id);
    Task<Response<IEnumerable<GenericCatalogDTO>>>    GetAllAsync();
    Task<ResponsePagination<IEnumerable<GenericCatalogDTO>>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<Response<int>>                               CountAsync();

    #endregion
}

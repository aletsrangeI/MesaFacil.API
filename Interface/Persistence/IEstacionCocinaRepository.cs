using Domain.Entities;

namespace Interface.Persistence;

public interface IEstacionCocinaRepository
{
    #region Metodos sincronos
    bool Insert(EstacionCocina entity);
    bool Update(EstacionCocina entity);
    bool Delete(int id);
    EstacionCocina Get(int id);
    IEnumerable<EstacionCocina> GetAll();
    IEnumerable<EstacionCocina> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(EstacionCocina entity);
    Task<bool> UpdateAsync(EstacionCocina entity);
    Task<bool> DeleteAsync(int id);
    Task<EstacionCocina> GetAsync(int id);
    Task<IEnumerable<EstacionCocina>> GetAllAsync();
    Task<IEnumerable<EstacionCocina>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}
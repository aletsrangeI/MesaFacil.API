using Domain.Entities;

namespace Interface.Persistence;

public interface ICatCredencialRepository
{
    bool Insert(CatCredencial entity);
    bool Update(CatCredencial entity);
    bool Delete(int id);
    CatCredencial Get(int id);
    IEnumerable<CatCredencial> GetAll();
    IEnumerable<CatCredencial> GetAllWithPagination(int page, int pageSize);
    int Count();

    Task<bool> InsertAsync(CatCredencial entity);
    Task<bool> UpdateAsync(CatCredencial entity);
    Task<bool> DeleteAsync(int id);
    Task<CatCredencial> GetAsync(int id);
    Task<IEnumerable<CatCredencial>> GetAllAsync();
    Task<IEnumerable<CatCredencial>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
}
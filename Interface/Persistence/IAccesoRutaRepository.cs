using Domain.Entities;

namespace Interface.Persistence;

public interface IAccesoRutaRepository
{
    #region Metodos sincronos
    bool Insert(AccesoRuta entity);
    bool Update(AccesoRuta entity);
    bool Delete(int id);
    AccesoRuta Get(int id);
    IEnumerable<AccesoRuta> GetAll();
    IEnumerable<AccesoRuta> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(AccesoRuta entity);
    Task<bool> UpdateAsync(AccesoRuta entity);
    Task<bool> DeleteAsync(int id);
    Task<AccesoRuta> GetAsync(int id);
    Task<IEnumerable<AccesoRuta>> GetAllAsync();
    Task<IEnumerable<AccesoRuta>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}
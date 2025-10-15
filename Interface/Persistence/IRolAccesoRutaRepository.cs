using Domain.Entities;

namespace Interface.Persistence;

public interface IRolAccesoRutaRepository
{
    #region Metodos sincronos
    bool Insert(RolAccesoRuta entity);
    bool Update(RolAccesoRuta entity);
    bool Delete(int id);
    RolAccesoRuta Get(int id);
    IEnumerable<RolAccesoRuta> GetAll();
    IEnumerable<RolAccesoRuta> GetAllWithPagination(int page, int pageSize);
    int Count();
    #endregion

    #region Metodos asincronos
    Task<bool> InsertAsync(RolAccesoRuta entity);
    Task<bool> UpdateAsync(RolAccesoRuta entity);
    Task<bool> DeleteAsync(int id);
    Task<RolAccesoRuta> GetAsync(int id);
    Task<IEnumerable<RolAccesoRuta>> GetAllAsync();
    Task<IEnumerable<RolAccesoRuta>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();
    #endregion
}
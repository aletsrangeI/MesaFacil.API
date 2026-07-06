using Domain.Entities;

namespace Interface.Persistence;

public interface IFormularioRepository
{
    // Métodos estándar
    bool Insert(Formulario entity);
    bool Update(Formulario entity);
    bool Delete(int id);
    Formulario Get(int id);
    IEnumerable<Formulario> GetAll();
    IEnumerable<Formulario> GetAllWithPagination(int page, int pageSize);
    int Count();

    // [NUEVO] Obtener estructura completa por código
    Formulario? GetByCode(string code, bool includeFields = true);

    // Métodos asíncronos
    Task<bool> InsertAsync(Formulario entity);
    Task<bool> UpdateAsync(Formulario entity);
    Task<bool> DeleteAsync(int id);
    Task<Formulario> GetAsync(int id);
    Task<IEnumerable<Formulario>> GetAllAsync();
    Task<IEnumerable<Formulario>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();

    // [NUEVO] Obtener estructura completa por código (Async)
    Task<Formulario?> GetByCodeAsync(string code, bool includeFields = true);
}
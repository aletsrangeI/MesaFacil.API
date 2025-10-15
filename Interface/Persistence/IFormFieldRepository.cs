using Domain.Entities;

namespace Interface.Persistence;

public interface IFormFieldRepository
{
    #region Metodos sincronos

    bool Insert(FormField entity);
    bool Update(FormField entity);
    bool Delete(int id);
    FormField Get(int id);
    IEnumerable<FormField> GetAll();
    IEnumerable<FormField> GetAllWithPagination(int page, int pageSize);
    int Count();

    public IEnumerable<FormField> GetFormFieldByFormCatId(int id);
    public Task<IEnumerable<FormField>> GetFormFieldByFormCatIdAsync(int id);

    #endregion

    #region Metodos asincronos

    Task<bool> InsertAsync(FormField entity);
    Task<bool> UpdateAsync(FormField entity);
    Task<bool> DeleteAsync(int id);
    Task<FormField> GetAsync(int id);
    Task<IEnumerable<FormField>> GetAllAsync();
    Task<IEnumerable<FormField>> GetAllWithPaginationAsync(int page, int pageSize);
    Task<int> CountAsync();

    #endregion
}
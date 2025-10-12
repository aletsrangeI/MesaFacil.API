using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class FormFieldRepository : IFormFieldRepository
{
    protected readonly ApplicationDbContext _context;

    public FormFieldRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(FormField entity)
    {
        _context.FormFields.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(FormField entity)
    {
        _context.FormFields.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.FormFields.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public FormField Get(int id)
    {
        return _context.FormFields.Find(id);
    }

    public IEnumerable<FormField> GetAll()
    {
        return _context.FormFields;
    }

    public IEnumerable<FormField> GetAllWithPagination(int page, int pageSize)
    {
        return _context.FormFields
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.FormFields.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(FormField entity)
    {
        await _context.FormFields.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(FormField entity)
    {
        _context.FormFields.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.FormFields.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<FormField> GetAsync(int id)
    {
        return await _context.FormFields.FindAsync(id);
    }

    public async Task<IEnumerable<FormField>> GetAllAsync()
    {
        return await _context.FormFields.ToListAsync();
    }

    public async Task<IEnumerable<FormField>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.FormFields
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.FormFields.CountAsync();
    }

    #endregion
}
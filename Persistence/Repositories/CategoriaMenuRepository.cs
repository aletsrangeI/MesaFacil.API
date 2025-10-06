using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CategoriaMenuRepository : ICategoriaMenuRepository
{
    protected readonly ApplicationDbContext _context;

    public CategoriaMenuRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(CategoriaMenu entity)
    {
        _context.CategoriaMenus.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(CategoriaMenu entity)
    {
        _context.CategoriaMenus.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.CategoriaMenus.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public CategoriaMenu Get(int id)
    {
        return _context.CategoriaMenus.Find(id);
    }

    public IEnumerable<CategoriaMenu> GetAll()
    {
        return _context.CategoriaMenus;
    }

    public IEnumerable<CategoriaMenu> GetAllWithPagination(int page, int pageSize)
    {
        return _context.CategoriaMenus
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.CategoriaMenus.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(CategoriaMenu entity)
    {
        await _context.CategoriaMenus.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(CategoriaMenu entity)
    {
        _context.CategoriaMenus.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.CategoriaMenus.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<CategoriaMenu> GetAsync(int id)
    {
        return await _context.CategoriaMenus.FindAsync(id);
    }

    public async Task<IEnumerable<CategoriaMenu>> GetAllAsync()
    {
        return await _context.CategoriaMenus.ToListAsync();
    }

    public async Task<IEnumerable<CategoriaMenu>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.CategoriaMenus
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.CategoriaMenus.CountAsync();
    }

    #endregion
}
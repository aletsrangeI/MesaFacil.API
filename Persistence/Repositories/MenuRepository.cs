using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class MenuRepository : IMenuRepository
{
    protected readonly ApplicationDbContext _context;

    public MenuRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Menu entity)
    {
        _context.Menus.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Menu entity)
    {
        _context.Menus.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Menus.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Menu Get(int id)
    {
        return _context.Menus.Find(id);
    }

    public IEnumerable<Menu> GetAll()
    {
        return _context.Menus;
    }

    public IEnumerable<Menu> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Menus
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Menus.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Menu entity)
    {
        await _context.Menus.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Menu entity)
    {
        _context.Menus.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Menus.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Menu> GetAsync(int id)
    {
        return await _context.Menus.FindAsync(id);
    }

    public async Task<IEnumerable<Menu>> GetAllAsync()
    {
        return await _context.Menus.ToListAsync();
    }

    public async Task<IEnumerable<Menu>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Menus
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Menus.CountAsync();
    }

    #endregion
}
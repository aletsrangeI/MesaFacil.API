using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CatalogItemRepository : ICatalogItemRepository
{
    protected readonly ApplicationDbContext _context;

    public CatalogItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(CatalogItem entity)
    {
        _context.CatalogItems.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(CatalogItem entity)
    {
        _context.CatalogItems.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.CatalogItems.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public CatalogItem Get(int id)
    {
        return _context.CatalogItems.Find(id);
    }

    public IEnumerable<CatalogItem> GetAll()
    {
        return _context.CatalogItems;
    }

    public IEnumerable<CatalogItem> GetAllWithPagination(int page, int pageSize)
    {
        return _context.CatalogItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.CatalogItems.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(CatalogItem entity)
    {
        await _context.CatalogItems.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(CatalogItem entity)
    {
        _context.CatalogItems.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.CatalogItems.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<CatalogItem> GetAsync(int id)
    {
        return await _context.CatalogItems.FindAsync(id);
    }

    public async Task<IEnumerable<CatalogItem>> GetAllAsync()
    {
        return await _context.CatalogItems.ToListAsync();
    }

    public async Task<IEnumerable<CatalogItem>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.CatalogItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.CatalogItems.CountAsync();
    }

    #endregion
}
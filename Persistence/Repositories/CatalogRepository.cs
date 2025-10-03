using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CatalogRepository : ICatalogRepository
{
    protected readonly ApplicationDbContext _context;

    public CatalogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Catalog entity)
    {
        _context.Catalogs.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Catalog entity)
    {
        _context.Catalogs.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        Catalog Catalog = Get(id);
        if (Catalog == null) return false;
        _context.Catalogs.Remove(Catalog);
        return _context.SaveChanges() > 0;
    }

    public Catalog Get(int id)
    {
        return _context.Catalogs.Find(id);
    }

    public IEnumerable<Catalog> GetAll()
    {
        return _context.Catalogs;
    }

    public IEnumerable<Catalog> GetAllWithPagination(int page, int pageSize)
    {
        IEnumerable<Catalog> Catalogs =
            _context.Catalogs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Catalogs;
    }

    public int Count()
    {
        return _context.Catalogs.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Catalog entity)
    {
        await _context.Catalogs.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Catalog entity)
    {
        _context.Catalogs.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Catalog Catalog = await GetAsync(id);
        if (Catalog == null) return false;
        _context.Catalogs.Remove(Catalog);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Catalog> GetAsync(int id)
    {
        return await _context.Catalogs.FindAsync(id);
    }

    public async Task<IEnumerable<Catalog>> GetAllAsync()
    {
        return await _context.Catalogs.ToListAsync();
    }

    public async Task<IEnumerable<Catalog>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        IEnumerable<Catalog> Catalogs =
            await _context.Catalogs.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Catalogs;
    }

    public async Task<int> CountAsync()
    {
        return await _context.Catalogs.CountAsync();
    }

    #endregion
}
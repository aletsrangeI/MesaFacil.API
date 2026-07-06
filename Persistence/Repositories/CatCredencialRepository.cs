using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CatCredencialRepository : ICatCredencialRepository
{
    protected readonly ApplicationDbContext _context;

    public CatCredencialRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(CatCredencial entity)
    {
        _context.CatCredenciales.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(CatCredencial entity)
    {
        _context.CatCredenciales.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        
        _context.CatCredenciales.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public CatCredencial Get(int id)
    {
        return _context.CatCredenciales.Find(id);
    }

    public IEnumerable<CatCredencial> GetAll()
    {
        return _context.CatCredenciales.ToList();
    }

    public IEnumerable<CatCredencial> GetAllWithPagination(int page, int pageSize)
    {
        return _context.CatCredenciales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.CatCredenciales.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(CatCredencial entity)
    {
        await _context.CatCredenciales.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(CatCredencial entity)
    {
        _context.CatCredenciales.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        
        _context.CatCredenciales.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<CatCredencial> GetAsync(int id)
    {
        return await _context.CatCredenciales.FindAsync(id);
    }

    public async Task<IEnumerable<CatCredencial>> GetAllAsync()
    {
        return await _context.CatCredenciales.ToListAsync();
    }

    public async Task<IEnumerable<CatCredencial>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.CatCredenciales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.CatCredenciales.CountAsync();
    }

    #endregion
}
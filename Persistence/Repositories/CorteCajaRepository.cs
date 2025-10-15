using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CorteCajaRepository : ICorteCajaRepository
{
    protected readonly ApplicationDbContext _context;

    public CorteCajaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(CorteCaja entity)
    {
        _context.CorteCajas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(CorteCaja entity)
    {
        _context.CorteCajas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.CorteCajas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public CorteCaja Get(int id)
    {
        return _context.CorteCajas.Find(id);
    }

    public IEnumerable<CorteCaja> GetAll()
    {
        return _context.CorteCajas;
    }

    public IEnumerable<CorteCaja> GetAllWithPagination(int page, int pageSize)
    {
        return _context.CorteCajas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.CorteCajas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(CorteCaja entity)
    {
        await _context.CorteCajas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(CorteCaja entity)
    {
        _context.CorteCajas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.CorteCajas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<CorteCaja> GetAsync(int id)
    {
        return await _context.CorteCajas.FindAsync(id);
    }

    public async Task<IEnumerable<CorteCaja>> GetAllAsync()
    {
        return await _context.CorteCajas.ToListAsync();
    }

    public async Task<IEnumerable<CorteCaja>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.CorteCajas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.CorteCajas.CountAsync();
    }

    #endregion
}
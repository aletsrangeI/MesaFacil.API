using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class MesaRepository : IMesaRepository
{
    protected readonly ApplicationDbContext _context;

    public MesaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Mesa entity)
    {
        _context.Mesas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Mesa entity)
    {
        _context.Mesas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Mesas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Mesa Get(int id)
    {
        return _context.Mesas.Find(id);
    }

    public IEnumerable<Mesa> GetAll()
    {
        return _context.Mesas;
    }

    public IEnumerable<Mesa> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Mesas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Mesas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Mesa entity)
    {
        await _context.Mesas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Mesa entity)
    {
        _context.Mesas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Mesas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Mesa> GetAsync(int id)
    {
        return await _context.Mesas.FindAsync(id);
    }

    public async Task<IEnumerable<Mesa>> GetAllAsync()
    {
        return await _context.Mesas.ToListAsync();
    }

    public async Task<IEnumerable<Mesa>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Mesas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Mesas.CountAsync();
    }

    #endregion
}
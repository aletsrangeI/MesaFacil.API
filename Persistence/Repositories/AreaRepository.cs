using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class AreaRepository : IAreaRepository
{
    protected readonly ApplicationDbContext _context;

    public AreaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Area entity)
    {
        _context.Areas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Area entity)
    {
        _context.Areas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Areas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Area Get(int id)
    {
        return _context.Areas.Find(id);
    }

    public IEnumerable<Area> GetAll()
    {
        return _context.Areas;
    }

    public IEnumerable<Area> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Areas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Areas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Area entity)
    {
        await _context.Areas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Area entity)
    {
        _context.Areas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Areas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Area> GetAsync(int id)
    {
        return await _context.Areas.FindAsync(id);
    }

    public async Task<IEnumerable<Area>> GetAllAsync()
    {
        return await _context.Areas.ToListAsync();
    }

    public async Task<IEnumerable<Area>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Areas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Areas.CountAsync();
    }

    #endregion
}
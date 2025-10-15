using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class EstacionCocinaRepository : IEstacionCocinaRepository
{
    protected readonly ApplicationDbContext _context;

    public EstacionCocinaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(EstacionCocina entity)
    {
        _context.EstacionesCocina.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(EstacionCocina entity)
    {
        _context.EstacionesCocina.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.EstacionesCocina.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public EstacionCocina Get(int id)
    {
        return _context.EstacionesCocina.Find(id);
    }

    public IEnumerable<EstacionCocina> GetAll()
    {
        return _context.EstacionesCocina;
    }

    public IEnumerable<EstacionCocina> GetAllWithPagination(int page, int pageSize)
    {
        return _context.EstacionesCocina
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.EstacionesCocina.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(EstacionCocina entity)
    {
        await _context.EstacionesCocina.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(EstacionCocina entity)
    {
        _context.EstacionesCocina.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.EstacionesCocina.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<EstacionCocina> GetAsync(int id)
    {
        return await _context.EstacionesCocina.FindAsync(id);
    }

    public async Task<IEnumerable<EstacionCocina>> GetAllAsync()
    {
        return await _context.EstacionesCocina.ToListAsync();
    }

    public async Task<IEnumerable<EstacionCocina>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.EstacionesCocina
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.EstacionesCocina.CountAsync();
    }

    #endregion
}
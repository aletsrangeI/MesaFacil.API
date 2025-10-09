using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class GrupoModificadorRepository : IGrupoModificadorRepository
{
    protected readonly ApplicationDbContext _context;

    public GrupoModificadorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(GrupoModificador entity)
    {
        _context.GruposModificador.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(GrupoModificador entity)
    {
        _context.GruposModificador.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.GruposModificador.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public GrupoModificador Get(int id)
    {
        return _context.GruposModificador.Find(id);
    }

    public IEnumerable<GrupoModificador> GetAll()
    {
        return _context.GruposModificador;
    }

    public IEnumerable<GrupoModificador> GetAllWithPagination(int page, int pageSize)
    {
        return _context.GruposModificador
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.GruposModificador.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(GrupoModificador entity)
    {
        await _context.GruposModificador.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(GrupoModificador entity)
    {
        _context.GruposModificador.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.GruposModificador.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<GrupoModificador> GetAsync(int id)
    {
        return await _context.GruposModificador.FindAsync(id);
    }

    public async Task<IEnumerable<GrupoModificador>> GetAllAsync()
    {
        return await _context.GruposModificador.ToListAsync();
    }

    public async Task<IEnumerable<GrupoModificador>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.GruposModificador
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.GruposModificador.CountAsync();
    }

    #endregion
}
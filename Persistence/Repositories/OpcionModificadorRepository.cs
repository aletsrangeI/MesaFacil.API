using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class OpcionModificadorRepository : IOpcionModificadorRepository
{
    protected readonly ApplicationDbContext _context;

    public OpcionModificadorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(OpcionModificador entity)
    {
        _context.OpcionesModificador.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(OpcionModificador entity)
    {
        _context.OpcionesModificador.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.OpcionesModificador.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public OpcionModificador Get(int id)
    {
        return _context.OpcionesModificador.Find(id);
    }

    public IEnumerable<OpcionModificador> GetAll()
    {
        return _context.OpcionesModificador;
    }

    public IEnumerable<OpcionModificador> GetAllWithPagination(int page, int pageSize)
    {
        return _context.OpcionesModificador
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.OpcionesModificador.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(OpcionModificador entity)
    {
        await _context.OpcionesModificador.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(OpcionModificador entity)
    {
        _context.OpcionesModificador.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.OpcionesModificador.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<OpcionModificador> GetAsync(int id)
    {
        return await _context.OpcionesModificador.FindAsync(id);
    }

    public async Task<IEnumerable<OpcionModificador>> GetAllAsync()
    {
        return await _context.OpcionesModificador.ToListAsync();
    }

    public async Task<IEnumerable<OpcionModificador>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.OpcionesModificador
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.OpcionesModificador.CountAsync();
    }

    #endregion
}
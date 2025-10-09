using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class TurnoRepository : ITurnoRepository
{
    protected readonly ApplicationDbContext _context;

    public TurnoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Turno entity)
    {
        _context.Turnos.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Turno entity)
    {
        _context.Turnos.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Turnos.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Turno Get(int id)
    {
        return _context.Turnos.Find(id);
    }

    public IEnumerable<Turno> GetAll()
    {
        return _context.Turnos;
    }

    public IEnumerable<Turno> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Turnos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Turnos.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Turno entity)
    {
        await _context.Turnos.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Turno entity)
    {
        _context.Turnos.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Turnos.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Turno> GetAsync(int id)
    {
        return await _context.Turnos.FindAsync(id);
    }

    public async Task<IEnumerable<Turno>> GetAllAsync()
    {
        return await _context.Turnos.ToListAsync();
    }

    public async Task<IEnumerable<Turno>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Turnos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Turnos.CountAsync();
    }

    #endregion
}
using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class MovimientoCajaRepository : IMovimientoCajaRepository
{
    protected readonly ApplicationDbContext _context;

    public MovimientoCajaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(MovimientoCaja entity)
    {
        _context.MovimientosCaja.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(MovimientoCaja entity)
    {
        _context.MovimientosCaja.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.MovimientosCaja.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public MovimientoCaja Get(int id)
    {
        return _context.MovimientosCaja.Find(id);
    }

    public IEnumerable<MovimientoCaja> GetAll()
    {
        return _context.MovimientosCaja;
    }

    public IEnumerable<MovimientoCaja> GetAllWithPagination(int page, int pageSize)
    {
        return _context.MovimientosCaja
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.MovimientosCaja.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(MovimientoCaja entity)
    {
        await _context.MovimientosCaja.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(MovimientoCaja entity)
    {
        _context.MovimientosCaja.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.MovimientosCaja.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<MovimientoCaja> GetAsync(int id)
    {
        return await _context.MovimientosCaja.FindAsync(id);
    }

    public async Task<IEnumerable<MovimientoCaja>> GetAllAsync()
    {
        return await _context.MovimientosCaja.ToListAsync();
    }

    public async Task<IEnumerable<MovimientoCaja>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.MovimientosCaja
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.MovimientosCaja.CountAsync();
    }

    #endregion
}
using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class DetalleCuentaRepository : IDetalleCuentaRepository
{
    protected readonly ApplicationDbContext _context;

    public DetalleCuentaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(DetalleCuenta entity)
    {
        _context.DetalleCuentas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(DetalleCuenta entity)
    {
        _context.DetalleCuentas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.DetalleCuentas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public DetalleCuenta Get(int id)
    {
        return _context.DetalleCuentas.Find(id);
    }

    public IEnumerable<DetalleCuenta> GetAll()
    {
        return _context.DetalleCuentas;
    }

    public IEnumerable<DetalleCuenta> GetAllWithPagination(int page, int pageSize)
    {
        return _context.DetalleCuentas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.DetalleCuentas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(DetalleCuenta entity)
    {
        await _context.DetalleCuentas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(DetalleCuenta entity)
    {
        _context.DetalleCuentas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.DetalleCuentas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<DetalleCuenta> GetAsync(int id)
    {
        return await _context.DetalleCuentas.FindAsync(id);
    }

    public async Task<IEnumerable<DetalleCuenta>> GetAllAsync()
    {
        return await _context.DetalleCuentas.ToListAsync();
    }

    public async Task<IEnumerable<DetalleCuenta>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.DetalleCuentas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.DetalleCuentas.CountAsync();
    }

    #endregion
}
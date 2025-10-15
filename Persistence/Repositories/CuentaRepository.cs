using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CuentaRepository : ICuentaRepository
{
    protected readonly ApplicationDbContext _context;

    public CuentaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Cuenta entity)
    {
        _context.Cuentas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Cuenta entity)
    {
        _context.Cuentas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Cuentas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Cuenta Get(int id)
    {
        return _context.Cuentas.Find(id);
    }

    public IEnumerable<Cuenta> GetAll()
    {
        return _context.Cuentas;
    }

    public IEnumerable<Cuenta> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Cuentas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Cuentas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Cuenta entity)
    {
        await _context.Cuentas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Cuenta entity)
    {
        _context.Cuentas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Cuentas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Cuenta> GetAsync(int id)
    {
        return await _context.Cuentas.FindAsync(id);
    }

    public async Task<IEnumerable<Cuenta>> GetAllAsync()
    {
        return await _context.Cuentas.ToListAsync();
    }

    public async Task<IEnumerable<Cuenta>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Cuentas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Cuentas.CountAsync();
    }

    #endregion
}
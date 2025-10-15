using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class SucursalRepository : ISucursalRepository
{
    protected readonly ApplicationDbContext _context;

    public SucursalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Sucursal entity)
    {
        _context.Sucursales.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Sucursal entity)
    {
        _context.Sucursales.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Sucursales.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Sucursal Get(int id)
    {
        return _context.Sucursales.Find(id);
    }

    public IEnumerable<Sucursal> GetAll()
    {
        return _context.Sucursales;
    }

    public IEnumerable<Sucursal> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Sucursales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Sucursales.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Sucursal entity)
    {
        await _context.Sucursales.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Sucursal entity)
    {
        _context.Sucursales.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Sucursales.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Sucursal> GetAsync(int id)
    {
        return await _context.Sucursales.FindAsync(id);
    }

    public async Task<IEnumerable<Sucursal>> GetAllAsync()
    {
        return await _context.Sucursales.ToListAsync();
    }

    public async Task<IEnumerable<Sucursal>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Sucursales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Sucursales.CountAsync();
    }

    #endregion
}
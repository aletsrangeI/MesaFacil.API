using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PrecioRepository : IPrecioRepository
{
    protected readonly ApplicationDbContext _context;

    public PrecioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Precio entity)
    {
        _context.Precios.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Precio entity)
    {
        _context.Precios.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Precios.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Precio Get(int id)
    {
        return _context.Precios.Find(id);
    }

    public IEnumerable<Precio> GetAll()
    {
        return _context.Precios;
    }

    public IEnumerable<Precio> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Precios
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Precios.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Precio entity)
    {
        await _context.Precios.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Precio entity)
    {
        _context.Precios.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Precios.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Precio> GetAsync(int id)
    {
        return await _context.Precios.FindAsync(id);
    }

    public async Task<IEnumerable<Precio>> GetAllAsync()
    {
        return await _context.Precios.ToListAsync();
    }

    public async Task<IEnumerable<Precio>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Precios
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Precios.CountAsync();
    }

    #endregion
}
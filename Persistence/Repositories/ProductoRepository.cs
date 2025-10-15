using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class ProductoRepository : IProductoRepository
{
    protected readonly ApplicationDbContext _context;

    public ProductoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Producto entity)
    {
        _context.Productos.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Producto entity)
    {
        _context.Productos.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Productos.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Producto Get(int id)
    {
        return _context.Productos.Find(id);
    }

    public IEnumerable<Producto> GetAll()
    {
        return _context.Productos;
    }

    public IEnumerable<Producto> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Productos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Productos.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Producto entity)
    {
        await _context.Productos.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Producto entity)
    {
        _context.Productos.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Productos.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Producto> GetAsync(int id)
    {
        return await _context.Productos.FindAsync(id);
    }

    public async Task<IEnumerable<Producto>> GetAllAsync()
    {
        return await _context.Productos.ToListAsync();
    }

    public async Task<IEnumerable<Producto>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Productos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Productos.CountAsync();
    }

    #endregion
}
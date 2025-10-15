using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class VarianteProductoRepository : IVarianteProductoRepository
{
    protected readonly ApplicationDbContext _context;

    public VarianteProductoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(VarianteProducto entity)
    {
        _context.VarianteProductos.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(VarianteProducto entity)
    {
        _context.VarianteProductos.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.VarianteProductos.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public VarianteProducto Get(int id)
    {
        return _context.VarianteProductos.Find(id);
    }

    public IEnumerable<VarianteProducto> GetAll()
    {
        return _context.VarianteProductos;
    }

    public IEnumerable<VarianteProducto> GetAllWithPagination(int page, int pageSize)
    {
        return _context.VarianteProductos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.VarianteProductos.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(VarianteProducto entity)
    {
        await _context.VarianteProductos.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(VarianteProducto entity)
    {
        _context.VarianteProductos.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.VarianteProductos.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<VarianteProducto> GetAsync(int id)
    {
        return await _context.VarianteProductos.FindAsync(id);
    }

    public async Task<IEnumerable<VarianteProducto>> GetAllAsync()
    {
        return await _context.VarianteProductos.ToListAsync();
    }

    public async Task<IEnumerable<VarianteProducto>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.VarianteProductos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.VarianteProductos.CountAsync();
    }

    #endregion
}
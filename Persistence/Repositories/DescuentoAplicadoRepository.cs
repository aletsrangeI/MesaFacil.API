using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class DescuentoAplicadoRepository : IDescuentoAplicadoRepository
{
    protected readonly ApplicationDbContext _context;

    public DescuentoAplicadoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(DescuentoAplicado entity)
    {
        _context.DescuentosAplicados.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(DescuentoAplicado entity)
    {
        _context.DescuentosAplicados.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.DescuentosAplicados.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public DescuentoAplicado Get(int id)
    {
        return _context.DescuentosAplicados.Find(id);
    }

    public IEnumerable<DescuentoAplicado> GetAll()
    {
        return _context.DescuentosAplicados;
    }

    public IEnumerable<DescuentoAplicado> GetAllWithPagination(int page, int pageSize)
    {
        return _context.DescuentosAplicados
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.DescuentosAplicados.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(DescuentoAplicado entity)
    {
        await _context.DescuentosAplicados.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(DescuentoAplicado entity)
    {
        _context.DescuentosAplicados.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.DescuentosAplicados.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<DescuentoAplicado> GetAsync(int id)
    {
        return await _context.DescuentosAplicados.FindAsync(id);
    }

    public async Task<IEnumerable<DescuentoAplicado>> GetAllAsync()
    {
        return await _context.DescuentosAplicados.ToListAsync();
    }

    public async Task<IEnumerable<DescuentoAplicado>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.DescuentosAplicados
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.DescuentosAplicados.CountAsync();
    }

    #endregion
}
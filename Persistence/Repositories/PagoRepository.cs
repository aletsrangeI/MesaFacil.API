using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PagoRepository : IPagoRepository
{
    protected readonly ApplicationDbContext _context;

    public PagoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Pago entity)
    {
        _context.Pagos.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Pago entity)
    {
        _context.Pagos.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Pagos.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Pago Get(int id)
    {
        return _context.Pagos.Find(id);
    }

    public IEnumerable<Pago> GetAll()
    {
        return _context.Pagos;
    }

    public IEnumerable<Pago> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Pagos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Pagos.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Pago entity)
    {
        await _context.Pagos.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Pago entity)
    {
        _context.Pagos.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Pagos.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Pago> GetAsync(int id)
    {
        return await _context.Pagos.FindAsync(id);
    }

    public async Task<IEnumerable<Pago>> GetAllAsync()
    {
        return await _context.Pagos.ToListAsync();
    }

    public async Task<IEnumerable<Pago>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Pagos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Pagos.CountAsync();
    }

    #endregion
}
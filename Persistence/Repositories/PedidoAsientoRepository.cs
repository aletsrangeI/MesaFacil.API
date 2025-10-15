using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PedidoAsientoRepository : IPedidoAsientoRepository
{
    protected readonly ApplicationDbContext _context;

    public PedidoAsientoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(PedidoAsiento entity)
    {
        _context.PedidosAsiento.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(PedidoAsiento entity)
    {
        _context.PedidosAsiento.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.PedidosAsiento.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public PedidoAsiento Get(int id)
    {
        return _context.PedidosAsiento.Find(id);
    }

    public IEnumerable<PedidoAsiento> GetAll()
    {
        return _context.PedidosAsiento;
    }

    public IEnumerable<PedidoAsiento> GetAllWithPagination(int page, int pageSize)
    {
        return _context.PedidosAsiento
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.PedidosAsiento.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(PedidoAsiento entity)
    {
        await _context.PedidosAsiento.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(PedidoAsiento entity)
    {
        _context.PedidosAsiento.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.PedidosAsiento.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<PedidoAsiento> GetAsync(int id)
    {
        return await _context.PedidosAsiento.FindAsync(id);
    }

    public async Task<IEnumerable<PedidoAsiento>> GetAllAsync()
    {
        return await _context.PedidosAsiento.ToListAsync();
    }

    public async Task<IEnumerable<PedidoAsiento>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.PedidosAsiento
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.PedidosAsiento.CountAsync();
    }

    #endregion
}
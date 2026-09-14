using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PedidoDetalleRepository : IPedidoDetalleRepository
{
    protected readonly ApplicationDbContext _context;

    public PedidoDetalleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(PedidoDetalle entity)
    {
        _context.PedidoDetalles.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(PedidoDetalle entity)
    {
        _context.PedidoDetalles.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(Guid id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.PedidoDetalles.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public PedidoDetalle Get(Guid id)
    {
        return _context.PedidoDetalles.Find(id);
    }

    public IEnumerable<PedidoDetalle> GetAll()
    {
        return _context.PedidoDetalles;
    }

    public IEnumerable<PedidoDetalle> GetAllWithPagination(int page, int pageSize)
    {
        return _context.PedidoDetalles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.PedidoDetalles.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(PedidoDetalle entity)
    {
        await _context.PedidoDetalles.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(PedidoDetalle entity)
    {
        _context.PedidoDetalles.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.PedidoDetalles.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<PedidoDetalle> GetAsync(Guid id)
    {
        return await _context.PedidoDetalles.FindAsync(id);
    }

    public async Task<IEnumerable<PedidoDetalle>> GetAllAsync()
    {
        return await _context.PedidoDetalles.ToListAsync();
    }

    public async Task<IEnumerable<PedidoDetalle>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.PedidoDetalles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.PedidoDetalles.CountAsync();
    }

    #endregion
}
using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PedidoRepository : IPedidoRepository
{
    protected readonly ApplicationDbContext _context;

    public PedidoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Pedido entity)
    {
        _context.Pedidos.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Pedido entity)
    {
        _context.Pedidos.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(Guid id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Pedidos.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Pedido Get(Guid id)
    {
        return _context.Pedidos.Find(id);
    }

    public IEnumerable<Pedido> GetAll()
    {
        return _context.Pedidos;
    }

    public IEnumerable<Pedido> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Pedidos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Pedidos.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Pedido entity)
    {
        await _context.Pedidos.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Pedido entity)
    {
        _context.Pedidos.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Pedidos.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Pedido> GetAsync(Guid id)
    {
        return await _context.Pedidos.FindAsync(id);
    }

    public async Task<IEnumerable<Pedido>> GetAllAsync()
    {
        return await _context.Pedidos.ToListAsync();
    }

    public async Task<IEnumerable<Pedido>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Pedidos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Pedidos.CountAsync();
    }

    #endregion
}
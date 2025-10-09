using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class EventoPedidoRepository : IEventoPedidoRepository
{
    protected readonly ApplicationDbContext _context;

    public EventoPedidoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(EventoPedido entity)
    {
        _context.EventosPedido.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(EventoPedido entity)
    {
        _context.EventosPedido.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.EventosPedido.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public EventoPedido Get(int id)
    {
        return _context.EventosPedido.Find(id);
    }

    public IEnumerable<EventoPedido> GetAll()
    {
        return _context.EventosPedido;
    }

    public IEnumerable<EventoPedido> GetAllWithPagination(int page, int pageSize)
    {
        return _context.EventosPedido
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.EventosPedido.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(EventoPedido entity)
    {
        await _context.EventosPedido.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(EventoPedido entity)
    {
        _context.EventosPedido.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.EventosPedido.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<EventoPedido> GetAsync(int id)
    {
        return await _context.EventosPedido.FindAsync(id);
    }

    public async Task<IEnumerable<EventoPedido>> GetAllAsync()
    {
        return await _context.EventosPedido.ToListAsync();
    }

    public async Task<IEnumerable<EventoPedido>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.EventosPedido
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.EventosPedido.CountAsync();
    }

    #endregion
}
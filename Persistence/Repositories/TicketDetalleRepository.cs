using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class TicketDetalleRepository : ITicketDetalleRepository
{
    protected readonly ApplicationDbContext _context;

    public TicketDetalleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(TicketDetalle entity)
    {
        _context.TicketDetalles.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(TicketDetalle entity)
    {
        _context.TicketDetalles.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.TicketDetalles.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public TicketDetalle Get(int id)
    {
        return _context.TicketDetalles.Find(id);
    }

    public IEnumerable<TicketDetalle> GetAll()
    {
        return _context.TicketDetalles;
    }

    public IEnumerable<TicketDetalle> GetAllWithPagination(int page, int pageSize)
    {
        return _context.TicketDetalles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.TicketDetalles.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(TicketDetalle entity)
    {
        await _context.TicketDetalles.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(TicketDetalle entity)
    {
        _context.TicketDetalles.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.TicketDetalles.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<TicketDetalle> GetAsync(int id)
    {
        return await _context.TicketDetalles.FindAsync(id);
    }

    public async Task<IEnumerable<TicketDetalle>> GetAllAsync()
    {
        return await _context.TicketDetalles.ToListAsync();
    }

    public async Task<IEnumerable<TicketDetalle>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.TicketDetalles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.TicketDetalles.CountAsync();
    }

    #endregion
}
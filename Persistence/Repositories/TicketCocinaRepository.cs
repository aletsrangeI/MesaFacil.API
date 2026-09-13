using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class TicketCocinaRepository : ITicketCocinaRepository
{
    protected readonly ApplicationDbContext _context;

    public TicketCocinaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(TicketCocina entity)
    {
        _context.TicketsCocina.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(TicketCocina entity)
    {
        _context.TicketsCocina.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(Guid id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.TicketsCocina.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public TicketCocina Get(Guid id)
    {
        return _context.TicketsCocina.Find(id);
    }

    public IEnumerable<TicketCocina> GetAll()
    {
        return _context.TicketsCocina;
    }

    public IEnumerable<TicketCocina> GetAllWithPagination(int page, int pageSize)
    {
        return _context.TicketsCocina
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.TicketsCocina.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(TicketCocina entity)
    {
        await _context.TicketsCocina.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(TicketCocina entity)
    {
        _context.TicketsCocina.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.TicketsCocina.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<TicketCocina> GetAsync(Guid id)
    {
        return await _context.TicketsCocina.FindAsync(id);
    }

    public async Task<IEnumerable<TicketCocina>> GetAllAsync()
    {
        return await _context.TicketsCocina.ToListAsync();
    }

    public async Task<IEnumerable<TicketCocina>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.TicketsCocina
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.TicketsCocina.CountAsync();
    }

    #endregion
}
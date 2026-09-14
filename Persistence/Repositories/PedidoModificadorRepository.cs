using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class PedidoModificadorRepository : IPedidoModificadorRepository
{
    protected readonly ApplicationDbContext _context;

    public PedidoModificadorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(PedidoModificador entity)
    {
        _context.PedidoModificadores.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(PedidoModificador entity)
    {
        _context.PedidoModificadores.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(Guid id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.PedidoModificadores.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public PedidoModificador Get(Guid id)
    {
        return _context.PedidoModificadores.Find(id);
    }

    public IEnumerable<PedidoModificador> GetAll()
    {
        return _context.PedidoModificadores;
    }

    public IEnumerable<PedidoModificador> GetAllWithPagination(int page, int pageSize)
    {
        return _context.PedidoModificadores
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.PedidoModificadores.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(PedidoModificador entity)
    {
        await _context.PedidoModificadores.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(PedidoModificador entity)
    {
        _context.PedidoModificadores.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.PedidoModificadores.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<PedidoModificador> GetAsync(Guid id)
    {
        return await _context.PedidoModificadores.FindAsync(id);
    }

    public async Task<IEnumerable<PedidoModificador>> GetAllAsync()
    {
        return await _context.PedidoModificadores.ToListAsync();
    }

    public async Task<IEnumerable<PedidoModificador>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.PedidoModificadores
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.PedidoModificadores.CountAsync();
    }

    #endregion
}
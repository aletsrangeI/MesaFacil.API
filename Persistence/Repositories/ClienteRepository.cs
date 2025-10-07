using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    protected readonly ApplicationDbContext _context;

    public ClienteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Cliente entity)
    {
        _context.Clientes.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Cliente entity)
    {
        _context.Clientes.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Clientes.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Cliente Get(int id)
    {
        return _context.Clientes.Find(id);
    }

    public IEnumerable<Cliente> GetAll()
    {
        return _context.Clientes;
    }

    public IEnumerable<Cliente> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Clientes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Clientes.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Cliente entity)
    {
        await _context.Clientes.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Cliente entity)
    {
        _context.Clientes.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Clientes.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Cliente> GetAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.ToListAsync();
    }

    public async Task<IEnumerable<Cliente>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Clientes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Clientes.CountAsync();
    }

    #endregion
}
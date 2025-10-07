using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class CredencialRepository : ICredencialRepository
{
    protected readonly ApplicationDbContext _context;

    public CredencialRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Credencial entity)
    {
        _context.Credenciales.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Credencial entity)
    {
        _context.Credenciales.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Credenciales.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Credencial Get(int id)
    {
        return _context.Credenciales.Find(id);
    }

    public IEnumerable<Credencial> GetAll()
    {
        return _context.Credenciales;
    }

    public IEnumerable<Credencial> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Credenciales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Credenciales.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Credencial entity)
    {
        await _context.Credenciales.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Credencial entity)
    {
        _context.Credenciales.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Credenciales.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Credencial> GetAsync(int id)
    {
        return await _context.Credenciales.FindAsync(id);
    }

    public async Task<IEnumerable<Credencial>> GetAllAsync()
    {
        return await _context.Credenciales.ToListAsync();
    }

    public async Task<IEnumerable<Credencial>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Credenciales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Credenciales.CountAsync();
    }

    #endregion
}
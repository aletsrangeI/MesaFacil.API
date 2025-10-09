using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class RolRepository : IRolRepository
{
    protected readonly ApplicationDbContext _context;

    public RolRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Rol entity)
    {
        _context.Roles.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Rol entity)
    {
        _context.Roles.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Roles.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Rol Get(int id)
    {
        return _context.Roles.Find(id);
    }

    public IEnumerable<Rol> GetAll()
    {
        return _context.Roles;
    }

    public IEnumerable<Rol> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Roles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Roles.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Rol entity)
    {
        await _context.Roles.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Rol entity)
    {
        _context.Roles.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Roles.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Rol> GetAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<IEnumerable<Rol>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<IEnumerable<Rol>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Roles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Roles.CountAsync();
    }

    #endregion
}
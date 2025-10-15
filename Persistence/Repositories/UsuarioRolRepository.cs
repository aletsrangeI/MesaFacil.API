using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class UsuarioRolRepository : IUsuarioRolRepository
{
    protected readonly ApplicationDbContext _context;

    public UsuarioRolRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(UsuarioRol entity)
    {
        _context.UsuarioRoles.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(UsuarioRol entity)
    {
        _context.UsuarioRoles.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.UsuarioRoles.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public UsuarioRol Get(int id)
    {
        return _context.UsuarioRoles.Find(id);
    }

    public IEnumerable<UsuarioRol> GetAll()
    {
        return _context.UsuarioRoles;
    }

    public IEnumerable<UsuarioRol> GetAllWithPagination(int page, int pageSize)
    {
        return _context.UsuarioRoles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.UsuarioRoles.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(UsuarioRol entity)
    {
        await _context.UsuarioRoles.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(UsuarioRol entity)
    {
        _context.UsuarioRoles.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.UsuarioRoles.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<UsuarioRol> GetAsync(int id)
    {
        return await _context.UsuarioRoles.FindAsync(id);
    }

    public async Task<IEnumerable<UsuarioRol>> GetAllAsync()
    {
        return await _context.UsuarioRoles.ToListAsync();
    }

    public async Task<IEnumerable<UsuarioRol>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.UsuarioRoles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.UsuarioRoles.CountAsync();
    }

    #endregion
}
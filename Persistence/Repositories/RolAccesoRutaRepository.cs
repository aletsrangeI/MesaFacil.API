using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class RolAccesoRutaRepository : IRolAccesoRutaRepository
{
    protected readonly ApplicationDbContext _context;

    public RolAccesoRutaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(RolAccesoRuta entity)
    {
        _context.RolAccesoRutas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(RolAccesoRuta entity)
    {
        _context.RolAccesoRutas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.RolAccesoRutas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public RolAccesoRuta Get(int id)
    {
        return _context.RolAccesoRutas.Find(id);
    }

    public IEnumerable<RolAccesoRuta> GetAll()
    {
        return _context.RolAccesoRutas;
    }

    public IEnumerable<RolAccesoRuta> GetAllWithPagination(int page, int pageSize)
    {
        return _context.RolAccesoRutas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.RolAccesoRutas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(RolAccesoRuta entity)
    {
        await _context.RolAccesoRutas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(RolAccesoRuta entity)
    {
        _context.RolAccesoRutas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.RolAccesoRutas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<RolAccesoRuta> GetAsync(int id)
    {
        return await _context.RolAccesoRutas.FindAsync(id);
    }

    public async Task<IEnumerable<RolAccesoRuta>> GetAllAsync()
    {
        return await _context.RolAccesoRutas.ToListAsync();
    }

    public async Task<IEnumerable<RolAccesoRuta>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.RolAccesoRutas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.RolAccesoRutas.CountAsync();
    }

    #endregion
}
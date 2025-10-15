using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class AccesoRutaRepository : IAccesoRutaRepository
{
    protected readonly ApplicationDbContext _context;

    public AccesoRutaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(AccesoRuta entity)
    {
        _context.AccesoRutas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(AccesoRuta entity)
    {
        _context.AccesoRutas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.AccesoRutas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public AccesoRuta Get(int id)
    {
        return _context.AccesoRutas.Find(id);
    }

    public IEnumerable<AccesoRuta> GetAll()
    {
        return _context.AccesoRutas;
    }

    public IEnumerable<AccesoRuta> GetAllWithPagination(int page, int pageSize)
    {
        return _context.AccesoRutas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.AccesoRutas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(AccesoRuta entity)
    {
        await _context.AccesoRutas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(AccesoRuta entity)
    {
        _context.AccesoRutas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.AccesoRutas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<AccesoRuta> GetAsync(int id)
    {
        return await _context.AccesoRutas.FindAsync(id);
    }

    public async Task<IEnumerable<AccesoRuta>> GetAllAsync()
    {
        return await _context.AccesoRutas.ToListAsync();
    }

    public async Task<IEnumerable<AccesoRuta>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.AccesoRutas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.AccesoRutas.CountAsync();
    }

    #endregion
}
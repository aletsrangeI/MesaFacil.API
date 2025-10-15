using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    protected readonly ApplicationDbContext _context;

    public EmpresaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Empresa entity)
    {
        _context.Empresas.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Empresa entity)
    {
        _context.Empresas.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Empresas.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Empresa Get(int id)
    {
        return _context.Empresas.Find(id);
    }

    public IEnumerable<Empresa> GetAll()
    {
        return _context.Empresas;
    }

    public IEnumerable<Empresa> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Empresas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Empresas.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Empresa entity)
    {
        await _context.Empresas.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Empresa entity)
    {
        _context.Empresas.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Empresas.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Empresa> GetAsync(int id)
    {
        return await _context.Empresas.FindAsync(id);
    }

    public async Task<IEnumerable<Empresa>> GetAllAsync()
    {
        return await _context.Empresas.ToListAsync();
    }

    public async Task<IEnumerable<Empresa>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Empresas
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Empresas.CountAsync();
    }

    #endregion
}
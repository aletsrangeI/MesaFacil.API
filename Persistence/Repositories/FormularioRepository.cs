using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class FormularioRepository : IFormularioRepository
{
    protected readonly ApplicationDbContext _context;

    public FormularioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Formulario entity)
    {
        _context.Formularios.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Formulario entity)
    {
        _context.Formularios.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        
        _context.Formularios.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Formulario Get(int id)
    {
        return _context.Formularios.Find(id);
    }

    public IEnumerable<Formulario> GetAll()
    {
        return _context.Formularios.ToList();
    }
    
    public Formulario? GetByCode(string code, bool includeFields = true)
    {
        var query = _context.Formularios.AsNoTracking();
        
        if (includeFields)
            query = query.Include(f => f.Campos.OrderBy(c => c.Orden));

        return query.FirstOrDefault(f => f.Codigo == code);
    }

    public IEnumerable<Formulario> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Formularios
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Formularios.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Formulario entity)
    {
        await _context.Formularios.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Formulario entity)
    {
        _context.Formularios.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        
        _context.Formularios.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Formulario> GetAsync(int id)
    {
        return await _context.Formularios.FindAsync(id);
    }

    public async Task<IEnumerable<Formulario>> GetAllAsync()
    {
        return await _context.Formularios.ToListAsync();
    }
    
    public async Task<Formulario?> GetByCodeAsync(string code, bool includeFields = true)
    {
        IQueryable<Formulario> query = _context.Formularios.AsNoTracking();

        if (includeFields)
            query = query.Include(f => f.Campos.Where(c => c.IsActive).OrderBy(c => c.Orden));

        return await query.FirstOrDefaultAsync(f => f.Codigo == code);
    }

    public async Task<IEnumerable<Formulario>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Formularios
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Formularios.CountAsync();
    }

    #endregion
}
using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

/// <summary>
/// Repositorio genérico que funciona con cualquier entidad que herede de
/// BaseAuditableEntity e implemente ICatalogEntity.
/// Elimina la necesidad de crear un repositorio concreto por cada catálogo simple.
/// </summary>
public class GenericCatalogRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : BaseAuditableEntity, ICatalogEntity, new()
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericCatalogRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet   = context.Set<TEntity>();
    }

    #region Métodos síncronos

    public bool Insert(TEntity entity)
    {
        _dbSet.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(TEntity entity)
    {
        _dbSet.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _dbSet.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public TEntity Get(int id) => _dbSet.Find(id);

    public IEnumerable<TEntity> GetAll() => _dbSet.ToList();

    public IEnumerable<TEntity> GetAllWithPagination(int page, int pageSize)
        => _dbSet.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    public int Count() => _dbSet.Count();

    #endregion

    #region Métodos asíncronos

    public async Task<bool> InsertAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _dbSet.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<TEntity> GetAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<TEntity>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<IEnumerable<TEntity>> GetAllWithPaginationAsync(int page, int pageSize)
        => await _dbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

    public async Task<int> CountAsync() => await _dbSet.CountAsync();

    #endregion
}

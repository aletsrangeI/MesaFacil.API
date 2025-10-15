using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    protected readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    #region Metodos sincronos

    public bool Insert(Usuario entity)
    {
        _context.Usuarios.Add(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Update(Usuario entity)
    {
        _context.Usuarios.Update(entity);
        return _context.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        var entity = Get(id);
        if (entity == null) return false;
        _context.Usuarios.Remove(entity);
        return _context.SaveChanges() > 0;
    }

    public Usuario Get(int id)
    {
        return _context.Usuarios.Find(id);
    }

    public IEnumerable<Usuario> GetAll()
    {
        return _context.Usuarios;
    }

    public IEnumerable<Usuario> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Usuarios
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int Count()
    {
        return _context.Usuarios.Count();
    }

    #endregion

    #region Metodos asincronos

    public async Task<bool> InsertAsync(Usuario entity)
    {
        await _context.Usuarios.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Usuario entity)
    {
        _context.Usuarios.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        _context.Usuarios.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Usuario> GetAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Usuarios
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Usuarios.CountAsync();
    }

    #endregion

    public async Task<Usuario?> GetByCorreoWithRolesAndCredentialsAsync(string correo, CancellationToken ct)
    {
        return await _context.Set<Usuario>()
            .Include(u => u.UsuarioRoles)
            .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
            .ThenInclude(c => c.TipoItem)
            .FirstOrDefaultAsync(u => u.Correo != null && u.Correo.ToLower() == correo.ToLower(), ct);
    }

    public async Task<bool> HasOpenTurnoAsync(int idUsuario, CancellationToken ct)
    {
        return await _context.Set<Turno>()
            .AnyAsync(t => t.IdUsuario == idUsuario && t.Cierre == null, ct);
    }
}
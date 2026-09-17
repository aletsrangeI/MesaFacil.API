using System.Security.Cryptography;
using System.Text;
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
        return _context.Usuarios
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
                .ThenInclude(c => c.CatCredencial)
            .Include(u => u.Turnos.Where(t => t.Cierre == null && t.IsActive))
            .FirstOrDefault(u => u.Id == id);
    }

    public IEnumerable<Usuario> GetAll()
    {
        return _context.Usuarios
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
                .ThenInclude(c => c.CatCredencial)
            .Include(u => u.Turnos.Where(t => t.Cierre == null && t.IsActive));
    }

    public IEnumerable<Usuario> GetAllWithPagination(int page, int pageSize)
    {
        return _context.Usuarios
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
                .ThenInclude(c => c.CatCredencial)
            .Include(u => u.Turnos.Where(t => t.Cierre == null && t.IsActive))
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
        return await _context.Usuarios
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
                .ThenInclude(c => c.CatCredencial)
            .Include(u => u.Turnos.Where(t => t.Cierre == null && t.IsActive))
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
                .ThenInclude(c => c.CatCredencial)
            .Include(u => u.Turnos.Where(t => t.Cierre == null && t.IsActive))
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        return await _context.Usuarios
            .Include(u => u.Empresa)
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
                .ThenInclude(c => c.CatCredencial)
            .Include(u => u.Turnos.Where(t => t.Cierre == null && t.IsActive))
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
        return await _context.Usuarios
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
            .ThenInclude(ur => ur.Rol)
            .Include(u => u.Credenciales)
            .ThenInclude(c => c.CatCredencial) // [CORREGIDO] Catálogo específico
            .FirstOrDefaultAsync(u => u.Correo != null && u.Correo.ToLower() == correo.ToLower(), ct);
    }

    public async Task<Usuario?> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userOrEmail)) return null;

        return await _context.Usuarios
            .Include(u => u.Sucursal)
            .Include(u => u.UsuarioRoles)
            .ThenInclude(ur => ur.Rol)
            .ThenInclude(r => r.AccesosRuta)
            .ThenInclude(rar => rar.AccesoRuta)
            .Include(u => u.Credenciales)
            .ThenInclude(c => c.CatCredencial) // [CORREGIDO]
            .FirstOrDefaultAsync(u => u.Correo.ToLower() == userOrEmail.Trim().ToLower(), ct);
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(int usuarioId, CancellationToken ct)
    {
        return await _context.UsuarioRoles
            .Where(ur => ur.UsuarioId == usuarioId && ur.IsActive)
            .Select(ur => ur.Rol.Nombre)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetAccesoPathsByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        return await _context.Set<Usuario>()
            .Where(u => u.Id == usuarioId)
            .SelectMany(u => u.UsuarioRoles.SelectMany(ur => ur.Rol.AccesosRuta.Select(ar => ar.AccesoRuta.Path)))
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<Credencial?> GetPasswordCredentialAsync(int usuarioId, CancellationToken ct)
    {
        return await GetCredentialByTypeAsync(usuarioId, "PASSWORD", ct);
    }

    public async Task<Credencial?> GetCredentialByTypeAsync(int usuarioId, string typeDescription, CancellationToken ct)
    {
        return await _context.Credenciales
            .Include(c => c.CatCredencial)
            .Where(c => c.IdUsuario == usuarioId && 
                        c.IsActive && 
                        c.CatCredencial.IsActive && 
                        c.CatCredencial.Descripcion == typeDescription)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> HasOpenTurnoAsync(int idUsuario, CancellationToken ct)
    {
        // Asumiendo que la tabla Turno tiene IdUsuario
        return await _context.Set<Turno>()
            .AnyAsync(t => t.IdUsuario == idUsuario && t.Cierre == null, ct);
    }

    public async Task<List<string>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        // Query optimizada para obtener los Keys de permiso (DASHBOARD_VIEW, etc.)
        return await (from ur in _context.UsuarioRoles
                where ur.UsuarioId == usuarioId && ur.IsActive
                join rar in _context.RolAccesoRutas on ur.IdRol equals rar.IdRol
                join ar in _context.AccesoRutas on rar.IdAccesoRuta equals ar.Id
                where rar.IsActive && ar.IsActive
                select ar.Key)
            .Distinct()
            .ToListAsync(ct);
    }

    public async Task<string?> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct)
    {
        var keys = await GetPermissionKeysByUsuarioIdAsync(usuarioId, ct);
        if (!keys.Any()) return "none";

        var canonical = string.Join("|", keys.OrderBy(k => k));
        using var sha1 = SHA1.Create();
        return Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(canonical)));
    }
}
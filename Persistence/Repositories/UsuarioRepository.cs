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
            .ThenInclude(r => r.AccesosRuta)
            .ThenInclude(rar => rar.AccesoRuta)
            .Include(u => u.Credenciales)
            .ThenInclude(c => c.TipoItem) // CatalogItem (Code/Name)
            .FirstOrDefaultAsync(u => u.Correo != null && u.Correo.ToLower() == correo.ToLower(), ct);
    }

    public async Task<Usuario?> GetByUserOrEmailWithAuthGraphAsync(string userOrEmail, CancellationToken ct)
    {
        var q = _context.Set<Usuario>()
            .Include(u => u.UsuarioRoles)
            .ThenInclude(ur => ur.Rol)
            .ThenInclude(r => r.AccesosRuta)
            .ThenInclude(rar => rar.AccesoRuta)
            .Include(u => u.Credenciales)
            .ThenInclude(c => c.TipoItem);

        userOrEmail = userOrEmail?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(userOrEmail))
            return null;

        return await q.FirstOrDefaultAsync(u =>
                u.Correo != null && u.Correo.ToLower() == userOrEmail.ToLower()
            // || u.Username != null && u.Username.ToLower() == userOrEmail.ToLower()
            , ct);
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesAsync(int usuarioId, CancellationToken ct)
    {
        // Nota: si tu mapping hace que UsuarioRol.Id == Usuario.Id, esta proyección funciona igual
        return await _context.Set<Usuario>()
            .Where(u => u.Id == usuarioId)
            .SelectMany(u => u.UsuarioRoles.Select(ur => ur.Rol.Nombre))
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
        // intento por Code
        var cred = await _context.Set<Credencial>()
            .Include(c => c.TipoItem)
            .Where(c => c.IdUsuario == usuarioId && c.TipoItem != null && c.TipoItem.Code == "PASSWORD")
            .FirstOrDefaultAsync(ct);

        if (cred != null) return cred;

        // fallback: primera credencial
        return await _context.Set<Credencial>()
            .Include(c => c.TipoItem)
            .Where(c => c.IdUsuario == usuarioId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> HasOpenTurnoAsync(int idUsuario, CancellationToken ct)
    {
        return await _context.Set<Turno>()
            .AnyAsync(t => t.IdUsuario == idUsuario && t.Cierre == null, ct);
    }

    public async Task<List<string>> GetPermissionKeysByUsuarioIdAsync(int usuarioId, CancellationToken ct)
    {
        var query =
            from ur in _context.UsuarioRoles.AsNoTracking()
            where ur.UsuarioId == usuarioId
            join rr in _context.RolAccesoRutas.AsNoTracking()
                on ur.IdRol equals rr.IdRol
            join ar in _context.AccesoRutas.AsNoTracking()
                on rr.IdAccesoRuta equals ar.Id
            select ar.Key;

        return await query.Distinct().ToListAsync(ct);
    }

    public async Task<string?> GetPermissionsVersionAsync(int usuarioId, CancellationToken ct)
    {
        // 1) Obtén las keys de permiso actuales (ya tienes este método)
        var keys = await GetPermissionKeysByUsuarioIdAsync(usuarioId, ct);

        // 2) Canonicaliza el conjunto para que el hash sea estable
        //    (mismo orden => mismo hash)
        var canonical = string.Join("\n", keys.OrderBy(k => k, StringComparer.Ordinal));

        // 3) Hashea (SHA-1/256; cualquiera funciona, es solo una etiqueta/ETag)
        using var sha1 = SHA1.Create();
        var bytes = Encoding.UTF8.GetBytes(canonical);
        var hash = sha1.ComputeHash(bytes);

        // 4) Devuelve una string corta y estable (Hex o Base64)
        return Convert.ToHexString(hash); // e.g. "A1B2C3..."
    }
}
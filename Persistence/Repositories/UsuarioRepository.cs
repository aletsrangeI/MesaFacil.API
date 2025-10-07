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
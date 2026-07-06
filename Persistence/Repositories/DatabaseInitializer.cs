using Domain.Entities;
using Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Context;

namespace Persistence;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<DatabaseInitializer> _logger;


    public DatabaseInitializer(
        ApplicationDbContext db,
        ILogger<DatabaseInitializer> logger)
    {
        _db = db;
        _logger = logger;
    }

    const string adminPlainPassword = "Admin123!";

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Iniciando inicialización de base de datos...");

        // 1) Asegurar Roles Base
        var adminRole = await EnsureRoleAsync("Admin", isSystem: true, isAssignable: true, ct);
        await EnsureRoleAsync("Manager", isSystem: false, isAssignable: true, ct);
        await EnsureRoleAsync("Mesero", isSystem: false, isAssignable: true, ct);

        // 2) Seed Operativo (Empresa, Usuario Admin, Credencial y Vínculo de Rol)
        await SeedAdminUserAsync(ct);

        // 3) RBAC: Permisos y asignación a roles
        await SeedRbacAsync(ct);

        // 4) UI: Formularios y Campos
        await SeedFormulariosAsync(ct);
        
        _logger.LogInformation("Inicialización completada con éxito.");
    }

    private async Task SeedFormulariosAsync(CancellationToken ct)
    {
        const string formLoginCode = "LOGIN";

        // 1. Formulario de Login
        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formLoginCode, ct);
        if (form is null)
        {
            form = new Formulario 
            { 
                Codigo = formLoginCode, 
                Nombre = "Formulario de Login", 
                Descripcion = "Acceso principal al sistema", 
                IsActive = true, 
                CreatedBy = "seed" 
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
        }

        // Campos del Formulario de Login
        await EnsureFormFieldAsync(form.Id, "username", "text", "Correo Usuario", "Ingresa tu correo electrónico", 1, 
            new() 
            { 
                new FormValidation { Type = "required", Value = 1 }, 
                new FormValidation { Type = "email", Value = 1 } 
            }, ct);

        await EnsureFormFieldAsync(form.Id, "password", "password", "Contraseña", "Ingresa tu contraseña", 2, 
            new() 
            { 
                new FormValidation { Type = "required", Value = 1 }, 
                new FormValidation { Type = "minLength", Value = 8 } 
            }, ct);

        // 2. Formulario CRUD de Usuario
        const string formUserCode = "USUARIO_CRUD";
        var userForm = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formUserCode, ct);
        if (userForm is null)
        {
            userForm = new Formulario 
            { 
                Codigo = formUserCode, 
                Nombre = "Formulario de Gestión de Usuarios", 
                Descripcion = "CRUD y asignación de seguridad para usuarios", 
                IsActive = true, 
                CreatedBy = "seed" 
            };
            _db.Add(userForm);
            await _db.SaveChangesAsync(ct);
        }

        // Campos del Formulario CRUD de Usuario:
        
        // 2.1 Nombre Completo
        await EnsureFormFieldAsync(userForm.Id, "nombreCompleto", "text", "Nombre Completo", "Ingresa el nombre completo", 1, 
            new() 
            { 
                new FormValidation { Type = "required", Value = 1 },
                new FormValidation { Type = "maxLength", Value = 150 }
            }, ct);

        // 2.2 Correo Electrónico
        await EnsureFormFieldAsync(userForm.Id, "correo", "text", "Correo Electrónico", "Ingresa el correo electrónico", 2, 
            new() 
            { 
                new FormValidation { Type = "required", Value = 1 }, 
                new FormValidation { Type = "email", Value = 1 },
                new FormValidation { Type = "maxLength", Value = 200 }
            }, ct);

        // 2.3 Contraseña (Para creación de credenciales iniciales)
        await EnsureFormFieldAsync(userForm.Id, "password", "password", "Contraseña", "Ingresa la contraseña (mínimo 8 caracteres)", 3, 
            new() 
            { 
                new FormValidation { Type = "minLength", Value = 8 } 
            }, ct);

        // 2.3.1 PIN (Para inicio de sesión rápido)
        await EnsureFormFieldAsync(userForm.Id, "pin", "password", "PIN de Acceso", "Ingresa un PIN numérico (4 a 8 dígitos)", 4, 
            new() 
            { 
                new FormValidation { Type = "minLength", Value = 4 },
                new FormValidation { Type = "maxLength", Value = 8 }
            }, ct);

        // 2.4 Rol de Usuario (Cargado dinámicamente)
        await EnsureFormFieldAsync(userForm.Id, "idRol", "select", "Rol de Usuario", "Selecciona el rol asignado", 5, 
            new() 
            { 
                new FormValidation { Type = "required", Value = 1 } 
            }, ct, dataSource: "roles");

        // 2.5 Empresa
        await EnsureFormFieldAsync(userForm.Id, "idEmpresa", "select", "Empresa", "Selecciona la empresa", 6, 
            new() 
            { 
                new FormValidation { Type = "required", Value = 1 } 
            }, ct, dataSource: "empresas");
    }

    private async Task EnsureFormFieldAsync(
        int idFormulario, 
        string name, 
        string type, 
        string label, 
        string placeholder, 
        int order, 
        List<FormValidation> validations, 
        CancellationToken ct,
        string? dataSource = null,
        List<SelectFormOption>? options = null)
    {
        var field = await _db.Set<FormField>().FirstOrDefaultAsync(f => f.IdFormulario == idFormulario && f.Name == name, ct);

        if (field is null)
        {
            field = new FormField
            {
                IdFormulario = idFormulario,
                Name = name,
                Type = type,
                Label = label,
                Placeholder = placeholder,
                Orden = order,
                IsActive = true,
                Validations = validations,
                DataSource = dataSource,
                Options = options ?? new(),
                CreatedBy = "seed"
            };
            _db.Add(field);
        }
        else
        {
            // Update si es necesario para mantener seed sincronizado
            field.Type = type;
            field.Label = label;
            field.Placeholder = placeholder;
            field.Orden = order;
            field.Validations = validations;
            field.DataSource = dataSource;
            field.Options = options ?? new();
            field.UpdatedAt = DateTime.UtcNow;
            _db.Update(field);
        }
        await _db.SaveChangesAsync(ct);
    }

    private static bool AreValidationsEqual(
        IList<FormValidation> a,
        IList<FormValidation> b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        if (a.Count != b.Count) return false;

        // Comparación simple por pares (orden importa);
        // si quieres que no importe el orden, ordena por (Type,Value)
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].Type != b[i].Type || a[i].Value != b[i].Value)
                return false;
        }

        return true;
    }

    // en Persistence/DatabaseInitializer.cs
    private async Task SeedAdminUserAsync(CancellationToken ct)
    {
        const string adminCorreo = "admin@mesafacil.local";
        const string empresaNombre = "Empresa Demo";

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // 1. Empresa
            var empresa = await _db.Empresas.FirstOrDefaultAsync(e => e.Nombre == empresaNombre, ct);
            if (empresa is null)
            {
                empresa = new Empresa { Nombre = empresaNombre, IsActive = true, CreatedBy = "seed" };
                _db.Add(empresa);
                await _db.SaveChangesAsync(ct);
            }

            // 2. Catálogo de Credencial (CatCredencial) - Reemplaza a CatalogItem
            var catCred = await _db.Set<CatCredencial>().FirstOrDefaultAsync(c => c.Descripcion == "PASSWORD", ct);
            if (catCred is null)
            {
                catCred = new CatCredencial { Descripcion = "PASSWORD", IsActive = true, CreatedBy = "seed" };
                _db.Add(catCred);
                await _db.SaveChangesAsync(ct);
            }

            var catCredPin = await _db.Set<CatCredencial>().FirstOrDefaultAsync(c => c.Descripcion == "PIN", ct);
            if (catCredPin is null)
            {
                catCredPin = new CatCredencial { Descripcion = "PIN", IsActive = true, CreatedBy = "seed" };
                _db.Add(catCredPin);
                await _db.SaveChangesAsync(ct);
            }

            // 3. Usuario Admin
            var admin = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == adminCorreo, ct);
            if (admin is null)
            {
                admin = new Usuario { IdEmpresa = empresa.Id, Correo = adminCorreo, NombreCompleto = "Administrador", IsActive = true, CreatedBy = "seed" };
                _db.Add(admin);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                admin.IsActive = true;
                admin.NombreCompleto = "Administrador";
                _db.Update(admin);
                await _db.SaveChangesAsync(ct);
            }

            // 4. UsuarioRol (Vincular Admin)
            var role = await _db.Roles.FirstAsync(r => r.Nombre == "Admin", ct);
            var hasRole = await _db.UsuarioRoles.AnyAsync(ur => ur.UsuarioId == admin.Id && ur.IdRol == role.Id, ct);
            if (!hasRole)
            {
                _db.Add(new UsuarioRol { UsuarioId = admin.Id, IdRol = role.Id, IsActive = true, CreatedBy = "seed" });
            }

            // 5. Credencial (Password)
            var existingCred = await _db.Credenciales.FirstOrDefaultAsync(c => c.IdUsuario == admin.Id && c.IdCredencial == catCred.Id, ct);
            var (hash, salt) = CreatePasswordHash(adminPlainPassword);
            if (existingCred is null)
            {
                _db.Add(new Credencial 
                { 
                    IdUsuario = admin.Id, 
                    IdCredencial = catCred.Id, 
                    Hash = hash, 
                    Salt = salt, 
                    IsActive = true, 
                    CreatedBy = "seed" 
                });
            }
            else
            {
                existingCred.Hash = hash;
                existingCred.Salt = salt;
                existingCred.IsActive = true;
                _db.Update(existingCred);
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Error en SeedAdminUserAsync");
            throw;
        }
    }

    private static (string Hash, string Salt) CreatePasswordHash(string password)
    {
        // PBKDF2 con SHA256, 100k iteraciones, salt 16 bytes, hash 32 bytes
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);

        var hashBytes = System.Security.Cryptography.Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: saltBytes,
            iterations: 100_000,
            hashAlgorithm: System.Security.Cryptography.HashAlgorithmName.SHA256,
            outputLength: 32
        );

        var saltBase64 = Convert.ToBase64String(saltBytes);
        var hashBase64 = Convert.ToBase64String(hashBytes);
        return (hashBase64, saltBase64);
    }

    private async Task SeedAccesosRutasAsync(CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // 1) Asegurar AccesoRuta: Dashboard (path "/")
            var dashboard = await _db.Set<AccesoRuta>()
                .FirstOrDefaultAsync(a => a.Path == "/", ct);

            if (dashboard is null)
            {
                dashboard = new AccesoRuta
                {
                    Nombre = "Dashboard",
                    Path = "/",
                    Descripcion = "Pantalla inicial post-login",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed",
                };
                _db.Add(dashboard);
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("Seed: creado AccesoRuta Dashboard ('/').");
            }
            else
            {
                bool changed = false;
                if (dashboard.Nombre != "Dashboard")
                {
                    dashboard.Nombre = "Dashboard";
                    changed = true;
                }

                if (!dashboard.IsActive)
                {
                    dashboard.IsActive = true;
                    changed = true;
                }

                if (changed)
                {
                    dashboard.UpdatedAt = DateTime.UtcNow;
                    dashboard.UpdatedBy = "seed";
                    await _db.SaveChangesAsync(ct);
                    _logger.LogInformation("Seed: actualizado AccesoRuta Dashboard ('/').");
                }
            }

            // 2) Asegurar que TODOS los roles tengan acceso al Dashboard
            var roles = await _db.Set<Rol>().ToListAsync(ct);
            foreach (var rol in roles)
            {
                var exists = await _db.Set<RolAccesoRuta>().AnyAsync(
                    r => r.IdRol == rol.Id && r.IdAccesoRuta == dashboard.Id, ct);

                if (!exists)
                {
                    var link = new RolAccesoRuta
                    {
                        IdRol = rol.Id,
                        IdAccesoRuta = dashboard.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "seed"
                    };
                    _db.Add(link);
                }
            }

            await _db.SaveChangesAsync(ct);

            // OPCIONAL: si ya quieres sembrar otros accesos base, descomenta y ajusta:
            // await EnsureAccesoAndBindAsync("Administración", "/admin", new[] { "Admin" }, ct);
            // await EnsureAccesoAndBindAsync("Vista Mesero", "/mesero", new[] { "Mesero" }, ct);

            await tx.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el seeding de Accesos/Roles");
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private async Task EnsureAccesoAndBindAsync(
        string nombre,
        string path,
        IEnumerable<string> roleNames,
        CancellationToken ct)
    {
        // Asegura acceso
        var acceso = await _db.Set<AccesoRuta>().FirstOrDefaultAsync(a => a.Path == path, ct);
        if (acceso is null)
        {
            acceso = new AccesoRuta
            {
                Nombre = nombre,
                Path = path,
                Descripcion = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            };
            _db.Add(acceso);
            await _db.SaveChangesAsync(ct);
        }

        // Vincula a roles por nombre
        var roles = await _db.Set<Rol>()
            .Where(r => roleNames.Contains(r.Nombre))
            .ToListAsync(ct);

        foreach (var rol in roles)
        {
            bool exists = await _db.Set<RolAccesoRuta>()
                .AnyAsync(x => x.IdRol == rol.Id && x.IdAccesoRuta == acceso.Id, ct);
            if (!exists)
            {
                _db.Add(new RolAccesoRuta
                {
                    IdRol = rol.Id,
                    IdAccesoRuta = acceso.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                });
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private static AccesoRuta Perm(
        string key, string nombre, string path, string group, bool isMenu = false, string? descripcion = null)
        => new()
        {
            Key = key,
            Nombre = nombre,
            Path = path, // ÚNICO en DB (índice)
            Group = group,
            IsMenu = isMenu,
            Descripcion = descripcion,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "seed"
        };

    private async Task SeedRbacAsync(CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // ------------------------------------------------------------------
            // 1) Declaración de permisos (AccesoRuta)
            // - Los que son menú (IsMenu=true) usan rutas reales /admin/*
            // - Los de acción usan pseudo-paths /perm/* para no duplicar path
            // - Dashboard se controla con DASHBOARD_VIEW
            // ------------------------------------------------------------------
            var permisos = new List<AccesoRuta>
            {
                // Core
                Perm("DASHBOARD_VIEW", "Dashboard", "/", "CORE", isMenu: true,
                    descripcion: "Pantalla inicial post-login"),

                // Usuarios
                Perm("USERS_READ", "Ver usuarios", "/admin/users", "USERS", isMenu: true),
                Perm("USERS_WRITE", "Crear/editar usuarios", "/perm/users:write", "USERS"),

                // Roles y permisos
                Perm("ROLES_READ", "Ver roles", "/admin/roles", "ROLES", isMenu: true),
                Perm("ROLES_WRITE", "Crear/editar roles", "/perm/roles:write", "ROLES"),
                Perm("ROUTES_ADMIN", "Asignar permisos", "/admin/permissions", "ROLES", isMenu: true),

                // Organización
                Perm("ORG_ADMIN", "Organización", "/admin/org", "ORG", isMenu: true),

                // Catálogo
                Perm("CATALOG_ADMIN", "Catálogo", "/admin/catalog", "CATALOG", isMenu: true),

                // Formularios dinámicos
                Perm("FORMS_ADMIN", "Form Builder", "/admin/forms", "FORMS", isMenu: true),

                // Precios / Impuestos / Promos
                Perm("PRICING_ADMIN", "Precios y Promos", "/admin/pricing", "PRICING", isMenu: true),

                // Inventario
                Perm("INVENTORY_READ", "Ver inventario", "/admin/inventory", "INVENTORY", isMenu: true),
                Perm("INVENTORY_WRITE", "Editar inventario", "/perm/inventory:write", "INVENTORY"),

                // Dispositivos
                Perm("DEVICES_ADMIN", "Dispositivos", "/admin/devices", "DEVICES", isMenu: true),

                // Reportes
                Perm("REPORTS_VIEW", "Reportes", "/admin/reports", "REPORTS", isMenu: true),
            };

            // UPSERT por Key (idempotente: si cambias Nombre/Path/Group/IsMenu se actualiza)
            foreach (var p in permisos)
            {
                var existing = await _db.Set<AccesoRuta>().FirstOrDefaultAsync(x => x.Key == p.Key, ct);
                if (existing is null)
                {
                    _db.Add(p);
                }
                else
                {
                    bool changed = false;
                    if (existing.Nombre != p.Nombre)
                    {
                        existing.Nombre = p.Nombre;
                        changed = true;
                    }

                    if (existing.Path != p.Path)
                    {
                        existing.Path = p.Path;
                        changed = true;
                    }

                    if (existing.Group != p.Group)
                    {
                        existing.Group = p.Group;
                        changed = true;
                    }

                    if (existing.IsMenu != p.IsMenu)
                    {
                        existing.IsMenu = p.IsMenu;
                        changed = true;
                    }

                    if (existing.Descripcion != p.Descripcion)
                    {
                        existing.Descripcion = p.Descripcion;
                        changed = true;
                    }

                    if (!existing.IsActive)
                    {
                        existing.IsActive = true;
                        changed = true;
                    }

                    if (changed)
                    {
                        existing.UpdatedAt = DateTime.UtcNow;
                        existing.UpdatedBy = "seed";
                        _db.Update(existing);
                    }
                }
            }

            await _db.SaveChangesAsync(ct);

            // ------------------------------------------------------------------
            // 2) Roles base
            // ------------------------------------------------------------------
            var admin = await _db.Roles.AsNoTracking().FirstAsync(r => r.Nombre == "Admin", ct);
            var manager = await _db.Roles.AsNoTracking().FirstAsync(r => r.Nombre == "Manager", ct);
            var mesero = await _db.Roles.AsNoTracking().FirstAsync(r => r.Nombre == "Mesero", ct);

            // ------------------------------------------------------------------
            // 3) Asignación de permisos por rol
            // ------------------------------------------------------------------
            var allPerms = await _db.Set<AccesoRuta>().AsNoTracking().ToListAsync(ct);

            // Admin => todos
            await EnsureRolePermissionsAsync(admin.Id, allPerms.Select(x => x.Id), ct);

            // Manager => operativo de administración (sin editar roles)
            var managerKeys = new[]
            {
                "DASHBOARD_VIEW",
                "USERS_READ",
                "ORG_ADMIN",
                "CATALOG_ADMIN",
                "FORMS_ADMIN",
                "PRICING_ADMIN",
                "INVENTORY_READ", "INVENTORY_WRITE",
                "DEVICES_ADMIN",
                "REPORTS_VIEW"
            };
            await EnsureRolePermissionsAsync(
                manager.Id,
                allPerms.Where(p => managerKeys.Contains(p.Key)).Select(p => p.Id),
                ct
            );

            // Mesero => lo mínimo (ajústalo a tu UX real)
            var meseroKeys = new[]
            {
                "DASHBOARD_VIEW",
                // si el mesero no debe entrar a admin, quita todos los /admin/*. 
                // para front de operación crea luego permisos específicos (e.g., ORDER_TAKE)
            };
            await EnsureRolePermissionsAsync(
                mesero.Id,
                allPerms.Where(p => meseroKeys.Contains(p.Key)).Select(p => p.Id),
                ct
            );

            await tx.CommitAsync(ct);
            _logger.LogInformation("Seed RBAC completado: permisos y roles asignados.");
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            _logger.LogError(ex, "Error durante el seeding de RBAC");
            throw;
        }
    }

    private async Task<Rol> EnsureRoleAsync(string nombre, bool isSystem, bool isAssignable, CancellationToken ct)
    {
        var r = await _db.Set<Rol>().FirstOrDefaultAsync(x => x.Nombre == nombre, ct);
        if (r is null)
        {
            r = new Rol
            {
                Nombre = nombre,
                IsSystem = isSystem,
                IsAssignable = isAssignable,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            };
            _db.Add(r);
            await _db.SaveChangesAsync(ct);
        }
        else
        {
            bool changed = false;
            if (r.IsSystem != isSystem)
            {
                r.IsSystem = isSystem;
                changed = true;
            }

            if (r.IsAssignable != isAssignable)
            {
                r.IsAssignable = isAssignable;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(r.ConcurrencyStamp))
            {
                r.ConcurrencyStamp = Guid.NewGuid().ToString();
                changed = true;
            }

            if (!r.IsActive)
            {
                r.IsActive = true;
                changed = true;
            }

            if (changed)
            {
                r.UpdatedAt = DateTime.UtcNow;
                r.UpdatedBy = "seed";
                _db.Update(r);
                await _db.SaveChangesAsync(ct);
            }
        }

        return r;
    }

    private async Task EnsureRolePermissionsAsync(int roleId, IEnumerable<int> permisoIds, CancellationToken ct)
    {
        // declarativo: reemplaza asignaciones del rol
        var current = _db.Set<RolAccesoRuta>().Where(x => x.IdRol == roleId);
        _db.RemoveRange(current);

        foreach (var pid in permisoIds.Distinct())
        {
            _db.Add(new RolAccesoRuta
            {
                IdRol = roleId,
                IdAccesoRuta = pid,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "seed"
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    private async Task<Usuario> EnsureAdminUserAsync(CancellationToken ct)
    {
        var admin = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == "admin@mesafacil.local", ct);
        if (admin is null)
        {
            admin = new Usuario
            {
                Correo = "admin@mesafacil.local",
                NombreCompleto = "Administrador",
                IdEmpresa = 1, // ajusta si tu modelo lo requiere
                IsActive = true
            };
            _db.Usuarios.Add(admin);
            await _db.SaveChangesAsync(ct);

            // Credencial
            var (hash, salt) = CreatePasswordHash(adminPlainPassword);
            _db.Credenciales.Add(new Credencial
            {
                Id = admin.Id,
                Hash = hash,
                Salt = salt
            });
            await _db.SaveChangesAsync(ct);
        }

        return admin;
    }

    private async Task EnsureUserRoleAsync(int usuarioId, int rolId, CancellationToken ct)
    {
        var exists = await _db.UsuarioRoles.AnyAsync(x => x.UsuarioId == usuarioId && x.IdRol == rolId, ct);
        if (!exists)
        {
            _db.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuarioId, IdRol = rolId });
            await _db.SaveChangesAsync(ct);
        }
    }
}
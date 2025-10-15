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

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        // Política: migrar solo en Dev; en Prod hazlo por CI/CD o job separado
        // if (_env.IsDevelopment())
        // {
        //     _logger.LogInformation("Applying EF Core migrations...");
        //     await _db.Database.MigrateAsync(ct);
        // }

        await SeedFormulariosAsync(ct);
        await SeedAdminUserAsync(ct);
        await SeedAccesosRutasAsync(ct);
    }

    private async Task SeedFormulariosAsync(CancellationToken ct)
    {
        const string catalogCode = "FORM";
        const string catalogName = "Formularios";
        const string loginCode = "LOGIN";
        const string loginName = "Formulario de Login";

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // ================================
            // 1) Catálogo: FORM
            // ================================
            var catalog = await _db.Set<Catalog>()
                .FirstOrDefaultAsync(c => c.Code == catalogCode, ct);

            if (catalog == null)
            {
                catalog = new Catalog
                {
                    Code = catalogCode,
                    Name = catalogName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(catalog);
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("Seed: creado catálogo {Code}", catalogCode);
            }
            else
            {
                // Si quieres mantener el nombre actualizado sin duplicar migraciones:
                if (catalog.Name != catalogName)
                {
                    catalog.Name = catalogName;
                    catalog.UpdatedAt = DateTime.UtcNow;
                    catalog.UpdatedBy = "seed";
                    await _db.SaveChangesAsync(ct);
                    _logger.LogInformation("Seed: actualizado nombre catálogo {Code}", catalogCode);
                }
            }

            // ================================
            // 2) CatalogItem: LOGIN
            // ================================
            var loginItem = await _db.Set<CatalogItem>()
                .FirstOrDefaultAsync(ci => ci.CatalogId == catalog.Id && ci.Code == loginCode, ct);

            if (loginItem == null)
            {
                loginItem = new CatalogItem
                {
                    CatalogId = catalog.Id,
                    Code = loginCode,
                    Name = loginName,
                    SortOrder = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(loginItem);
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("Seed: creado item {ItemCode} en catálogo {CatalogCode}", loginCode,
                    catalogCode);
            }
            else
            {
                // Mantener nombre y estado alineados
                bool changed = false;
                if (loginItem.Name != loginName)
                {
                    loginItem.Name = loginName;
                    changed = true;
                }

                if (!loginItem.IsActive)
                {
                    loginItem.IsActive = true;
                    changed = true;
                }

                if (changed)
                {
                    loginItem.UpdatedAt = DateTime.UtcNow;
                    loginItem.UpdatedBy = "seed";
                    await _db.SaveChangesAsync(ct);
                    _logger.LogInformation("Seed: actualizado item {ItemCode}", loginCode);
                }
            }

            var formCatalogId = loginItem.CatalogId;
            var formItemId = loginItem.Id;

            // ================================
            // 3) FormFields: username
            // ================================
            await EnsureFormFieldAsync(
                formCatalogId, formItemId,
                name: "username",
                type: "text",
                label: "Usuario",
                placeholder: "Ingresa tu usuario o email",
                order: 1,
                validations: new()
                {
                    new FormValidation { Type = "required", Value = 1 },
                    // Si deseas aplicar email:
                    // new FormValidation { Type = "email", Value = 1 }
                },
                ct);

            // ================================
            // 4) FormFields: password
            // ================================
            await EnsureFormFieldAsync(
                formCatalogId, formItemId,
                name: "password",
                type: "password",
                label: "Contraseña",
                placeholder: "Ingresa tu contraseña",
                order: 2,
                validations: new()
                {
                    new FormValidation { Type = "required", Value = 1 },
                    new FormValidation { Type = "minLength", Value = 8 }
                },
                ct);

            await tx.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el seeding de formularios");
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private async Task EnsureFormFieldAsync(
        int formularioCatalogId,
        int formularioItemId,
        string name,
        string type,
        string label,
        string placeholder,
        int order,
        List<FormValidation> validations,
        CancellationToken ct)
    {
        var exists = await _db.Set<FormField>().AnyAsync(f =>
            f.FormularioCatalogId == formularioCatalogId &&
            f.FormularioItemId == formularioItemId &&
            f.Name == name, ct);

        if (!exists)
        {
            var field = new FormField
            {
                Type = type,
                Name = name,
                Label = label,
                Placeholder = placeholder,
                Value = string.Empty,
                Order = order,

                FormularioCatalogId = formularioCatalogId,
                FormularioItemId = formularioItemId,

                CatalogId = null, // sin relación a otro catálogo

                Validations = validations,
                Options = new()
            };

            _db.Add(field);
            await _db.SaveChangesAsync(ct);
        }
        else
        {
            // Opcional: mantener metadatos alineados si cambian
            var field = await _db.Set<FormField>().FirstAsync(f =>
                f.FormularioCatalogId == formularioCatalogId &&
                f.FormularioItemId == formularioItemId &&
                f.Name == name, ct);

            bool changed = false;

            if (field.Type != type)
            {
                field.Type = type;
                changed = true;
            }

            if (field.Label != label)
            {
                field.Label = label;
                changed = true;
            }

            if (field.Placeholder != placeholder)
            {
                field.Placeholder = placeholder;
                changed = true;
            }

            if (field.Order != order)
            {
                field.Order = order;
                changed = true;
            }

            // Si quieres sobreescribir reglas cuando difieren:
            // (esto reemplaza completamente la lista)
            if (!AreValidationsEqual(field.Validations, validations))
            {
                field.Validations = validations;
                changed = true;
            }

            if (changed)
                await _db.SaveChangesAsync(ct);
        }
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
        // ================================
        // 0) Constantes
        // ================================
        const string empresaNombre = "Empresa Demo";
        const string empresaRazon = "Empresa Demo S.A. de C.V."; // ajusta a tu modelo de Empresa
        const string adminNombre = "Administrador";
        const string adminCorreo = "admin@mesafacil.local";
        const string rolAdminCode = "ADMIN";
        const string rolAdminNombre = "Admin";
        const string credCatalogCode = "CRED";
        const string credCatalogName = "Tipos de Credencial";
        const string credItemCode = "PASSWORD";
        const string credItemName = "Contraseña";

        // Password semilla (cámbialo en prod)
        const string adminPlainPassword = "Admin123!";

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            // ================================
            // 1) Empresa (necesaria para Usuario.IdEmpresa)
            // ================================
            // Asumo que Empresa tiene Code/Name; ajusta según tu entidad real.
            var empresa = await _db.Set<Empresa>()
                .FirstOrDefaultAsync(e => e.Nombre == empresaNombre, ct);

            if (empresa is null)
            {
                empresa = new Empresa
                {
                    Nombre = empresaNombre,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(empresa);
                await _db.SaveChangesAsync(ct);
            }

            // ================================
            // 2) Rol ADMIN
            // ================================
            var rolAdmin = await _db.Set<Rol>()
                .FirstOrDefaultAsync(r => r.Nombre == rolAdminNombre, ct);

            if (rolAdmin is null)
            {
                rolAdmin = new Rol
                {
                    Nombre = rolAdminNombre,
                    IsSystem = true, // los system roles suelen marcarse así
                    IsAssignable = true, // el admin se puede asignar
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    IsActive = true, // si tu BaseAuditableEntity lo tiene
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };

                _db.Add(rolAdmin);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                bool changed = false;

                if (!rolAdmin.IsSystem)
                {
                    rolAdmin.IsSystem = true;
                    changed = true;
                }

                if (!rolAdmin.IsAssignable)
                {
                    rolAdmin.IsAssignable = true;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(rolAdmin.ConcurrencyStamp))
                {
                    rolAdmin.ConcurrencyStamp = Guid.NewGuid().ToString();
                    changed = true;
                }

                if (!rolAdmin.IsActive)
                {
                    rolAdmin.IsActive = true;
                    changed = true;
                }

                // Si quieres asegurar nombre exacto (por si alguien lo modificó):
                if (rolAdmin.Nombre != rolAdminNombre)
                {
                    rolAdmin.Nombre = rolAdminNombre;
                    changed = true;
                }

                if (changed)
                {
                    rolAdmin.UpdatedAt = DateTime.UtcNow;
                    rolAdmin.UpdatedBy = "seed";
                    await _db.SaveChangesAsync(ct);
                }
            }


            // ================================
            // 3) Catálogo/Ítem para tipo de credencial (CRED → PASSWORD)
            // ================================
            var credCatalog = await _db.Set<Catalog>()
                .FirstOrDefaultAsync(c => c.Code == credCatalogCode, ct);

            if (credCatalog is null)
            {
                credCatalog = new Catalog
                {
                    Code = credCatalogCode,
                    Name = credCatalogName,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(credCatalog);
                await _db.SaveChangesAsync(ct);
            }

            var credPasswordItem = await _db.Set<CatalogItem>().FirstOrDefaultAsync(ci =>
                ci.CatalogId == credCatalog.Id && ci.Code == credItemCode, ct);

            if (credPasswordItem is null)
            {
                credPasswordItem = new CatalogItem
                {
                    CatalogId = credCatalog.Id,
                    Code = credItemCode,
                    Name = credItemName,
                    IsActive = true,
                    SortOrder = 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(credPasswordItem);
                await _db.SaveChangesAsync(ct);
            }

            // ================================
            // 4) Usuario admin
            // ================================
            var admin = await _db.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.Correo == adminCorreo, ct);

            if (admin is null)
            {
                admin = new Usuario
                {
                    IdEmpresa = empresa.Id,
                    NombreCompleto = adminNombre,
                    Correo = adminCorreo,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(admin);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                bool changed = false;
                if (admin.IdEmpresa != empresa.Id)
                {
                    admin.IdEmpresa = empresa.Id;
                    changed = true;
                }

                if (admin.NombreCompleto != adminNombre)
                {
                    admin.NombreCompleto = adminNombre;
                    changed = true;
                }

                if (!admin.IsActive)
                {
                    admin.IsActive = true;
                    changed = true;
                }

                if (changed)
                {
                    admin.UpdatedAt = DateTime.UtcNow;
                    admin.UpdatedBy = "seed";
                    await _db.SaveChangesAsync(ct);
                }
            }

            // ================================
            // 5) UsuarioRol (vincular ADMIN)
            // ================================
            // OJO: tu mapping define:
            //   - PK: { Id, IdRol }
            //   - FK a Usuario por 'Id' (heredado de BaseAuditableEntity) en UsuarioRol
            // Eso implica que UsuarioRol.Id == Usuario.Id (inusual, pero así está configurado).
            var usuarioRolExists = await _db.Set<UsuarioRol>()
                .AnyAsync(ur => ur.Id == admin.Id && ur.IdRol == rolAdmin.Id, ct);

            if (!usuarioRolExists)
            {
                var ur = new UsuarioRol
                {
                    // IMPORTANTE: aquí el Id de UsuarioRol DEBE SER el Id del Usuario (por tu FK)
                    Id = admin.Id,
                    IdRol = rolAdmin.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(ur);
                await _db.SaveChangesAsync(ct);
            }

            // ================================
            // 6) Credencial (Password) para admin
            //     PK: (IdUsuario, TipoCatalogId, TipoItemId)
            // ================================
            var credExists = await _db.Set<Credencial>().AnyAsync(c =>
                c.IdUsuario == admin.Id &&
                c.TipoCatalogId == credPasswordItem.CatalogId &&
                c.TipoItemId == credPasswordItem.Id, ct);

            if (!credExists)
            {
                // Hash PBKDF2 con salt aleatorio
                var (hash, salt) = CreatePasswordHash(adminPlainPassword);

                var cred = new Credencial
                {
                    IdUsuario = admin.Id,
                    TipoCatalogId = credPasswordItem.CatalogId,
                    TipoItemId = credPasswordItem.Id,
                    Hash = hash,
                    Salt = salt,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "seed"
                };
                _db.Add(cred);
                await _db.SaveChangesAsync(ct);
            }

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
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

}
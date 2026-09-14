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

        // 0) Reparar datos corruptos de ejecuciones anteriores
        await RepairCorruptRolesAsync(ct);

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

        // 5) Formulario genérico de catálogos
        await SeedFormularioCatalogosAsync(ct);
        
        // 6) Formulario de Productos
        await SeedFormularioProductosAsync(ct);
        
        // 7) Formulario de Categorias
        await SeedFormularioCategoriasAsync(ct);
        
        // 8) Formulario de Menus
        await SeedFormularioMenusAsync(ct);
        
        // 9) Formulario de Variantes
        await SeedFormularioVariantesAsync(ct);
        
        // 10) Formulario de Precios
        await SeedFormularioPreciosAsync(ct);
        
        // 11) Formularios de Modificadores
        await SeedFormularioGrupoModificadorAsync(ct);
        await SeedFormularioOpcionModificadorAsync(ct);
        // 12) CatEstadoMesa
        await SeedCatEstadoMesaAsync(ct);
        
        // 13) Monedas e Impuestos
        await SeedCatMonedasAsync(ct);
        await SeedCatImpuestosAsync(ct);

        // 14) Estados y Tipos de Pedido
        await SeedCatEstadoPedidoAsync(ct);
        await SeedCatEstadoPedidoDetalleAsync(ct);
        await SeedCatTipoPedidoAsync(ct);

        // 15) Modificadores para producto de prueba (Café Americano)
        await SeedCoffeeModifiersAsync(ct);

        // 16) KDS Catalogs
        await SeedCatEstacionesCocinaAsync(ct);
        await SeedEstacionCocinaAsync(ct);
        await SeedCatEstadoTicketCocinaAsync(ct);
        await SeedCatEstadoItemKDSAsync(ct);

        // 17) Delivery & Métodos de Pago
        await EnsureDeliveryColumnsAsync(ct);
        await SeedCatMetodosDePagoAsync(ct);

        // 18) Nuevos Catálogos del Sistema (Almacenes, Inventario, Caja, Cancelaciones y Canales)
        await SeedCatTipoAlmacenAsync(ct);
        await SeedCatMotivoMovimientoInventarioAsync(ct);
        await SeedCatConceptoMovimientoCajaAsync(ct);
        await SeedCatMotivoCancelacionPedidoAsync(ct);
        await SeedCatCanalVentaAsync(ct);

        // 19) Spec 024: Candado de Supervisor y Auditoría de Cancelaciones
        await SeedCatMotivoCancelacionAsync(ct);

        _logger.LogInformation("Inicialización completada con éxito.");
    }

    /// <summary>
    /// Corrige roles que llegaron a la DB con campos inválidos por ausencia de validación previa.
    /// - ConcurrencyStamp NULL  → genera un GUID nuevo
    /// - IsActive = false       → lo reactiva (roles que nunca debieron quedar inactivos por defecto)
    /// - Nombre vacío           → los elimina (no tienen valor de negocio recuperable)
    /// </summary>
    private async Task RepairCorruptRolesAsync(CancellationToken ct)
    {
        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        // 1. Eliminar roles sin nombre (irrecuperables)
        var sinNombre = await _db.Roles
            .Where(r => r.Nombre == null || r.Nombre == string.Empty)
            .ToListAsync(ct);

        if (sinNombre.Count > 0)
        {
            _db.RemoveRange(sinNombre);
            await _db.SaveChangesAsync(ct);
            _logger.LogWarning("Seed: eliminados {Count} rol(es) sin Nombre.", sinNombre.Count);
        }

        // 2. Reparar roles con ConcurrencyStamp NULL
        var sinStamp = await _db.Roles
            .Where(r => r.ConcurrencyStamp == null || r.ConcurrencyStamp == string.Empty)
            .ToListAsync(ct);

        foreach (var r in sinStamp)
        {
            r.ConcurrencyStamp = Guid.NewGuid().ToString();
            r.UpdatedAt  = now;
            r.UpdatedBy  = "seed-repair";
        }

        if (sinStamp.Count > 0)
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogWarning("Seed: reparado ConcurrencyStamp en {Count} rol(es).", sinStamp.Count);
        }
    }

    private async Task SeedCatEstadoMesaAsync(CancellationToken ct)
    {
        var estados = new[] { "Disponible", "Ocupada", "Reservada", "Sucia", "Fuera de Servicio" };
        foreach (var estado in estados)
        {
            var exists = await _db.Set<CatEstadoMesa>().AnyAsync(e => e.Descripcion == estado, ct);
            if (!exists)
            {
                _db.Add(new CatEstadoMesa
                {
                    Descripcion = estado,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados estados base en CatEstadoMesa.");
    }

    private async Task SeedCatEstacionesCocinaAsync(CancellationToken ct)
    {
        var estaciones = new[] { "Parrilla & Brasa", "Cocina Caliente & Fría", "Barra & Coctelería" };
        foreach (var est in estaciones)
        {
            var exists = await _db.Set<CatEstacionesCocina>().AnyAsync(e => e.Descripcion == est, ct);
            if (!exists)
            {
                _db.Add(new CatEstacionesCocina
                {
                    Descripcion = est,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: aseguradas estaciones base en CatEstacionesCocina.");
    }

    private async Task SeedEstacionCocinaAsync(CancellationToken ct)
    {
        var estacion = await _db.Set<EstacionCocina>().FirstOrDefaultAsync(e => e.Id == 1, ct);
        if (estacion == null)
        {
            _db.Add(new EstacionCocina
            {
                Id = 1,
                Nombre = "Cocina Principal",
                IdSucursal = 1,
                IsActive = true,
                CreatedBy = "seed"
            });
            await _db.SaveChangesAsync(ct);
        }
    }

    private async Task SeedCatEstadoTicketCocinaAsync(CancellationToken ct)
    {
        var estados = new[] { "Pendiente", "Preparando", "Listo" };
        for (int i = 0; i < estados.Length; i++)
        {
            int id = i + 1;
            if (!await _db.Set<CatEstadoTicketCocina>().AnyAsync(e => e.Id == id, ct))
            {
                _db.Add(new CatEstadoTicketCocina { Id = id, Descripcion = estados[i], IsActive = true, CreatedBy = "seed" });
            }
        }
        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedCatEstadoItemKDSAsync(CancellationToken ct)
    {
        var estados = new[] { "Pendiente", "Preparando", "Listo" };
        for (int i = 0; i < estados.Length; i++)
        {
            int id = i + 1;
            if (!await _db.Set<CatEstadoItemKDS>().AnyAsync(e => e.Id == id, ct))
            {
                _db.Add(new CatEstadoItemKDS { Id = id, Descripcion = estados[i], IsActive = true, CreatedBy = "seed" });
            }
        }
        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedCatEstadoPedidoAsync(CancellationToken ct)
    {
        var estados = new[] { "Registrado", "En Preparación", "Listo", "En Camino", "Entregado", "Cerrado", "Cancelado" };
        foreach (var estado in estados)
        {
            var exists = await _db.Set<CatEstadoPedido>().AnyAsync(e => e.Descripcion == estado, ct);
            if (!exists)
            {
                _db.Add(new CatEstadoPedido
                {
                    Descripcion = estado,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados estados base en CatEstadoPedido.");
    }

    private async Task SeedCatEstadoPedidoDetalleAsync(CancellationToken ct)
    {
        var estados = new[] { "Registrado", "En Cocina", "Preparado", "Entregado", "Cancelado" };
        foreach (var estado in estados)
        {
            var exists = await _db.Set<CatEstadoPedidoDetalle>().AnyAsync(e => e.Descripcion == estado, ct);
            if (!exists)
            {
                _db.Add(new CatEstadoPedidoDetalle
                {
                    Descripcion = estado,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados estados base en CatEstadoPedidoDetalle.");
    }

    private async Task SeedCatTipoPedidoAsync(CancellationToken ct)
    {
        var tipos = new[] { "Comedor", "Para Llevar", "Delivery" };
        foreach (var tipo in tipos)
        {
            var exists = await _db.Set<CatTipoPedido>().AnyAsync(e => e.Descripcion == tipo, ct);
            if (!exists)
            {
                _db.Add(new CatTipoPedido
                {
                    Descripcion = tipo,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados tipos base en CatTipoPedido.");
    }

    private async Task SeedCatMonedasAsync(CancellationToken ct)
    {
        var monedas = new[] { "MXN", "USD" };
        foreach (var moneda in monedas)
        {
            var exists = await _db.Set<CatMoneda>().AnyAsync(e => e.Descripcion == moneda, ct);
            if (!exists)
            {
                _db.Add(new CatMoneda
                {
                    Descripcion = moneda,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: aseguradas monedas base en CatMoneda.");
    }

    private async Task SeedCatImpuestosAsync(CancellationToken ct)
    {
        var impuestos = new[] { "IVA 16%", "IVA 8%", "Exento" };
        foreach (var impuesto in impuestos)
        {
            var exists = await _db.Set<CatImpuesto>().AnyAsync(e => e.Descripcion == impuesto, ct);
            if (!exists)
            {
                _db.Add(new CatImpuesto
                {
                    Descripcion = impuesto,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados impuestos base en CatImpuesto.");
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

    /// <summary>
    /// Siembra el formulario CRUD genérico utilizado por todos los catálogos simples (Cat*).
    /// Código: CATALOGO_CRUD — dos campos: Descripcion e IsActive.
    /// El frontend resuelve el catálogo concreto a través del parámetro de ruta (e.g. /catalogos/monedas).
    /// </summary>
    private async Task SeedFormularioCatalogosAsync(CancellationToken ct)
    {
        const string formCode = "CATALOGO_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario Genérico de Catálogo",
                Descripcion = "Formulario reutilizable para el CRUD de todos los catálogos simples (Cat*). " +
                              "El catálogo concreto se determina por el parámetro de ruta.",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario CATALOGO_CRUD.");
        }

        // Campo 1 — Descripción (texto libre, obligatorio)
        await EnsureFormFieldAsync(
            form.Id,
            name:        "descripcion",
            type:        "text",
            label:       "Descripción",
            placeholder: "Ingresa la descripción del catálogo",
            order:       1,
            validations: new()
            {
                new FormValidation { Type = "required",  Value = 1 },
                new FormValidation { Type = "maxLength",  Value = 200 }
            },
            ct);

        // Campo 2 — Activo (checkbox booleano)
        await EnsureFormFieldAsync(
            form.Id,
            name:        "isActive",
            type:        "checkbox",
            label:       "Activo",
            placeholder: "",
            order:       2,
            validations: new(),   // sin validaciones obligatorias
            ct);
    }

    private async Task SeedFormularioProductosAsync(CancellationToken ct)
    {
        const string formCode = "PRODUCTO_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario de Productos",
                Descripcion = "CRUD para la gestión de productos",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario PRODUCTO_CRUD.");
        }

        await EnsureFormFieldAsync(form.Id, "nombre", "text", "Nombre", "Ingresa el nombre del producto", 1, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "codigo", "text", "Código", "Ingresa el código del producto", 2, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "descripcion", "text", "Descripción", "Descripción del producto", 3, 
            new(), ct);

        await EnsureFormFieldAsync(form.Id, "idMenu", "select", "Menú", "Selecciona el menú", 4, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "menus");

        await EnsureFormFieldAsync(form.Id, "idCategoria", "select", "Categoría", "Selecciona la categoría", 5, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "categorias");

        await EnsureFormFieldAsync(form.Id, "idEstacionCocina", "select", "Estación KDS", "Selecciona la estación", 6, 
            new(), ct, dataSource: "estaciones");

        await EnsureFormFieldAsync(form.Id, "activo", "checkbox", "Activo", "", 7, 
            new(), ct);
    }

    private async Task SeedFormularioCategoriasAsync(CancellationToken ct)
    {
        const string formCode = "CATEGORIA_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario de Categorías",
                Descripcion = "CRUD para la gestión de categorías del menú",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario CATEGORIA_CRUD.");
        }

        await EnsureFormFieldAsync(form.Id, "nombre", "text", "Nombre", "Ingresa el nombre de la categoría", 1, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "idMenu", "select", "Menú", "Selecciona el menú", 2, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "menus");

        await EnsureFormFieldAsync(form.Id, "orden", "text", "Orden", "Orden de aparición (Ej. 1, 2, 3)", 3, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "activo", "checkbox", "Activo", "", 4, 
            new(), ct);
    }

    private async Task SeedFormularioMenusAsync(CancellationToken ct)
    {
        const string formCode = "MENU_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario de Menús",
                Descripcion = "CRUD para la gestión de menús",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario MENU_CRUD.");
        }

        await EnsureFormFieldAsync(form.Id, "nombre", "text", "Nombre", "Ingresa el nombre del menú", 1, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "idSucursal", "select", "Sucursal", "Selecciona la sucursal", 2, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "sucursales");

        await EnsureFormFieldAsync(form.Id, "activo", "checkbox", "Activo", "", 3, 
            new(), ct);
    }

    private async Task SeedFormularioVariantesAsync(CancellationToken ct)
    {
        const string formCode = "VARIANTE_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario de Variantes",
                Descripcion = "CRUD para la gestión de variantes de productos",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario VARIANTE_CRUD.");
        }

        await EnsureFormFieldAsync(form.Id, "nombre", "text", "Nombre", "Ej: Regular, Grande, Extra", 1, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "codigo", "text", "Código", "Código interno (opcional)", 2, 
            new(), ct);

        await EnsureFormFieldAsync(form.Id, "idProducto", "select", "Producto", "Selecciona el producto", 3, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "productos");

        await EnsureFormFieldAsync(form.Id, "esDefault", "checkbox", "Es Default", "", 4, 
            new(), ct);

        await EnsureFormFieldAsync(form.Id, "activo", "checkbox", "Activo", "", 5, 
            new(), ct);
    }

    private async Task SeedFormularioPreciosAsync(CancellationToken ct)
    {
        const string formCode = "PRECIO_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario de Precios",
                Descripcion = "CRUD para la gestión de precios",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario PRECIO_CRUD.");
        }

        await EnsureFormFieldAsync(form.Id, "idVariante", "select", "Variante", "Selecciona la variante", 1, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "variantes");

        await EnsureFormFieldAsync(form.Id, "monto", "text", "Monto", "0.00", 2, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "idMoneda", "select", "Moneda", "Selecciona la moneda", 3, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "monedas");

        await EnsureFormFieldAsync(form.Id, "idImpuesto", "select", "Impuesto", "Selecciona el impuesto", 4, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "impuestos");

        await EnsureFormFieldAsync(form.Id, "validoDesde", "date", "Válido Desde", "", 5, 
            new(), ct);

        await EnsureFormFieldAsync(form.Id, "validoHasta", "date", "Válido Hasta", "", 6, 
            new(), ct);

        await EnsureFormFieldAsync(form.Id, "activo", "checkbox", "Activo", "", 7, 
            new(), ct);
    }

    private async Task SeedFormularioGrupoModificadorAsync(CancellationToken ct)
    {
        const string formCode = "GRUPO_MODIFICADOR_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario de Grupo de Modificadores",
                Descripcion = "CRUD para grupos de modificadores",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario GRUPO_MODIFICADOR_CRUD.");
        }

        await EnsureFormFieldAsync(form.Id, "nombre", "text", "Nombre", "Ej: Tipo de Pan", 1, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "idProducto", "select", "Producto", "Selecciona el producto", 2, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "productos");

        await EnsureFormFieldAsync(form.Id, "minSeleccion", "text", "Selección Mínima", "0", 3, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "maxSeleccion", "text", "Selección Máxima", "1", 4, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "obligatorio", "checkbox", "Obligatorio", "", 5, 
            new(), ct);

        await EnsureFormFieldAsync(form.Id, "activo", "checkbox", "Activo", "", 6, 
            new(), ct);
    }

    private async Task SeedFormularioOpcionModificadorAsync(CancellationToken ct)
    {
        const string formCode = "OPCION_MODIFICADOR_CRUD";

        var form = await _db.Set<Formulario>().FirstOrDefaultAsync(f => f.Codigo == formCode, ct);
        if (form is null)
        {
            form = new Formulario
            {
                Codigo      = formCode,
                Nombre      = "Formulario de Opción de Modificadores",
                Descripcion = "CRUD para opciones de modificadores",
                IsActive    = true,
                CreatedBy   = "seed"
            };
            _db.Add(form);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seed: creado Formulario OPCION_MODIFICADOR_CRUD.");
        }

        await EnsureFormFieldAsync(form.Id, "nombre", "text", "Nombre", "Ej: Pan Blanco", 1, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "idGrupo", "select", "Grupo", "Selecciona el grupo", 2, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct, dataSource: "grupoModificadores");

        await EnsureFormFieldAsync(form.Id, "precioExtra", "text", "Precio Extra", "0.00", 3, 
            new() { new FormValidation { Type = "required", Value = 1 } }, ct);

        await EnsureFormFieldAsync(form.Id, "esDefault", "checkbox", "Es Default", "", 4, 
            new(), ct);

        await EnsureFormFieldAsync(form.Id, "activo", "checkbox", "Activo", "", 5, 
            new(), ct);
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
            await EnsureAccesoAndBindAsync("POS", "/ventas/pos", new[] { "Admin" }, ct);
            
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

                // MenÃº y Productos
                Perm("MENU_ADMIN", "GestiÃ³n de MenÃº", "/menu", "MENU", isMenu: true),

                // Reportes
                Perm("REPORTS_VIEW", "Reportes", "/admin/reports", "REPORTS", isMenu: true),

                // Operación
                Perm("POS_VIEW", "Punto de Venta", "/ventas/pos", "OPERACION", isMenu: true),
                Perm("KDS_VIEW", "Cocina KDS", "/ventas/kds", "OPERACION", isMenu: true),
                Perm("DELIVERY_VIEW", "Delivery y Despacho", "/delivery", "OPERACION", isMenu: true),

                // Caja
                Perm("CAJA_TURNOS", "Turnos de Caja", "/caja/turnos", "CAJA", isMenu: true),
                Perm("CAJA_MOVIMIENTOS", "Movimientos de Caja", "/caja/movimientos", "CAJA", isMenu: true),
                Perm("CAJA_CORTES", "Cortes de Caja", "/caja/cortes", "CAJA", isMenu: true),
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
                "MENU_ADMIN",
                "INVENTORY_READ", "INVENTORY_WRITE",
                "DEVICES_ADMIN",
                "REPORTS_VIEW",
                "POS_VIEW",
                "KDS_VIEW",
                "DELIVERY_VIEW",
                "CAJA_TURNOS",
                "CAJA_MOVIMIENTOS",
                "CAJA_CORTES"
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
                "POS_VIEW",
                "KDS_VIEW",
                "DELIVERY_VIEW"
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

    private async Task SeedCoffeeModifiersAsync(CancellationToken ct)
    {
        var cafeId = 1; // Asumimos que el Café Americano tiene ID 1
        
        // Verificar si ya existe algún grupo
        var exists = await _db.Set<GrupoModificador>().AnyAsync(g => g.IdProducto == cafeId && g.Nombre == "Tipo de Leche", ct);
        if (exists) return;

        // Tipo de Leche
        var grpLeche = new GrupoModificador { IdProducto = cafeId, Nombre = "Tipo de Leche", MinSeleccion = 0, MaxSeleccion = 1, Obligatorio = false, IsActive = true, CreatedBy = "seed" };
        _db.Set<GrupoModificador>().Add(grpLeche);

        // Endulzante
        var grpEndulzante = new GrupoModificador { IdProducto = cafeId, Nombre = "Endulzante", MinSeleccion = 0, MaxSeleccion = 2, Obligatorio = false, IsActive = true, CreatedBy = "seed" };
        _db.Set<GrupoModificador>().Add(grpEndulzante);

        // Extras
        var grpExtras = new GrupoModificador { IdProducto = cafeId, Nombre = "Shots / Extras", MinSeleccion = 0, MaxSeleccion = 5, Obligatorio = false, IsActive = true, CreatedBy = "seed" };
        _db.Set<GrupoModificador>().Add(grpExtras);

        // Temperatura
        var grpTemp = new GrupoModificador { IdProducto = cafeId, Nombre = "Temperatura", MinSeleccion = 1, MaxSeleccion = 1, Obligatorio = true, IsActive = true, CreatedBy = "seed" };
        _db.Set<GrupoModificador>().Add(grpTemp);

        await _db.SaveChangesAsync(ct);

        // Opciones Tipo Leche
        _db.Set<OpcionModificador>().AddRange(
            new OpcionModificador { IdGrupo = grpLeche.Id, Nombre = "Sin Leche", PrecioExtra = 0, EsDefault = true, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpLeche.Id, Nombre = "Entera", PrecioExtra = 0, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpLeche.Id, Nombre = "Deslactosada", PrecioExtra = 5m, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpLeche.Id, Nombre = "Almendra", PrecioExtra = 10m, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpLeche.Id, Nombre = "Avena", PrecioExtra = 12m, EsDefault = false, IsActive = true, CreatedBy = "seed" }
        );

        // Opciones Endulzante
        _db.Set<OpcionModificador>().AddRange(
            new OpcionModificador { IdGrupo = grpEndulzante.Id, Nombre = "Sobres de Azúcar", PrecioExtra = 0, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpEndulzante.Id, Nombre = "Stevia", PrecioExtra = 0, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpEndulzante.Id, Nombre = "Splenda", PrecioExtra = 0, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpEndulzante.Id, Nombre = "Miel de Abeja", PrecioExtra = 5m, EsDefault = false, IsActive = true, CreatedBy = "seed" }
        );

        // Opciones Extras
        _db.Set<OpcionModificador>().AddRange(
            new OpcionModificador { IdGrupo = grpExtras.Id, Nombre = "Shot Espresso", PrecioExtra = 15m, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpExtras.Id, Nombre = "Jarabe de Vainilla", PrecioExtra = 10m, EsDefault = false, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpExtras.Id, Nombre = "Jarabe de Caramelo", PrecioExtra = 10m, EsDefault = false, IsActive = true, CreatedBy = "seed" }
        );

        // Opciones Temperatura
        _db.Set<OpcionModificador>().AddRange(
            new OpcionModificador { IdGrupo = grpTemp.Id, Nombre = "Caliente", PrecioExtra = 0, EsDefault = true, IsActive = true, CreatedBy = "seed" },
            new OpcionModificador { IdGrupo = grpTemp.Id, Nombre = "Frío / En las Rocas", PrecioExtra = 5m, EsDefault = false, IsActive = true, CreatedBy = "seed" }
        );

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: Modificadores para Café Americano agregados.");
    }

    private async Task EnsureDeliveryColumnsAsync(CancellationToken ct)
    {
        try
        {
            var sql = @"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'CanalOrigen') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""CanalOrigen"" text NOT NULL DEFAULT 'POS';
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'IdExterno') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""IdExterno"" text NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'NombreClienteDelivery') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""NombreClienteDelivery"" text NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'TelefonoDelivery') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""TelefonoDelivery"" text NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'DireccionEntrega') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""DireccionEntrega"" text NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'NombreRepartidor') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""NombreRepartidor"" text NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'TelefonoRepartidor') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""TelefonoRepartidor"" text NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'DespachadoEn') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""DespachadoEn"" timestamp with time zone NULL;
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Pedido' AND column_name = 'EntregadoEn') THEN
                        ALTER TABLE ""Pedido"" ADD COLUMN ""EntregadoEn"" timestamp with time zone NULL;
                    END IF;
                END $$;";

            await _db.Database.ExecuteSqlRawAsync(sql, ct);
            _logger.LogInformation("DatabaseInitializer: columnas de delivery en 'Pedido' verificadas/creadas.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error asegurando columnas de delivery en Pedido.");
        }
    }

    private async Task SeedCatMetodosDePagoAsync(CancellationToken ct)
    {
        var metodos = new[]
        {
            "Efectivo",
            "Tarjeta",
            "Uber Eats",
            "Rappi",
            "Didi Food",
            "Transferencia"
        };

        foreach (var desc in metodos)
        {
            var exists = await _db.Set<CatMetodoDePago>().AnyAsync(m => m.Descripcion == desc, ct);
            if (!exists)
            {
                _db.Add(new CatMetodoDePago
                {
                    Descripcion = desc,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados métodos de pago base en CatMetodoDePago.");
    }

    private async Task SeedCatTipoAlmacenAsync(CancellationToken ct)
    {
        var tipos = new[]
        {
            new { Codigo = "GENERAL", Descripcion = "General / Bodega Central" },
            new { Codigo = "COCINA", Descripcion = "Cocina Principal" },
            new { Codigo = "BARRA", Descripcion = "Barra / Bebidas" },
            new { Codigo = "PRODUCCION", Descripcion = "Producción / Subrecetas" }
        };

        foreach (var t in tipos)
        {
            var exists = await _db.Set<CatTipoAlmacen>().AnyAsync(x => x.Codigo == t.Codigo, ct);
            if (!exists)
            {
                _db.Add(new CatTipoAlmacen
                {
                    Codigo = t.Codigo,
                    Descripcion = t.Descripcion,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados tipos de almacén en CatTipoAlmacen.");
    }

    private async Task SeedCatMotivoMovimientoInventarioAsync(CancellationToken ct)
    {
        var motivos = new[]
        {
            // Entradas
            new { Codigo = "CompraEmergencia", Tipo = "EntradaManual", Descripcion = "Compra de Emergencia / Caja Chica" },
            new { Codigo = "Donacion", Tipo = "EntradaManual", Descripcion = "Donación / Bonificación" },
            new { Codigo = "AjusteCargaManual", Tipo = "EntradaManual", Descripcion = "Carga Manual de Stock" },
            new { Codigo = "OtroEntrada", Tipo = "EntradaManual", Descripcion = "Otro Motivo de Entrada" },

            // Mermas / Bajas
            new { Codigo = "Caducidad", Tipo = "SalidaMerma", Descripcion = "Caducidad / Vencimiento" },
            new { Codigo = "Descomposicion", Tipo = "SalidaMerma", Descripcion = "Descomposición / Mal Estado" },
            new { Codigo = "CaidaAccidente", Tipo = "SalidaMerma", Descripcion = "Caída o Accidente en Cocina" },
            new { Codigo = "DegustacionCortesia", Tipo = "SalidaMerma", Descripcion = "Degustación / Cortesía" },
            new { Codigo = "MermaOperativa", Tipo = "SalidaMerma", Descripcion = "Merma Operativa de Preparación" },

            // Ajustes
            new { Codigo = "AjusteManual", Tipo = "AjusteInventario", Descripcion = "Corrección de Conteo Físico" },
            new { Codigo = "MuestraCalidad", Tipo = "AjusteInventario", Descripcion = "Muestra de Calidad" },
            new { Codigo = "OtroAjuste", Tipo = "AjusteInventario", Descripcion = "Otro Ajuste" }
        };

        foreach (var m in motivos)
        {
            var exists = await _db.Set<CatMotivoMovimientoInventario>().AnyAsync(x => x.Codigo == m.Codigo && x.TipoMovimiento == m.Tipo, ct);
            if (!exists)
            {
                _db.Add(new CatMotivoMovimientoInventario
                {
                    Codigo = m.Codigo,
                    TipoMovimiento = m.Tipo,
                    Descripcion = m.Descripcion,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados motivos de inventario en CatMotivoMovimientoInventario.");
    }

    private async Task SeedCatConceptoMovimientoCajaAsync(CancellationToken ct)
    {
        var conceptos = new[]
        {
            // Egresos
            new { Tipo = "Egreso", Descripcion = "Compra de Insumos" },
            new { Tipo = "Egreso", Descripcion = "Pago a Proveedor" },
            new { Tipo = "Egreso", Descripcion = "Retiro a Caja Fuerte (Drop)" },
            new { Tipo = "Egreso", Descripcion = "Gasto Operativo" },
            new { Tipo = "Egreso", Descripcion = "Otro Egreso" },

            // Ingresos
            new { Tipo = "Ingreso", Descripcion = "Fondo Adicional / Cambio" },
            new { Tipo = "Ingreso", Descripcion = "Ingreso Extraordinario" },
            new { Tipo = "Ingreso", Descripcion = "Otro Ingreso" }
        };

        foreach (var c in conceptos)
        {
            var exists = await _db.Set<CatConceptoMovimientoCaja>().AnyAsync(x => x.Descripcion == c.Descripcion && x.Tipo == c.Tipo, ct);
            if (!exists)
            {
                _db.Add(new CatConceptoMovimientoCaja
                {
                    Tipo = c.Tipo,
                    Descripcion = c.Descripcion,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados conceptos de caja en CatConceptoMovimientoCaja.");
    }

    private async Task SeedCatMotivoCancelacionPedidoAsync(CancellationToken ct)
    {
        var motivos = new[]
        {
            "Cliente canceló en app / llamada",
            "Dirección incorrecta / Fuera de zona",
            "Cliente ausente en domicilio",
            "Pedido equivocado / Producto en mal estado",
            "Repartidor accidentado / no disponible",
            "Otro motivo de anulación"
        };

        foreach (var m in motivos)
        {
            var exists = await _db.Set<CatMotivoCancelacionPedido>().AnyAsync(x => x.Descripcion == m, ct);
            if (!exists)
            {
                _db.Add(new CatMotivoCancelacionPedido
                {
                    Descripcion = m,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados motivos de cancelación en CatMotivoCancelacionPedido.");
    }

    /// <summary>
    /// Spec 024: los 4 motivos obligatorios (sección 2.2 del spec) para autorizar con PIN de
    /// supervisor la cancelación de un platillo ya enviado a cocina.
    /// </summary>
    private async Task SeedCatMotivoCancelacionAsync(CancellationToken ct)
    {
        var motivos = new[]
        {
            "Error de captura del mesero",
            "Platillo devuelto por el comensal",
            "Mesa se retiró sin consumir",
            "Cortesía de la casa autorizada"
        };

        foreach (var m in motivos)
        {
            var exists = await _db.Set<CatMotivoCancelacion>().AnyAsync(x => x.Descripcion == m, ct);
            if (!exists)
            {
                _db.Add(new CatMotivoCancelacion
                {
                    Descripcion = m,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados motivos base en CatMotivoCancelacion.");
    }

    private async Task SeedCatCanalVentaAsync(CancellationToken ct)
    {
        var canales = new[]
        {
            new { Codigo = "COMEDOR", Descripcion = "Comedor Local", EsDelivery = false },
            new { Codigo = "LLEVAR", Descripcion = "Para Llevar (Mostrador)", EsDelivery = false },
            new { Codigo = "PROPIO", Descripcion = "Delivery Propio", EsDelivery = true },
            new { Codigo = "UBER_EATS", Descripcion = "Uber Eats", EsDelivery = true },
            new { Codigo = "RAPPI", Descripcion = "Rappi", EsDelivery = true },
            new { Codigo = "DIDI_FOOD", Descripcion = "Didi Food", EsDelivery = true }
        };

        foreach (var c in canales)
        {
            var exists = await _db.Set<CatCanalVenta>().AnyAsync(x => x.Codigo == c.Codigo, ct);
            if (!exists)
            {
                _db.Add(new CatCanalVenta
                {
                    Codigo = c.Codigo,
                    Descripcion = c.Descripcion,
                    EsDelivery = c.EsDelivery,
                    IsActive = true,
                    CreatedBy = "seed"
                });
            }
        }
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Seed: asegurados canales de venta en CatCanalVenta.");
    }
}
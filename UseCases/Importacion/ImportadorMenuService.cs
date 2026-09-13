using Common;
using Domain.Entities;
using DTO.Importacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Persistence.Context;

namespace UseCases.Importacion;

public interface IImportadorMenuService
{
    Task<Response<ImportarMenuPreviewResponseDTO>> PreviewAsync(Stream archivo, string nombreArchivo, string modo, int sucursalId);
    Task<Response<ConfirmarImportacionMenuResponseDTO>> ConfirmarAsync(Guid tokenPreview, string modo, int sucursalId);
    byte[] GenerarPlantilla();
}

/// <summary>
/// Spec 022: orquesta el importador masivo de menú (preview de calidad ➔ confirmación atómica).
/// Reutiliza las entidades de catálogo existentes (Menu, CategoriaMenu, Producto, VarianteProducto,
/// Precio, GrupoModificador, OpcionModificador, CatEstacionesCocina) en lugar de duplicar el modelo.
/// </summary>
public class ImportadorMenuService : IImportadorMenuService
{
    // Spec 022 Sección 2.2 Paso 2: el token de preview vive 15 minutos en memoria; no vale la
    // pena persistir en BD un análisis desechable que solo sirve para confirmar o descartar.
    private static readonly TimeSpan VigenciaCachePreview = TimeSpan.FromMinutes(15);
    private const string PrefijoCache = "importacion-menu:";

    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public ImportadorMenuService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public byte[] GenerarPlantilla() => PlantillaMenuGeneratorService.Generar();

    public async Task<Response<ImportarMenuPreviewResponseDTO>> PreviewAsync(Stream archivo, string nombreArchivo, string modo, int sucursalId)
    {
        var response = new Response<ImportarMenuPreviewResponseDTO>();

        if (modo != ModosImportacionMenu.Merge && modo != ModosImportacionMenu.Overwrite)
        {
            response.isSuccess = false;
            response.Message = $"Modo de importación inválido: '{modo}'. Usa 'Merge' u 'Overwrite'.";
            return response;
        }

        var sucursal = await _context.Sucursales.FindAsync(sucursalId);
        if (sucursal == null)
        {
            response.isSuccess = false;
            response.Message = "La sucursal especificada no existe.";
            return response;
        }

        byte[] contenido;
        using (var ms = new MemoryStream())
        {
            await archivo.CopyToAsync(ms);
            contenido = ms.ToArray();
        }

        var parseo = MenuExcelParserService.Parse(contenido, nombreArchivo);

        // Comparación contra la BD: categorías/productos nuevos vs. existentes de la sucursal.
        var menu = await _context.Menus
            .Include(m => m.Categorias)
            .ThenInclude(c => c.Productos)
            .FirstOrDefaultAsync(m => m.IdSucursal == sucursalId);

        var categoriasExistentes = menu?.Categorias
            .Select(c => c.Nombre ?? string.Empty)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var productosExistentes = menu?.Categorias
            .SelectMany(c => c.Productos)
            .Select(p => p.Nombre ?? string.Empty)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var categoriasEnArchivo = parseo.Productos.Select(p => p.Categoria).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        int categoriasNuevas = categoriasEnArchivo.Count(c => !categoriasExistentes.Contains(c));
        int categoriasYaExistentes = categoriasEnArchivo.Count - categoriasNuevas;

        var productosEnArchivo = parseo.Productos.Select(p => p.Producto).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        int productosActualizar = productosEnArchivo.Count(p => productosExistentes.Contains(p));
        int productosNuevos = productosEnArchivo.Count - productosActualizar;

        int gruposDetectados = parseo.Modificadores
            .Select(m => m.Grupo)
            .Concat(parseo.Productos.SelectMany(p => p.GruposModificadores))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        var token = Guid.NewGuid();
        var cacheEntry = new ImportacionMenuCacheEntry
        {
            SucursalId = sucursalId,
            Modo = modo,
            Datos = parseo
        };
        _cache.Set(PrefijoCache + token, cacheEntry, VigenciaCachePreview);

        response.Data = new ImportarMenuPreviewResponseDTO
        {
            TokenPreview = token,
            TotalRenglones = parseo.Productos.Count + parseo.Modificadores.Count,
            CategoriasNuevas = categoriasNuevas,
            CategoriasExistentes = categoriasYaExistentes,
            ProductosNuevos = productosNuevos,
            ProductosActualizar = productosActualizar,
            GruposModificadoresDetectados = gruposDetectados,
            OpcionesModificadoresDetectadas = parseo.Modificadores.Count,
            EsValido = parseo.Errores.Count == 0,
            Errores = parseo.Errores,
            Advertencias = parseo.Advertencias
        };
        response.isSuccess = true;
        response.Message = response.Data.EsValido
            ? "Archivo analizado correctamente."
            : "El archivo contiene errores que deben corregirse antes de confirmar la importación.";
        return response;
    }

    public async Task<Response<ConfirmarImportacionMenuResponseDTO>> ConfirmarAsync(Guid tokenPreview, string modo, int sucursalId)
    {
        var response = new Response<ConfirmarImportacionMenuResponseDTO>();

        if (!_cache.TryGetValue(PrefijoCache + tokenPreview, out ImportacionMenuCacheEntry? cacheEntry) || cacheEntry == null)
        {
            response.isSuccess = false;
            response.Message = "La vista previa expiró (15 min) o el token no existe. Vuelve a subir el archivo.";
            return response;
        }

        if (cacheEntry.SucursalId != sucursalId)
        {
            response.isSuccess = false;
            response.Message = "El token de previsualización no corresponde a la sucursal indicada.";
            return response;
        }

        if (cacheEntry.Datos.Errores.Count > 0)
        {
            response.isSuccess = false;
            response.Message = "No se puede confirmar una importación cuyo archivo contiene errores. Corrígelos y vuelve a generar el preview.";
            return response;
        }

        if (modo != ModosImportacionMenu.Merge && modo != ModosImportacionMenu.Overwrite)
        {
            response.isSuccess = false;
            response.Message = $"Modo de importación inválido: '{modo}'. Usa 'Merge' u 'Overwrite'.";
            return response;
        }

        var resumen = new ConfirmarImportacionMenuResponseDTO();

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var sucursal = await _context.Sucursales.FindAsync(sucursalId);
            if (sucursal == null)
            {
                response.isSuccess = false;
                response.Message = "La sucursal especificada no existe.";
                return response;
            }

            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.IdSucursal == sucursalId);
            if (menu == null)
            {
                menu = new Menu { IdSucursal = sucursalId, Nombre = "Menú Principal", IsActive = true };
                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();
            }

            // Modo Overwrite: soft-delete de los productos actuales de la sucursal. Nunca se
            // borran físicamente (protege la integridad referencial con ventas históricas, PedidoDetalle, etc.).
            if (modo == ModosImportacionMenu.Overwrite)
            {
                var productosActuales = await _context.Productos
                    .Where(p => p.IdMenu == menu.Id && p.Activo)
                    .ToListAsync();

                foreach (var p in productosActuales)
                {
                    p.Activo = false;
                    p.IsActive = false;
                }
                await _context.SaveChangesAsync();
            }

            var idImpuestoDefault = await ResolverCatalogoDefaultAsync<CatImpuesto>("IVA 16%");
            var idMonedaDefault = await ResolverCatalogoDefaultAsync<CatMoneda>("MXN");

            var categoriasCache = await _context.CategoriaMenus
                .Where(c => c.IdMenu == menu.Id)
                .ToDictionaryAsync(c => c.Nombre ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            var estacionesCache = await _context.CatEstacionesCocina
                .ToDictionaryAsync(e => e.Descripcion ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            var productosCache = await _context.Productos
                .Where(p => p.IdMenu == menu.Id)
                .Include(p => p.Variantes).ThenInclude(v => v.Precios)
                .Include(p => p.GruposModificador).ThenInclude(g => g.Opciones)
                .ToDictionaryAsync(p => p.Nombre ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            foreach (var fila in cacheEntry.Datos.Productos)
            {
                // 1. Resolver/crear categoría
                if (!categoriasCache.TryGetValue(fila.Categoria, out var categoria))
                {
                    categoria = new CategoriaMenu { IdMenu = menu.Id, Nombre = fila.Categoria, IsActive = true };
                    _context.CategoriaMenus.Add(categoria);
                    await _context.SaveChangesAsync();
                    categoriasCache[fila.Categoria] = categoria;
                    resumen.CategoriasCreadas++;
                }

                // 2. Resolver/crear estación de cocina (catálogo tipado, reutilizado tal cual)
                int? idEstacion = null;
                if (!string.IsNullOrWhiteSpace(fila.EstacionCocina))
                {
                    if (!estacionesCache.TryGetValue(fila.EstacionCocina, out var estacion))
                    {
                        estacion = new CatEstacionesCocina { Descripcion = fila.EstacionCocina, IsActive = true };
                        _context.CatEstacionesCocina.Add(estacion);
                        await _context.SaveChangesAsync();
                        estacionesCache[fila.EstacionCocina] = estacion;
                    }
                    idEstacion = estacion.Id;
                }

                // 3. Merge/crear Producto
                if (productosCache.TryGetValue(fila.Producto, out var producto))
                {
                    producto.IdCategoria = categoria.Id;
                    producto.Descripcion = fila.Descripcion;
                    producto.IdEstacionCocina = idEstacion;
                    producto.Codigo = fila.CodigoInterno ?? producto.Codigo;
                    producto.Activo = true;
                    producto.IsActive = true;

                    var variante = producto.Variantes.FirstOrDefault(v => v.EsDefault) ?? producto.Variantes.FirstOrDefault();
                    if (variante == null)
                    {
                        variante = new VarianteProducto { IdProducto = producto.Id, Nombre = "Regular", EsDefault = true, IsActive = true };
                        _context.VarianteProductos.Add(variante);
                        await _context.SaveChangesAsync();
                    }

                    var precio = variante.Precios.FirstOrDefault();
                    if (precio == null)
                    {
                        precio = new Precio
                        {
                            IdVariante = variante.Id,
                            Monto = fila.PrecioVenta,
                            Moneda = "MXN",
                            IdImpuesto = idImpuestoDefault,
                            IdMoneda = idMonedaDefault,
                            IsActive = true
                        };
                        _context.Precios.Add(precio);
                    }
                    else
                    {
                        precio.Monto = fila.PrecioVenta;
                    }

                    resumen.ProductosActualizados++;
                }
                else
                {
                    producto = new Producto
                    {
                        IdMenu = menu.Id,
                        IdCategoria = categoria.Id,
                        Codigo = fila.CodigoInterno,
                        Nombre = fila.Producto,
                        Descripcion = fila.Descripcion,
                        Activo = true,
                        IsActive = true,
                        IdEstacionCocina = idEstacion
                    };
                    _context.Productos.Add(producto);
                    await _context.SaveChangesAsync();

                    var variante = new VarianteProducto { IdProducto = producto.Id, Nombre = "Regular", EsDefault = true, IsActive = true };
                    _context.VarianteProductos.Add(variante);
                    await _context.SaveChangesAsync();

                    var precio = new Precio
                    {
                        IdVariante = variante.Id,
                        Monto = fila.PrecioVenta,
                        Moneda = "MXN",
                        IdImpuesto = idImpuestoDefault,
                        IdMoneda = idMonedaDefault,
                        IsActive = true
                    };
                    _context.Precios.Add(precio);

                    producto.Variantes = new List<VarianteProducto> { variante };
                    productosCache[fila.Producto] = producto;
                    resumen.ProductosCreados++;
                }

                await _context.SaveChangesAsync();

                // 4. Grupos y opciones de modificadores referenciados por el producto
                foreach (var nombreGrupo in fila.GruposModificadores)
                {
                    var filasGrupo = cacheEntry.Datos.Modificadores
                        .Where(m => string.Equals(m.Grupo, nombreGrupo, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (filasGrupo.Count == 0)
                        continue; // referenciado en Menú pero sin definición en la hoja de Modificadores

                    var yaExiste = producto.GruposModificador.Any(g => string.Equals(g.Nombre, nombreGrupo, StringComparison.OrdinalIgnoreCase));
                    if (yaExiste)
                        continue;

                    var primero = filasGrupo[0];
                    var grupoEntidad = new GrupoModificador
                    {
                        IdProducto = producto.Id,
                        Nombre = nombreGrupo,
                        MinSeleccion = primero.EsObligatorio ? 1 : 0,
                        MaxSeleccion = primero.MaxSeleccion,
                        Obligatorio = primero.EsObligatorio,
                        IsActive = true
                    };
                    _context.GruposModificador.Add(grupoEntidad);
                    await _context.SaveChangesAsync();
                    resumen.GruposModificadoresCreados++;

                    foreach (var filaOpcion in filasGrupo)
                    {
                        _context.OpcionesModificador.Add(new OpcionModificador
                        {
                            IdGrupo = grupoEntidad.Id,
                            Nombre = filaOpcion.Opcion,
                            PrecioExtra = filaOpcion.PrecioExtra,
                            EsDefault = false,
                            IsActive = true
                        });
                        resumen.OpcionesModificadoresCreadas++;
                    }
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
            _cache.Remove(PrefijoCache + tokenPreview);

            response.Data = resumen;
            response.isSuccess = true;
            response.Message = "Importación de menú confirmada correctamente.";
            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al confirmar la importación: {ex.Message}";
            return response;
        }
    }

    /// <summary>
    /// Busca un registro de catálogo simple (Cat*) por Descripcion; si no existe (p.ej. entorno
    /// de pruebas sin seed), lo crea al vuelo para no bloquear la transacción.
    /// </summary>
    private async Task<int> ResolverCatalogoDefaultAsync<TEntity>(string descripcion)
        where TEntity : BaseAuditableEntity, ICatalogEntity, new()
    {
        var set = _context.Set<TEntity>();
        var existente = await set.FirstOrDefaultAsync(e => e.Descripcion == descripcion);
        if (existente != null)
            return existente.Id;

        var cualquiera = await set.FirstOrDefaultAsync();
        if (cualquiera != null)
            return cualquiera.Id;

        var nuevo = new TEntity { Descripcion = descripcion, IsActive = true };
        set.Add(nuevo);
        await _context.SaveChangesAsync();
        return nuevo.Id;
    }
}

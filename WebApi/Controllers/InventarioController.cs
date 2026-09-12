using System.Security.Claims;
using Common;
using Domain.Entities;
using DTO.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InventarioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InventarioController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst("idUsuario") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out var id))
            return id;
        return null;
    }

    #region Catálogos Base e Inicialización

    [HttpGet("CatalogosBase")]
    public async Task<ActionResult<Response<CatalogosBaseInventarioDTO>>> GetCatalogosBase([FromQuery] int? idSucursal = null)
    {
        var response = new Response<CatalogosBaseInventarioDTO>();

        try
        {
            await SeedCatalogosSiVacioAsync();

            var unidades = await _context.UnidadesMedida
                .Where(u => u.IsActive)
                .OrderBy(u => u.Tipo)
                .ThenBy(u => u.Nombre)
                .Select(u => new UnidadMedidaDTO
                {
                    Id = u.Id,
                    Codigo = u.Codigo,
                    Nombre = u.Nombre,
                    Tipo = u.Tipo,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            var categorias = await _context.CategoriasInsumo
                .Where(c => c.IsActive)
                .OrderBy(c => c.Nombre)
                .Select(c => new CategoriaInsumoDTO
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            var almacenesQuery = _context.Almacenes
                .Include(a => a.Sucursal)
                .Where(a => a.IsActive);

            if (idSucursal.HasValue && idSucursal.Value > 0)
            {
                almacenesQuery = almacenesQuery.Where(a => a.IdSucursal == idSucursal.Value);
            }

            var almacenes = await almacenesQuery
                .OrderBy(a => a.IdSucursal)
                .ThenByDescending(a => a.EsPrincipal)
                .ThenBy(a => a.Nombre)
                .Select(a => new AlmacenDTO
                {
                    Id = a.Id,
                    IdSucursal = a.IdSucursal,
                    SucursalNombre = a.Sucursal != null ? a.Sucursal.Nombre : "Sin Sucursal",
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    TipoAlmacen = a.TipoAlmacen,
                    EsPrincipal = a.EsPrincipal,
                    IsActive = a.IsActive
                })
                .ToListAsync();

            var sucursales = await _context.Sucursales
                .Where(s => s.IsActive)
                .OrderBy(s => s.Nombre)
                .Select(s => new SucursalSimpleDTO
                {
                    Id = s.Id,
                    Codigo = $"SUC-{s.Id:D3}",
                    Nombre = s.Nombre ?? string.Empty
                })
                .ToListAsync();

            var tiposAlmacen = await _context.CatTiposAlmacen
                .Where(t => t.IsActive)
                .OrderBy(t => t.Id)
                .Select(t => new TipoAlmacenDTO
                {
                    Id = t.Id,
                    Codigo = t.Codigo ?? t.Id.ToString(),
                    Descripcion = t.Descripcion
                })
                .ToListAsync();

            var motivosMovimiento = await _context.CatMotivosMovimientoInventario
                .Where(m => m.IsActive)
                .OrderBy(m => m.TipoMovimiento)
                .ThenBy(m => m.Id)
                .Select(m => new MotivoInventarioDTO
                {
                    Id = m.Id,
                    Codigo = m.Codigo ?? m.Id.ToString(),
                    TipoMovimiento = m.TipoMovimiento,
                    Descripcion = m.Descripcion
                })
                .ToListAsync();

            response.Data = new CatalogosBaseInventarioDTO
            {
                UnidadesMedida = unidades,
                Categorias = categorias,
                Almacenes = almacenes,
                Sucursales = sucursales,
                TiposAlmacen = tiposAlmacen,
                MotivosMovimiento = motivosMovimiento
            };
            response.isSuccess = true;
            response.Message = "Catálogos base obtenidos con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al obtener catálogos base: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    private async Task SeedCatalogosSiVacioAsync()
    {
        if (!await _context.UnidadesMedida.AnyAsync())
        {
            _context.UnidadesMedida.AddRange(
                new UnidadMedida { Codigo = "KG", Nombre = "Kilogramo", Tipo = "Masa", IsActive = true },
                new UnidadMedida { Codigo = "G", Nombre = "Gramo", Tipo = "Masa", IsActive = true },
                new UnidadMedida { Codigo = "L", Nombre = "Litro", Tipo = "Volumen", IsActive = true },
                new UnidadMedida { Codigo = "ML", Nombre = "Mililitro", Tipo = "Volumen", IsActive = true },
                new UnidadMedida { Codigo = "PZA", Nombre = "Pieza", Tipo = "Conteo", IsActive = true },
                new UnidadMedida { Codigo = "CJ", Nombre = "Caja", Tipo = "Conteo", IsActive = true },
                new UnidadMedida { Codigo = "PQ", Nombre = "Paquete", Tipo = "Conteo", IsActive = true }
            );
            await _context.SaveChangesAsync();
        }

        if (!await _context.CategoriasInsumo.AnyAsync())
        {
            _context.CategoriasInsumo.AddRange(
                new CategoriaInsumo { Codigo = "PROT", Nombre = "Proteínas / Carnes", Descripcion = "Res, pollo, cerdo, pescados y mariscos", IsActive = true },
                new CategoriaInsumo { Codigo = "LACT", Nombre = "Lácteos y Quesos", Descripcion = "Quesos, leche, cremas, mantequilla", IsActive = true },
                new CategoriaInsumo { Codigo = "VERD", Nombre = "Frutas y Verduras", Descripcion = "Vegetales frescos, frutas, hierbas", IsActive = true },
                new CategoriaInsumo { Codigo = "ABAR", Nombre = "Abarrotes y Especias", Descripcion = "Harinas, aceites, salsas, condimentos, granos", IsActive = true },
                new CategoriaInsumo { Codigo = "BEBI", Nombre = "Bebidas y Licores", Descripcion = "Refrescos, jugos, destilados, vinos, café", IsActive = true },
                new CategoriaInsumo { Codigo = "DESE", Nombre = "Desechables y Empaques", Descripcion = "Vasos, servilletas, contenedores, bolsas", IsActive = true }
            );
            await _context.SaveChangesAsync();
        }

        if (!await _context.Almacenes.AnyAsync())
        {
            var primeraSucursal = await _context.Sucursales.FirstOrDefaultAsync(s => s.IsActive);
            if (primeraSucursal != null)
            {
                _context.Almacenes.AddRange(
                    new Almacen { IdSucursal = primeraSucursal.Id, Codigo = "ALM-GEN", Nombre = "Almacén General", TipoAlmacen = "General", EsPrincipal = true, IsActive = true },
                    new Almacen { IdSucursal = primeraSucursal.Id, Codigo = "ALM-COC", Nombre = "Cocina Principal", TipoAlmacen = "Cocina", EsPrincipal = false, IsActive = true },
                    new Almacen { IdSucursal = primeraSucursal.Id, Codigo = "ALM-BAR", Nombre = "Barra y Bebidas", TipoAlmacen = "Barra", EsPrincipal = false, IsActive = true }
                );
                await _context.SaveChangesAsync();
            }
        }

        if (!await _context.CatTiposAlmacen.AnyAsync())
        {
            _context.CatTiposAlmacen.AddRange(
                new CatTipoAlmacen { Codigo = "GENERAL", Descripcion = "General / Bodega Central", IsActive = true },
                new CatTipoAlmacen { Codigo = "COCINA", Descripcion = "Cocina Principal", IsActive = true },
                new CatTipoAlmacen { Codigo = "BARRA", Descripcion = "Barra / Bebidas", IsActive = true },
                new CatTipoAlmacen { Codigo = "PRODUCCION", Descripcion = "Producción / Subrecetas", IsActive = true }
            );
            await _context.SaveChangesAsync();
        }

        if (!await _context.CatMotivosMovimientoInventario.AnyAsync())
        {
            _context.CatMotivosMovimientoInventario.AddRange(
                new CatMotivoMovimientoInventario { Codigo = "CompraEmergencia", TipoMovimiento = "EntradaManual", Descripcion = "Compra de Emergencia / Caja Chica", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "Donacion", TipoMovimiento = "EntradaManual", Descripcion = "Donación / Bonificación", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "AjusteCargaManual", TipoMovimiento = "EntradaManual", Descripcion = "Carga Manual de Stock", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "OtroEntrada", TipoMovimiento = "EntradaManual", Descripcion = "Otro Motivo de Entrada", IsActive = true },

                new CatMotivoMovimientoInventario { Codigo = "Caducidad", TipoMovimiento = "SalidaMerma", Descripcion = "Caducidad / Vencimiento", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "Descomposicion", TipoMovimiento = "SalidaMerma", Descripcion = "Descomposición / Mal Estado", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "CaidaAccidente", TipoMovimiento = "SalidaMerma", Descripcion = "Caída o Accidente en Cocina", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "DegustacionCortesia", TipoMovimiento = "SalidaMerma", Descripcion = "Degustación / Cortesía", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "MermaOperativa", TipoMovimiento = "SalidaMerma", Descripcion = "Merma Operativa de Preparación", IsActive = true },

                new CatMotivoMovimientoInventario { Codigo = "AjusteManual", TipoMovimiento = "AjusteInventario", Descripcion = "Corrección de Conteo Físico", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "MuestraCalidad", TipoMovimiento = "AjusteInventario", Descripcion = "Muestra de Calidad", IsActive = true },
                new CatMotivoMovimientoInventario { Codigo = "OtroAjuste", TipoMovimiento = "AjusteInventario", Descripcion = "Otro Ajuste", IsActive = true }
            );
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Insumos

    [HttpGet("Insumos")]
    public async Task<ActionResult<Response<IEnumerable<InsumoDTO>>>> GetInsumos(
        [FromQuery] int? idCategoria = null,
        [FromQuery] bool? esCritico = null,
        [FromQuery] string? search = null)
    {
        var response = new Response<IEnumerable<InsumoDTO>>();

        try
        {
            var query = _context.Insumos
                .Include(i => i.CategoriaInsumo)
                .Include(i => i.UnidadMedidaBase)
                .Include(i => i.Existencias)
                .Where(i => i.IsActive);

            if (idCategoria.HasValue && idCategoria.Value > 0)
                query = query.Where(i => i.IdCategoriaInsumo == idCategoria.Value);

            if (esCritico.HasValue)
                query = query.Where(i => i.EsCritico == esCritico.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(i => i.Nombre.ToLower().Contains(s) || i.Codigo.ToLower().Contains(s));
            }

            var insumos = await query
                .OrderBy(i => i.CategoriaInsumo != null ? i.CategoriaInsumo.Nombre : "")
                .ThenBy(i => i.Nombre)
                .Select(i => new InsumoDTO
                {
                    Id = i.Id,
                    Codigo = i.Codigo,
                    Nombre = i.Nombre,
                    IdCategoriaInsumo = i.IdCategoriaInsumo,
                    CategoriaNombre = i.CategoriaInsumo != null ? i.CategoriaInsumo.Nombre : "Sin categoría",
                    IdUnidadMedidaBase = i.IdUnidadMedidaBase,
                    UnidadMedidaCodigo = i.UnidadMedidaBase != null ? i.UnidadMedidaBase.Codigo : "N/A",
                    UnidadMedidaNombre = i.UnidadMedidaBase != null ? i.UnidadMedidaBase.Nombre : "N/A",
                    CostoPromedio = i.CostoPromedio,
                    UltimoCosto = i.UltimoCosto,
                    StockMinimo = i.StockMinimo,
                    StockMaximo = i.StockMaximo,
                    EsCritico = i.EsCritico,
                    IsActive = i.IsActive,
                    StockTotalConsolidado = i.Existencias.Sum(e => e.StockActual),
                    ValorizadoConsolidado = i.Existencias.Sum(e => e.StockActual) * i.CostoPromedio
                })
                .ToListAsync();

            response.Data = insumos;
            response.isSuccess = true;
            response.Message = "Insumos consultados con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al consultar insumos: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPost("Insumos")]
    public async Task<ActionResult<Response<InsumoDTO>>> CrearInsumo([FromBody] CrearInsumoDTO dto)
    {
        var response = new Response<InsumoDTO>();

        try
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                response.isSuccess = false;
                response.Message = "El nombre del insumo es obligatorio.";
                return BadRequest(response);
            }

            var codigo = string.IsNullOrWhiteSpace(dto.Codigo)
                ? "INS-" + DateTime.UtcNow.ToString("yyMMddHHmmss")
                : dto.Codigo.Trim().ToUpper();

            if (await _context.Insumos.AnyAsync(i => i.Codigo == codigo && i.IsActive))
            {
                response.isSuccess = false;
                response.Message = $"Ya existe un insumo activo con el código '{codigo}'.";
                return BadRequest(response);
            }

            var insumo = new Insumo
            {
                Codigo = codigo,
                Nombre = dto.Nombre.Trim(),
                IdCategoriaInsumo = dto.IdCategoriaInsumo,
                IdUnidadMedidaBase = dto.IdUnidadMedidaBase,
                CostoPromedio = dto.CostoInicial >= 0 ? dto.CostoInicial : 0,
                UltimoCosto = dto.CostoInicial >= 0 ? dto.CostoInicial : 0,
                StockMinimo = dto.StockMinimo >= 0 ? dto.StockMinimo : 0,
                StockMaximo = dto.StockMaximo >= 0 ? dto.StockMaximo : 0,
                EsCritico = dto.EsCritico,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "Sistema"
            };

            _context.Insumos.Add(insumo);
            await _context.SaveChangesAsync();

            // Si se especificó un stock inicial en algún almacén, registrar existencia y kardex
            if (dto.StockInicial > 0 && dto.IdAlmacenInicial.HasValue && dto.IdAlmacenInicial.Value > 0)
            {
                var existencia = new InventarioExistencia
                {
                    IdAlmacen = dto.IdAlmacenInicial.Value,
                    IdInsumo = insumo.Id,
                    StockActual = dto.StockInicial,
                    FechaUltimoMovimiento = DateTime.UtcNow,
                    IsActive = true
                };
                _context.InventarioExistencias.Add(existencia);

                var kardex = new KardexMovimiento
                {
                    IdAlmacen = dto.IdAlmacenInicial.Value,
                    IdInsumo = insumo.Id,
                    TipoMovimiento = "EntradaManual",
                    Submotivo = "InventarioInicial",
                    Cantidad = dto.StockInicial,
                    CostoUnitario = insumo.CostoPromedio,
                    CostoTotal = dto.StockInicial * insumo.CostoPromedio,
                    SaldoAnterior = 0,
                    SaldoNuevo = dto.StockInicial,
                    CostoPromedioResultante = insumo.CostoPromedio,
                    Observaciones = "Carga inicial de inventario",
                    IdUsuario = GetCurrentUserId(),
                    FechaHora = DateTime.UtcNow,
                    IsActive = true
                };
                _context.KardexMovimientos.Add(kardex);
                await _context.SaveChangesAsync();
            }

            var cat = await _context.CategoriasInsumo.FindAsync(insumo.IdCategoriaInsumo);
            var um = await _context.UnidadesMedida.FindAsync(insumo.IdUnidadMedidaBase);

            response.Data = new InsumoDTO
            {
                Id = insumo.Id,
                Codigo = insumo.Codigo,
                Nombre = insumo.Nombre,
                IdCategoriaInsumo = insumo.IdCategoriaInsumo,
                CategoriaNombre = cat != null ? cat.Nombre : "Sin categoría",
                IdUnidadMedidaBase = insumo.IdUnidadMedidaBase,
                UnidadMedidaCodigo = um != null ? um.Codigo : "N/A",
                UnidadMedidaNombre = um != null ? um.Nombre : "N/A",
                CostoPromedio = insumo.CostoPromedio,
                UltimoCosto = insumo.UltimoCosto,
                StockMinimo = insumo.StockMinimo,
                StockMaximo = insumo.StockMaximo,
                EsCritico = insumo.EsCritico,
                IsActive = insumo.IsActive,
                StockTotalConsolidado = dto.StockInicial > 0 ? dto.StockInicial : 0,
                ValorizadoConsolidado = dto.StockInicial > 0 ? (dto.StockInicial * insumo.CostoPromedio) : 0
            };
            response.isSuccess = true;
            response.Message = "Insumo creado con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al crear insumo: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPut("Insumos/{id}")]
    public async Task<ActionResult<Response<bool>>> ActualizarInsumo(int id, [FromBody] ActualizarInsumoDTO dto)
    {
        var response = new Response<bool>();

        try
        {
            var insumo = await _context.Insumos.FindAsync(id);
            if (insumo == null)
            {
                response.isSuccess = false;
                response.Message = "El insumo especificado no existe.";
                return NotFound(response);
            }

            if (!string.IsNullOrWhiteSpace(dto.Codigo) && dto.Codigo.Trim().ToUpper() != insumo.Codigo)
            {
                var cod = dto.Codigo.Trim().ToUpper();
                if (await _context.Insumos.AnyAsync(i => i.Id != id && i.Codigo == cod && i.IsActive))
                {
                    response.isSuccess = false;
                    response.Message = $"El código '{cod}' ya está en uso por otro insumo.";
                    return BadRequest(response);
                }
                insumo.Codigo = cod;
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                insumo.Nombre = dto.Nombre.Trim();

            insumo.IdCategoriaInsumo = dto.IdCategoriaInsumo;
            insumo.IdUnidadMedidaBase = dto.IdUnidadMedidaBase;
            insumo.StockMinimo = dto.StockMinimo;
            insumo.StockMaximo = dto.StockMaximo;
            insumo.EsCritico = dto.EsCritico;
            insumo.IsActive = dto.IsActive;
            insumo.UpdatedAt = DateTime.UtcNow;
            insumo.UpdatedBy = User.Identity?.Name ?? "Sistema";

            await _context.SaveChangesAsync();

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Insumo actualizado con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al actualizar insumo: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpDelete("Insumos/{id}")]
    public async Task<ActionResult<Response<bool>>> EliminarInsumo(int id)
    {
        var response = new Response<bool>();

        try
        {
            var insumo = await _context.Insumos.FindAsync(id);
            if (insumo == null)
            {
                response.isSuccess = false;
                response.Message = "El insumo especificado no existe.";
                return NotFound(response);
            }

            insumo.IsActive = false;
            insumo.UpdatedAt = DateTime.UtcNow;
            insumo.UpdatedBy = User.Identity?.Name ?? "Sistema";

            await _context.SaveChangesAsync();

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Insumo desactivado con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al eliminar insumo: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Almacenes

    [HttpGet("Almacenes")]
    public async Task<ActionResult<Response<IEnumerable<AlmacenDTO>>>> GetAlmacenes([FromQuery] int? idSucursal = null)
    {
        var response = new Response<IEnumerable<AlmacenDTO>>();

        try
        {
            var query = _context.Almacenes
                .Include(a => a.Sucursal)
                .Include(a => a.Existencias)
                    .ThenInclude(e => e.Insumo)
                .Where(a => a.IsActive);

            if (idSucursal.HasValue && idSucursal.Value > 0)
                query = query.Where(a => a.IdSucursal == idSucursal.Value);

            var list = await query
                .OrderBy(a => a.IdSucursal)
                .ThenByDescending(a => a.EsPrincipal)
                .ThenBy(a => a.Nombre)
                .Select(a => new AlmacenDTO
                {
                    Id = a.Id,
                    IdSucursal = a.IdSucursal,
                    SucursalNombre = a.Sucursal != null ? a.Sucursal.Nombre : "Sin Sucursal",
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    TipoAlmacen = a.TipoAlmacen,
                    EsPrincipal = a.EsPrincipal,
                    IsActive = a.IsActive,
                    TotalInsumos = a.Existencias.Count(e => e.Insumo != null && e.Insumo.IsActive && e.StockActual > 0),
                    ValorizadoTotal = a.Existencias
                        .Where(e => e.Insumo != null && e.Insumo.IsActive)
                        .Sum(e => e.StockActual * e.Insumo!.CostoPromedio)
                })
                .ToListAsync();

            response.Data = list;
            response.isSuccess = true;
            response.Message = "Almacenes consultados con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al consultar almacenes: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPost("Almacenes")]
    public async Task<ActionResult<Response<AlmacenDTO>>> CrearAlmacen([FromBody] CrearAlmacenDTO dto)
    {
        var response = new Response<AlmacenDTO>();

        try
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                response.isSuccess = false;
                response.Message = "El nombre del almacén es obligatorio.";
                return BadRequest(response);
            }

            if (dto.IdSucursal <= 0)
            {
                var primeraSuc = await _context.Sucursales.FirstOrDefaultAsync(s => s.IsActive);
                if (primeraSuc != null)
                {
                    dto.IdSucursal = primeraSuc.Id;
                }
                else
                {
                    response.isSuccess = false;
                    response.Message = "No se encontró ninguna sucursal activa para asociar el almacén.";
                    return BadRequest(response);
                }
            }

            var codigo = string.IsNullOrWhiteSpace(dto.Codigo)
                ? "ALM-" + DateTime.UtcNow.ToString("yyMMddHHmmss")
                : dto.Codigo.Trim().ToUpper();

            if (await _context.Almacenes.AnyAsync(a => a.Codigo == codigo && a.IsActive))
            {
                response.isSuccess = false;
                response.Message = $"El código '{codigo}' ya existe.";
                return BadRequest(response);
            }

            // Si se marca como principal, desmarcar los otros de la misma sucursal
            if (dto.EsPrincipal)
            {
                var principales = await _context.Almacenes
                    .Where(a => a.IdSucursal == dto.IdSucursal && a.EsPrincipal && a.IsActive)
                    .ToListAsync();
                foreach (var p in principales)
                {
                    p.EsPrincipal = false;
                }
            }

            var almacen = new Almacen
            {
                IdSucursal = dto.IdSucursal,
                Codigo = codigo,
                Nombre = dto.Nombre.Trim(),
                TipoAlmacen = string.IsNullOrWhiteSpace(dto.TipoAlmacen) ? "General" : dto.TipoAlmacen.Trim(),
                EsPrincipal = dto.EsPrincipal,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "Sistema"
            };

            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync();

            var sucursal = await _context.Sucursales.FindAsync(almacen.IdSucursal);

            response.Data = new AlmacenDTO
            {
                Id = almacen.Id,
                IdSucursal = almacen.IdSucursal,
                SucursalNombre = sucursal != null ? sucursal.Nombre : "Sin Sucursal",
                Codigo = almacen.Codigo,
                Nombre = almacen.Nombre,
                TipoAlmacen = almacen.TipoAlmacen,
                EsPrincipal = almacen.EsPrincipal,
                IsActive = almacen.IsActive,
                TotalInsumos = 0,
                ValorizadoTotal = 0
            };
            response.isSuccess = true;
            response.Message = "Almacén creado con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al crear almacén: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Existencias

    [HttpGet("Existencias")]
    public async Task<ActionResult<Response<IEnumerable<ExistenciaDTO>>>> GetExistencias(
        [FromQuery] int? idSucursal = null,
        [FromQuery] int? idAlmacen = null,
        [FromQuery] int? idCategoria = null,
        [FromQuery] bool? soloBajoStock = null,
        [FromQuery] string? search = null)
    {
        var response = new Response<IEnumerable<ExistenciaDTO>>();

        try
        {
            var query = _context.InventarioExistencias
                .Include(e => e.Almacen)
                    .ThenInclude(a => a!.Sucursal)
                .Include(e => e.Insumo)
                    .ThenInclude(i => i!.CategoriaInsumo)
                .Include(e => e.Insumo)
                    .ThenInclude(i => i!.UnidadMedidaBase)
                .Where(e => e.IsActive && e.Almacen != null && e.Almacen.IsActive && e.Insumo != null && e.Insumo.IsActive);

            if (idSucursal.HasValue && idSucursal.Value > 0)
                query = query.Where(e => e.Almacen!.IdSucursal == idSucursal.Value);

            if (idAlmacen.HasValue && idAlmacen.Value > 0)
                query = query.Where(e => e.IdAlmacen == idAlmacen.Value);

            if (idCategoria.HasValue && idCategoria.Value > 0)
                query = query.Where(e => e.Insumo!.IdCategoriaInsumo == idCategoria.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(e => e.Insumo!.Nombre.ToLower().Contains(s) || e.Insumo!.Codigo.ToLower().Contains(s));
            }

            var items = await query.ToListAsync();

            var result = items.Select(e =>
            {
                var estado = "Normal";
                if (e.StockActual <= 0) estado = "Agotado";
                else if (e.StockActual <= e.Insumo!.StockMinimo) estado = "Bajo";
                else if (e.Insumo!.StockMaximo > 0 && e.StockActual > e.Insumo!.StockMaximo) estado = "SobreInventario";

                return new ExistenciaDTO
                {
                    Id = e.Id,
                    IdAlmacen = e.IdAlmacen,
                    AlmacenNombre = e.Almacen?.Nombre ?? "Almacén",
                    IdSucursal = e.Almacen?.IdSucursal ?? 0,
                    SucursalNombre = e.Almacen?.Sucursal?.Nombre ?? "",
                    IdInsumo = e.IdInsumo,
                    InsumoCodigo = e.Insumo?.Codigo ?? "",
                    InsumoNombre = e.Insumo?.Nombre ?? "",
                    CategoriaNombre = e.Insumo?.CategoriaInsumo?.Nombre ?? "General",
                    UnidadMedidaCodigo = e.Insumo?.UnidadMedidaBase?.Codigo ?? "PZA",
                    StockActual = e.StockActual,
                    StockMinimo = e.Insumo?.StockMinimo ?? 0,
                    StockMaximo = e.Insumo?.StockMaximo ?? 0,
                    CostoPromedio = e.Insumo?.CostoPromedio ?? 0,
                    Valorizado = e.StockActual * (e.Insumo?.CostoPromedio ?? 0),
                    EsCritico = e.Insumo?.EsCritico ?? false,
                    EstadoStock = estado,
                    FechaUltimoMovimiento = e.FechaUltimoMovimiento
                };
            });

            if (soloBajoStock == true)
            {
                result = result.Where(r => r.EstadoStock == "Bajo" || r.EstadoStock == "Agotado");
            }

            response.Data = result.OrderBy(r => r.AlmacenNombre).ThenBy(r => r.InsumoNombre).ToList();
            response.isSuccess = true;
            response.Message = "Existencias obtenidas con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al obtener existencias: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Movimientos Manuales y Valuación CPP

    [HttpPost("Movimiento")]
    public async Task<ActionResult<Response<bool>>> RegistrarMovimiento([FromBody] MovimientoManualDTO dto)
    {
        var response = new Response<bool>();

        if (dto.Cantidad <= 0)
        {
            response.isSuccess = false;
            response.Message = "La cantidad del movimiento debe ser estrictamente mayor a 0.";
            return BadRequest(response);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var insumo = await _context.Insumos.FindAsync(dto.IdInsumo);
            if (insumo == null || !insumo.IsActive)
            {
                response.isSuccess = false;
                response.Message = "El insumo especificado no existe o está inactivo.";
                return BadRequest(response);
            }

            var almacen = await _context.Almacenes.FindAsync(dto.IdAlmacen);
            if (almacen == null || !almacen.IsActive)
            {
                response.isSuccess = false;
                response.Message = "El almacén especificado no existe o está inactivo.";
                return BadRequest(response);
            }

            var existencia = await _context.InventarioExistencias
                .FirstOrDefaultAsync(e => e.IdAlmacen == dto.IdAlmacen && e.IdInsumo == dto.IdInsumo);

            if (existencia == null)
            {
                existencia = new InventarioExistencia
                {
                    IdAlmacen = dto.IdAlmacen,
                    IdInsumo = dto.IdInsumo,
                    StockActual = 0,
                    FechaUltimoMovimiento = DateTime.UtcNow,
                    IsActive = true
                };
                _context.InventarioExistencias.Add(existencia);
            }

            decimal saldoAnterior = existencia.StockActual;
            decimal nuevoSaldo = saldoAnterior;
            decimal costoUnitario = 0;
            decimal costoTotal = 0;
            decimal nuevoCostoPromedio = insumo.CostoPromedio;

            var tipo = dto.TipoMovimiento.Trim();

            if (tipo == "EntradaManual")
            {
                costoUnitario = dto.CostoUnitario ?? insumo.CostoPromedio;
                costoTotal = dto.Cantidad * costoUnitario;
                nuevoSaldo = saldoAnterior + dto.Cantidad;

                // Valuación por Costo Promedio Ponderado (CPP)
                if (saldoAnterior <= 0)
                {
                    nuevoCostoPromedio = costoUnitario;
                }
                else
                {
                    decimal valorPrevio = saldoAnterior * insumo.CostoPromedio;
                    decimal valorEntrada = dto.Cantidad * costoUnitario;
                    nuevoCostoPromedio = Math.Round((valorPrevio + valorEntrada) / nuevoSaldo, 4);
                }

                insumo.CostoPromedio = nuevoCostoPromedio;
                insumo.UltimoCosto = costoUnitario;
            }
            else if (tipo == "SalidaMerma")
            {
                costoUnitario = insumo.CostoPromedio;
                costoTotal = dto.Cantidad * costoUnitario;
                nuevoSaldo = saldoAnterior - dto.Cantidad;
                // El costo promedio unitario se mantiene invariable en las salidas
                nuevoCostoPromedio = insumo.CostoPromedio;
            }
            else if (tipo == "AjusteInventario")
            {
                costoUnitario = insumo.CostoPromedio;
                costoTotal = dto.Cantidad * costoUnitario;
                nuevoSaldo = saldoAnterior + dto.Cantidad;
                nuevoCostoPromedio = insumo.CostoPromedio;
            }
            else
            {
                response.isSuccess = false;
                response.Message = $"Tipo de movimiento '{tipo}' no soportado.";
                return BadRequest(response);
            }

            existencia.StockActual = nuevoSaldo;
            existencia.FechaUltimoMovimiento = DateTime.UtcNow;

            var kardex = new KardexMovimiento
            {
                IdAlmacen = dto.IdAlmacen,
                IdInsumo = dto.IdInsumo,
                TipoMovimiento = tipo,
                Submotivo = dto.Submotivo,
                Cantidad = dto.Cantidad,
                CostoUnitario = costoUnitario,
                CostoTotal = costoTotal,
                SaldoAnterior = saldoAnterior,
                SaldoNuevo = nuevoSaldo,
                CostoPromedioResultante = nuevoCostoPromedio,
                DocumentoReferencia = dto.DocumentoReferencia,
                Observaciones = dto.Observaciones,
                IdUsuario = GetCurrentUserId(),
                FechaHora = DateTime.UtcNow,
                IsActive = true
            };

            _context.KardexMovimientos.Add(kardex);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Movimiento registrado y aplicado en el Kárdex exitosamente.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al registrar movimiento: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Kárdex

    [HttpGet("Kardex")]
    public async Task<ActionResult<Response<KardexReporteDTO>>> GetKardex(
        [FromQuery] int idInsumo,
        [FromQuery] int? idAlmacen = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        var response = new Response<KardexReporteDTO>();

        try
        {
            var insumo = await _context.Insumos
                .Include(i => i.UnidadMedidaBase)
                .FirstOrDefaultAsync(i => i.Id == idInsumo);

            if (insumo == null)
            {
                response.isSuccess = false;
                response.Message = "Insumo no encontrado.";
                return NotFound(response);
            }

            var query = _context.KardexMovimientos
                .Include(k => k.Almacen)
                .Include(k => k.Usuario)
                .Where(k => k.IdInsumo == idInsumo && k.IsActive);

            if (idAlmacen.HasValue && idAlmacen.Value > 0)
                query = query.Where(k => k.IdAlmacen == idAlmacen.Value);

            DateTime dtInicio = fechaInicio ?? DateTime.UtcNow.AddMonths(-1);
            DateTime dtFin = fechaFin ?? DateTime.UtcNow.AddDays(1);

            // Calcular saldo inicial antes de dtInicio
            var movimientosPrevios = await query
                .Where(k => k.FechaHora < dtInicio)
                .OrderBy(k => k.FechaHora)
                .ToListAsync();

            decimal saldoInicial = movimientosPrevios.Any()
                ? movimientosPrevios.Last().SaldoNuevo
                : 0;

            // Movimientos en el rango
            var movimientosRango = await query
                .Where(k => k.FechaHora >= dtInicio && k.FechaHora <= dtFin)
                .OrderBy(k => k.FechaHora)
                .Select(k => new KardexMovimientoItemDTO
                {
                    Id = k.Id,
                    IdAlmacen = k.IdAlmacen,
                    AlmacenNombre = k.Almacen != null ? k.Almacen.Nombre : "Almacén",
                    IdInsumo = k.IdInsumo,
                    InsumoCodigo = insumo.Codigo,
                    InsumoNombre = insumo.Nombre,
                    UnidadMedidaCodigo = insumo.UnidadMedidaBase != null ? insumo.UnidadMedidaBase.Codigo : "N/A",
                    TipoMovimiento = k.TipoMovimiento,
                    Submotivo = k.Submotivo,
                    Cantidad = k.Cantidad,
                    CostoUnitario = k.CostoUnitario,
                    CostoTotal = k.CostoTotal,
                    SaldoAnterior = k.SaldoAnterior,
                    SaldoNuevo = k.SaldoNuevo,
                    CostoPromedioResultante = k.CostoPromedioResultante,
                    DocumentoReferencia = k.DocumentoReferencia,
                    Observaciones = k.Observaciones,
                    UsuarioNombre = k.Usuario != null ? k.Usuario.NombreCompleto : "Sistema",
                    FechaHora = k.FechaHora
                })
                .ToListAsync();

            decimal totalEntradas = movimientosRango
                .Where(m => m.TipoMovimiento == "EntradaManual" || m.TipoMovimiento == "TraspasoEntrada" || (m.TipoMovimiento == "AjusteInventario" && m.SaldoNuevo >= m.SaldoAnterior))
                .Sum(m => m.Cantidad);

            decimal totalSalidas = movimientosRango
                .Where(m => m.TipoMovimiento == "SalidaMerma" || m.TipoMovimiento == "TraspasoSalida" || m.TipoMovimiento == "ConsumoVenta" || (m.TipoMovimiento == "AjusteInventario" && m.SaldoNuevo < m.SaldoAnterior))
                .Sum(m => m.Cantidad);

            decimal saldoFinal = movimientosRango.Any()
                ? movimientosRango.Last().SaldoNuevo
                : saldoInicial;

            decimal cppFinal = movimientosRango.Any()
                ? movimientosRango.Last().CostoPromedioResultante
                : insumo.CostoPromedio;

            string? nombreAlmacen = null;
            if (idAlmacen.HasValue && idAlmacen.Value > 0)
            {
                var alm = await _context.Almacenes.FindAsync(idAlmacen.Value);
                nombreAlmacen = alm?.Nombre;
            }

            response.Data = new KardexReporteDTO
            {
                InsumoId = insumo.Id,
                InsumoCodigo = insumo.Codigo,
                InsumoNombre = insumo.Nombre,
                UnidadMedida = insumo.UnidadMedidaBase?.Codigo ?? "N/A",
                AlmacenId = idAlmacen,
                AlmacenNombre = nombreAlmacen ?? "Todos los Almacenes",
                SaldoInicial = saldoInicial,
                TotalEntradas = totalEntradas,
                TotalSalidas = totalSalidas,
                SaldoFinal = saldoFinal,
                CostoPromedioFinal = cppFinal,
                ValorizadoFinal = saldoFinal * cppFinal,
                Movimientos = movimientosRango
            };
            response.isSuccess = true;
            response.Message = "Kárdex obtenido con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al obtener kárdex: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Traspasos Atómicos

    [HttpPost("Traspaso")]
    public async Task<ActionResult<Response<TraspasoResumenDTO>>> RegistrarTraspaso([FromBody] TraspasoCrearDTO dto)
    {
        var response = new Response<TraspasoResumenDTO>();

        if (dto.IdAlmacenOrigen == dto.IdAlmacenDestino)
        {
            response.isSuccess = false;
            response.Message = "El almacén de origen y destino deben ser distintos.";
            return BadRequest(response);
        }

        if (dto.Items == null || !dto.Items.Any() || dto.Items.All(i => i.Cantidad <= 0))
        {
            response.isSuccess = false;
            response.Message = "Debe especificar al menos un insumo con cantidad mayor a 0 para el traspaso.";
            return BadRequest(response);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var origen = await _context.Almacenes.FindAsync(dto.IdAlmacenOrigen);
            var destino = await _context.Almacenes.FindAsync(dto.IdAlmacenDestino);

            if (origen == null || !origen.IsActive || destino == null || !destino.IsActive)
            {
                response.isSuccess = false;
                response.Message = "Uno de los almacenes no existe o está inactivo.";
                return BadRequest(response);
            }

            var countHoy = await _context.TraspasosAlmacen
                .CountAsync(t => t.FechaSolicitud.Date == DateTime.UtcNow.Date);
            string folio = $"TRP-{DateTime.UtcNow:yyyyMMdd}-{(countHoy + 1):D4}";

            var currentUserId = GetCurrentUserId() ?? 1;

            var traspaso = new TraspasoAlmacen
            {
                Folio = folio,
                IdAlmacenOrigen = dto.IdAlmacenOrigen,
                IdAlmacenDestino = dto.IdAlmacenDestino,
                Estado = "Completado",
                IdUsuarioSolicita = currentUserId,
                IdUsuarioRecibe = currentUserId,
                FechaSolicitud = DateTime.UtcNow,
                FechaRecepcion = DateTime.UtcNow,
                Observaciones = dto.Observaciones,
                IsActive = true
            };

            _context.TraspasosAlmacen.Add(traspaso);
            await _context.SaveChangesAsync();

            var resumenItems = new List<TraspasoItemDTO>();

            foreach (var item in dto.Items.Where(i => i.Cantidad > 0))
            {
                var insumo = await _context.Insumos
                    .Include(i => i.UnidadMedidaBase)
                    .FirstOrDefaultAsync(i => i.Id == item.IdInsumo);

                if (insumo == null || !insumo.IsActive)
                    continue;

                // Existencia Origen
                var existOrigen = await _context.InventarioExistencias
                    .FirstOrDefaultAsync(e => e.IdAlmacen == dto.IdAlmacenOrigen && e.IdInsumo == item.IdInsumo);

                decimal stockOrigenPrevio = existOrigen?.StockActual ?? 0;

                if (stockOrigenPrevio < item.Cantidad)
                {
                    await transaction.RollbackAsync();
                    response.isSuccess = false;
                    response.Message = $"Stock insuficiente en almacén de origen para '{insumo.Nombre}'. Disponible: {stockOrigenPrevio}, Solicitado: {item.Cantidad}.";
                    return BadRequest(response);
                }

                decimal nuevoStockOrigen = stockOrigenPrevio - item.Cantidad;
                if (existOrigen == null)
                {
                    existOrigen = new InventarioExistencia
                    {
                        IdAlmacen = dto.IdAlmacenOrigen,
                        IdInsumo = item.IdInsumo,
                        StockActual = nuevoStockOrigen,
                        FechaUltimoMovimiento = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.InventarioExistencias.Add(existOrigen);
                }
                else
                {
                    existOrigen.StockActual = nuevoStockOrigen;
                    existOrigen.FechaUltimoMovimiento = DateTime.UtcNow;
                }

                // Kárdex Salida en Origen
                var kardexSalida = new KardexMovimiento
                {
                    IdAlmacen = dto.IdAlmacenOrigen,
                    IdInsumo = item.IdInsumo,
                    TipoMovimiento = "TraspasoSalida",
                    Submotivo = $"Traspaso hacia {destino.Nombre}",
                    Cantidad = item.Cantidad,
                    CostoUnitario = insumo.CostoPromedio,
                    CostoTotal = item.Cantidad * insumo.CostoPromedio,
                    SaldoAnterior = stockOrigenPrevio,
                    SaldoNuevo = nuevoStockOrigen,
                    CostoPromedioResultante = insumo.CostoPromedio,
                    DocumentoReferencia = folio,
                    Observaciones = dto.Observaciones,
                    IdUsuario = currentUserId,
                    FechaHora = DateTime.UtcNow,
                    IsActive = true
                };
                _context.KardexMovimientos.Add(kardexSalida);

                // Existencia Destino
                var existDestino = await _context.InventarioExistencias
                    .FirstOrDefaultAsync(e => e.IdAlmacen == dto.IdAlmacenDestino && e.IdInsumo == item.IdInsumo);

                decimal stockDestinoPrevio = existDestino?.StockActual ?? 0;
                decimal nuevoStockDestino = stockDestinoPrevio + item.Cantidad;

                if (existDestino == null)
                {
                    existDestino = new InventarioExistencia
                    {
                        IdAlmacen = dto.IdAlmacenDestino,
                        IdInsumo = item.IdInsumo,
                        StockActual = nuevoStockDestino,
                        FechaUltimoMovimiento = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.InventarioExistencias.Add(existDestino);
                }
                else
                {
                    existDestino.StockActual = nuevoStockDestino;
                    existDestino.FechaUltimoMovimiento = DateTime.UtcNow;
                }

                // Kárdex Entrada en Destino
                var kardexEntrada = new KardexMovimiento
                {
                    IdAlmacen = dto.IdAlmacenDestino,
                    IdInsumo = item.IdInsumo,
                    TipoMovimiento = "TraspasoEntrada",
                    Submotivo = $"Traspaso desde {origen.Nombre}",
                    Cantidad = item.Cantidad,
                    CostoUnitario = insumo.CostoPromedio,
                    CostoTotal = item.Cantidad * insumo.CostoPromedio,
                    SaldoAnterior = stockDestinoPrevio,
                    SaldoNuevo = nuevoStockDestino,
                    CostoPromedioResultante = insumo.CostoPromedio,
                    DocumentoReferencia = folio,
                    Observaciones = dto.Observaciones,
                    IdUsuario = currentUserId,
                    FechaHora = DateTime.UtcNow,
                    IsActive = true
                };
                _context.KardexMovimientos.Add(kardexEntrada);

                // Detalle del traspaso
                var detalle = new TraspasoAlmacenDetalle
                {
                    IdTraspasoAlmacen = traspaso.Id,
                    IdInsumo = item.IdInsumo,
                    Cantidad = item.Cantidad,
                    CantidadRecibida = item.Cantidad,
                    IsActive = true
                };
                _context.TraspasoAlmacenDetalles.Add(detalle);

                resumenItems.Add(new TraspasoItemDTO
                {
                    IdInsumo = insumo.Id,
                    InsumoNombre = insumo.Nombre,
                    UnidadMedida = insumo.UnidadMedidaBase?.Codigo ?? "N/A",
                    Cantidad = item.Cantidad,
                    StockDisponible = nuevoStockOrigen
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var user = await _context.Usuarios.FindAsync(currentUserId);

            response.Data = new TraspasoResumenDTO
            {
                Id = traspaso.Id,
                Folio = traspaso.Folio,
                IdAlmacenOrigen = origen.Id,
                AlmacenOrigenNombre = origen.Nombre,
                IdAlmacenDestino = destino.Id,
                AlmacenDestinoNombre = destino.Nombre,
                Estado = traspaso.Estado,
                UsuarioSolicitaNombre = user?.NombreCompleto ?? "Sistema",
                FechaSolicitud = traspaso.FechaSolicitud,
                Observaciones = traspaso.Observaciones,
                TotalItems = resumenItems.Count,
                Detalles = resumenItems
            };
            response.isSuccess = true;
            response.Message = $"Traspaso {folio} realizado con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al procesar el traspaso: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Conteo Físico y Conciliación

    [HttpPost("ConteoFisico/Ajustar")]
    public async Task<ActionResult<Response<int>>> AplicarAjustesConteoFisico([FromBody] LoteConteoFisicoDTO dto)
    {
        var response = new Response<int>();

        if (dto.Conteos == null || !dto.Conteos.Any())
        {
            response.isSuccess = false;
            response.Message = "No se recibieron conteos para ajustar.";
            return BadRequest(response);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var almacen = await _context.Almacenes.FindAsync(dto.IdAlmacen);
            if (almacen == null || !almacen.IsActive)
            {
                response.isSuccess = false;
                response.Message = "El almacén especificado no existe o está inactivo.";
                return BadRequest(response);
            }

            int ajustados = 0;
            var currentUserId = GetCurrentUserId() ?? 1;

            foreach (var item in dto.Conteos)
            {
                var insumo = await _context.Insumos.FindAsync(item.IdInsumo);
                if (insumo == null || !insumo.IsActive)
                    continue;

                var existencia = await _context.InventarioExistencias
                    .FirstOrDefaultAsync(e => e.IdAlmacen == dto.IdAlmacen && e.IdInsumo == item.IdInsumo);

                decimal stockTeorico = existencia?.StockActual ?? 0;
                decimal discrepancia = item.StockFisico - stockTeorico;

                if (Math.Abs(discrepancia) < 0.0001m)
                    continue; // Sin variación

                if (existencia == null)
                {
                    existencia = new InventarioExistencia
                    {
                        IdAlmacen = dto.IdAlmacen,
                        IdInsumo = item.IdInsumo,
                        StockActual = item.StockFisico,
                        FechaUltimoMovimiento = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.InventarioExistencias.Add(existencia);
                }
                else
                {
                    existencia.StockActual = item.StockFisico;
                    existencia.FechaUltimoMovimiento = DateTime.UtcNow;
                }

                var kardex = new KardexMovimiento
                {
                    IdAlmacen = dto.IdAlmacen,
                    IdInsumo = item.IdInsumo,
                    TipoMovimiento = "AjusteInventario",
                    Submotivo = discrepancia > 0 ? "AjusteConteoFisicoSobrante" : "AjusteConteoFisicoFaltante",
                    Cantidad = Math.Abs(discrepancia),
                    CostoUnitario = insumo.CostoPromedio,
                    CostoTotal = Math.Abs(discrepancia) * insumo.CostoPromedio,
                    SaldoAnterior = stockTeorico,
                    SaldoNuevo = item.StockFisico,
                    CostoPromedioResultante = insumo.CostoPromedio,
                    Observaciones = dto.Observaciones ?? "Ajuste generado tras auditoría de conteo físico.",
                    IdUsuario = currentUserId,
                    FechaHora = DateTime.UtcNow,
                    IsActive = true
                };
                _context.KardexMovimientos.Add(kardex);
                ajustados++;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Data = ajustados;
            response.isSuccess = true;
            response.Message = $"Se conciliaron y ajustaron {ajustados} insumo(s) correctamente.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.isSuccess = false;
            response.Message = $"Error al aplicar ajustes de conteo físico: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion
}

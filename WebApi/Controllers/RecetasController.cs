using System.Security.Claims;
using Common;
using Domain.Entities;
using DTO.Receta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using UseCases.Inventario;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RecetasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RecetasController(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GetCurrentUserName()
    {
        return User.Identity?.Name ?? "Sistema";
    }

    #region Consultas

    [HttpGet]
    public async Task<ActionResult<Response<IEnumerable<RecetaDTO>>>> GetRecetas(
        [FromQuery] int? idProducto = null,
        [FromQuery] int? idVariante = null,
        [FromQuery] int? idOpcionModificador = null,
        [FromQuery] bool? esSubReceta = null,
        [FromQuery] string? search = null)
    {
        var response = new Response<IEnumerable<RecetaDTO>>();

        try
        {
            var query = _context.Recetas
                .Include(r => r.Producto)
                .Include(r => r.Variante)
                .Include(r => r.OpcionModificador)
                .Include(r => r.UnidadMedidaRendimiento)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.Insumo)
                        .ThenInclude(i => i.UnidadMedidaBase)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.UnidadMedida)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.UnidadMedidaRendimiento)
                .Where(r => r.IsActive);

            if (idProducto.HasValue && idProducto.Value > 0)
                query = query.Where(r => r.IdProducto == idProducto.Value);

            if (idVariante.HasValue && idVariante.Value > 0)
                query = query.Where(r => r.IdVariante == idVariante.Value);

            if (idOpcionModificador.HasValue && idOpcionModificador.Value > 0)
                query = query.Where(r => r.IdOpcionModificador == idOpcionModificador.Value);

            if (esSubReceta.HasValue)
                query = query.Where(r => r.EsSubReceta == esSubReceta.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(r => r.Nombre.ToLower().Contains(s)
                    || (r.Producto != null && r.Producto.Nombre.ToLower().Contains(s))
                    || (r.Variante != null && r.Variante.Nombre != null && r.Variante.Nombre.ToLower().Contains(s)));
            }

            var list = await query
                .OrderBy(r => r.EsSubReceta)
                .ThenBy(r => r.Nombre)
                .ToListAsync();

            // Cargar precios de venta para calcular márgenes
            var precios = await _context.Precios
                .Where(p => p.IsActive)
                .ToListAsync();

            var dtos = list.Select(r => MapToDTO(r, precios)).ToList();

            response.Data = dtos;
            response.isSuccess = true;
            response.Message = "Recetas consultadas con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al consultar recetas: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<RecetaDTO>>> GetRecetaById(int id)
    {
        var response = new Response<RecetaDTO>();

        try
        {
            var receta = await _context.Recetas
                .Include(r => r.Producto)
                .Include(r => r.Variante)
                .Include(r => r.OpcionModificador)
                .Include(r => r.UnidadMedidaRendimiento)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.Insumo)
                        .ThenInclude(i => i.UnidadMedidaBase)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.UnidadMedida)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.UnidadMedidaRendimiento)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                response.isSuccess = false;
                response.Message = "La receta especificada no existe.";
                return NotFound(response);
            }

            var precios = await _context.Precios
                .Where(p => p.IsActive)
                .ToListAsync();

            response.Data = MapToDTO(receta, precios);
            response.isSuccess = true;
            response.Message = "Receta obtenida con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al consultar receta: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpGet("SubRecetasDisponibles")]
    public async Task<ActionResult<Response<IEnumerable<SubRecetaSimpleDTO>>>> GetSubRecetasDisponibles([FromQuery] int? idRecetaExcluir = null)
    {
        var response = new Response<IEnumerable<SubRecetaSimpleDTO>>();

        try
        {
            var query = _context.Recetas
                .Include(r => r.UnidadMedidaRendimiento)
                .Where(r => r.EsSubReceta && r.IsActive);

            if (idRecetaExcluir.HasValue && idRecetaExcluir.Value > 0)
                query = query.Where(r => r.Id != idRecetaExcluir.Value);

            var items = await query
                .OrderBy(r => r.Nombre)
                .Select(r => new SubRecetaSimpleDTO
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Rendimiento = r.Rendimiento,
                    IdUnidadMedidaRendimiento = r.IdUnidadMedidaRendimiento,
                    UnidadMedidaCodigo = r.UnidadMedidaRendimiento != null ? r.UnidadMedidaRendimiento.Codigo : "N/A",
                    UnidadMedidaNombre = r.UnidadMedidaRendimiento != null ? r.UnidadMedidaRendimiento.Nombre : "N/A",
                    CostoEstimadoUnitario = r.CostoEstimadoUnitario
                })
                .ToListAsync();

            response.Data = items;
            response.isSuccess = true;
            response.Message = "Sub-recetas obtenidas con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al consultar sub-recetas: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpGet("PorVariante/{idVariante}")]
    public async Task<ActionResult<Response<RecetaDTO>>> GetPorVariante(int idVariante)
    {
        var response = new Response<RecetaDTO>();

        try
        {
            var variante = await _context.VarianteProductos.FindAsync(idVariante);
            if (variante == null)
            {
                response.isSuccess = false;
                response.Message = "La variante especificada no existe.";
                return NotFound(response);
            }

            var receta = await _context.Recetas
                .Include(r => r.Producto)
                .Include(r => r.Variante)
                .Include(r => r.UnidadMedidaRendimiento)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.Insumo)
                        .ThenInclude(i => i.UnidadMedidaBase)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.UnidadMedida)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.UnidadMedidaRendimiento)
                .FirstOrDefaultAsync(r => r.IdVariante == idVariante && r.IsActive)
                ?? await _context.Recetas
                .Include(r => r.Producto)
                .Include(r => r.Variante)
                .Include(r => r.UnidadMedidaRendimiento)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.Insumo)
                        .ThenInclude(i => i.UnidadMedidaBase)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.UnidadMedida)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.SubReceta)
                        .ThenInclude(s => s.UnidadMedidaRendimiento)
                .FirstOrDefaultAsync(r => r.IdProducto == variante.IdProducto && !r.EsSubReceta && r.IsActive);

            if (receta == null)
            {
                response.isSuccess = false;
                response.Message = "No se encontró receta configurada para esta variante.";
                return NotFound(response);
            }

            var precios = await _context.Precios.Where(p => p.IsActive).ToListAsync();
            response.Data = MapToDTO(receta, precios);
            response.isSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpGet("PorModificador/{idOpcion}")]
    public async Task<ActionResult<Response<RecetaDTO>>> GetPorModificador(int idOpcion)
    {
        var response = new Response<RecetaDTO>();

        try
        {
            var receta = await _context.Recetas
                .Include(r => r.OpcionModificador)
                .Include(r => r.UnidadMedidaRendimiento)
                .Include(r => r.Detalles.Where(d => d.IsActive))
                    .ThenInclude(d => d.Insumo)
                        .ThenInclude(i => i.UnidadMedidaBase)
                .Include(r => r.Detalles.Where(d => d.UnidadMedida != null))
                    .ThenInclude(d => d.UnidadMedida)
                .FirstOrDefaultAsync(r => r.IdOpcionModificador == idOpcion && r.IsActive);

            if (receta == null)
            {
                response.isSuccess = false;
                response.Message = "No se encontró receta para esta opción de modificador.";
                return NotFound(response);
            }

            var precios = await _context.Precios.Where(p => p.IsActive).ToListAsync();
            response.Data = MapToDTO(receta, precios);
            response.isSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Simulación y Live Costing

    [HttpPost("Simular")]
    public async Task<ActionResult<Response<SimulacionCosteoResponseDTO>>> SimularCosteo([FromBody] SimulacionCosteoRequestDTO dto)
    {
        var response = new Response<SimulacionCosteoResponseDTO>();

        try
        {
            var insumoIds = dto.Detalles.Where(d => d.IdInsumo.HasValue).Select(d => d.IdInsumo!.Value).Distinct().ToList();
            var subRecetaIds = dto.Detalles.Where(d => d.IdSubReceta.HasValue).Select(d => d.IdSubReceta!.Value).Distinct().ToList();
            var umIds = dto.Detalles.Select(d => d.IdUnidadMedida).Distinct().ToList();

            var insumos = await _context.Insumos
                .Include(i => i.UnidadMedidaBase)
                .Where(i => insumoIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id);

            var subRecetas = await _context.Recetas
                .Include(r => r.UnidadMedidaRendimiento)
                .Where(r => subRecetaIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id);

            var unidades = await _context.UnidadesMedida
                .Where(u => umIds.Contains(u.Id) || u.Id == dto.IdUnidadMedidaRendimiento)
                .ToDictionaryAsync(u => u.Id);

            var factores = await _context.FactoresConversion.Where(f => f.IsActive).ToListAsync();

            decimal costoTotalLote = 0m;
            var desglose = new List<SimulacionIngredienteResultDTO>();

            foreach (var item in dto.Detalles)
            {
                unidades.TryGetValue(item.IdUnidadMedida, out var um);
                string umCod = um?.Codigo ?? "PZA";

                if (item.IdInsumo.HasValue && insumos.TryGetValue(item.IdInsumo.Value, out var insumo))
                {
                    string umBaseCod = insumo.UnidadMedidaBase?.Codigo ?? umCod;

                    decimal? factorBd = factores
                        .FirstOrDefault(f => f.IdUnidadOrigen == item.IdUnidadMedida && f.IdUnidadDestino == insumo.IdUnidadMedidaBase)?
                        .Factor;

                    decimal cantBase = ConversionUnidadesHelper.ConvertirCantidad(item.Cantidad, umCod, umBaseCod, factorBd);
                    decimal cantConMerma = cantBase * (1m + (item.PorcentajeMermaEsperada / 100m));
                    decimal costoItem = Math.Round(cantConMerma * insumo.CostoPromedio, 4);

                    costoTotalLote += costoItem;

                    desglose.Add(new SimulacionIngredienteResultDTO
                    {
                        IdInsumo = item.IdInsumo,
                        Nombre = insumo.Nombre,
                        Cantidad = item.Cantidad,
                        UnidadMedida = umCod,
                        CostoUnitario = insumo.CostoPromedio,
                        CantidadEquivalenteBase = Math.Round(cantConMerma, 4),
                        MermaPct = item.PorcentajeMermaEsperada,
                        CostoTotal = costoItem
                    });
                }
                else if (item.IdSubReceta.HasValue && subRecetas.TryGetValue(item.IdSubReceta.Value, out var sub))
                {
                    decimal cantConMerma = item.Cantidad * (1m + (item.PorcentajeMermaEsperada / 100m));
                    decimal costoItem = Math.Round(cantConMerma * sub.CostoEstimadoUnitario, 4);
                    costoTotalLote += costoItem;

                    desglose.Add(new SimulacionIngredienteResultDTO
                    {
                        IdSubReceta = item.IdSubReceta,
                        Nombre = $"[Sub-receta] {sub.Nombre}",
                        Cantidad = item.Cantidad,
                        UnidadMedida = umCod,
                        CostoUnitario = sub.CostoEstimadoUnitario,
                        CantidadEquivalenteBase = Math.Round(cantConMerma, 4),
                        MermaPct = item.PorcentajeMermaEsperada,
                        CostoTotal = costoItem
                    });
                }
            }

            // Calcular porcentajes de participación
            foreach (var item in desglose)
            {
                item.ParticipacionCostoPct = costoTotalLote > 0
                    ? Math.Round((item.CostoTotal / costoTotalLote) * 100m, 2)
                    : 0m;
            }

            decimal rendimiento = dto.Rendimiento > 0 ? dto.Rendimiento : 1m;
            decimal costoPorcion = Math.Round(costoTotalLote / rendimiento, 4);

            decimal pvp = dto.PrecioVenta ?? 0m;
            decimal margenBrutoMonto = pvp - costoPorcion;
            decimal foodCostPct = pvp > 0 ? Math.Round((costoPorcion / pvp) * 100m, 2) : 0m;
            decimal margenBrutoPct = pvp > 0 ? Math.Round((margenBrutoMonto / pvp) * 100m, 2) : 0m;

            // Si se suministró margen objetivo deseado (ej. 70%), calcular PVP sugerido
            decimal precioVentaCalculado = pvp;
            if (dto.MargenObjetivoPct.HasValue && dto.MargenObjetivoPct.Value > 0 && dto.MargenObjetivoPct.Value < 100)
            {
                decimal targetMarginFactor = (100m - dto.MargenObjetivoPct.Value) / 100m;
                if (targetMarginFactor > 0)
                {
                    precioVentaCalculado = Math.Round(costoPorcion / targetMarginFactor, 2);
                }
            }
            else if (pvp <= 0 && costoPorcion > 0)
            {
                // Default target Food Cost 28% -> Margen 72%
                precioVentaCalculado = Math.Round(costoPorcion / 0.28m, 2);
            }

            decimal precioVentaConIva = Math.Round(precioVentaCalculado * 1.16m, 2);

            string nivelSalud = "Optimo";
            if (foodCostPct > 38m) nivelSalud = "Critico";
            else if (foodCostPct >= 30m) nivelSalud = "Ajustado";

            response.Data = new SimulacionCosteoResponseDTO
            {
                CostoTotalLote = Math.Round(costoTotalLote, 2),
                CostoPorcion = Math.Round(costoPorcion, 2),
                FoodCostPct = foodCostPct,
                MargenBrutoPct = margenBrutoPct,
                MargenBrutoMonto = Math.Round(margenBrutoMonto, 2),
                PrecioVentaCalculado = precioVentaCalculado,
                PrecioVentaConIva = precioVentaConIva,
                NivelSaludMargen = nivelSalud,
                Desglose = desglose.OrderByDescending(d => d.CostoTotal).ToList()
            };

            response.isSuccess = true;
            response.Message = "Simulación realizada con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al simular costeo: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Creación, Edición y Eliminación

    [HttpPost]
    public async Task<ActionResult<Response<RecetaDTO>>> CrearReceta([FromBody] CrearRecetaDTO dto)
    {
        var response = new Response<RecetaDTO>();

        try
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                response.isSuccess = false;
                response.Message = "El nombre de la receta es obligatorio.";
                return BadRequest(response);
            }

            if (dto.Rendimiento <= 0)
            {
                response.isSuccess = false;
                response.Message = "El rendimiento debe ser mayor a 0.";
                return BadRequest(response);
            }

            // Spec 026: Asegurar variante default "Estándar" si el platillo no tiene una asignada
            int? idVarianteFinal = dto.EsSubReceta ? null : dto.IdVariante;
            if (!dto.EsSubReceta && dto.IdProducto.HasValue && dto.IdProducto.Value > 0)
            {
                if (!idVarianteFinal.HasValue || idVarianteFinal.Value <= 0)
                {
                    var varDefault = await _context.VarianteProductos
                        .FirstOrDefaultAsync(v => v.IdProducto == dto.IdProducto.Value && v.EsDefault)
                        ?? await _context.VarianteProductos
                        .FirstOrDefaultAsync(v => v.IdProducto == dto.IdProducto.Value);

                    if (varDefault == null)
                    {
                        varDefault = new Domain.Entities.VarianteProducto
                        {
                            IdProducto = dto.IdProducto.Value,
                            Nombre = "Estándar",
                            EsDefault = true
                        };
                        _context.VarianteProductos.Add(varDefault);
                        await _context.SaveChangesAsync();
                    }
                    idVarianteFinal = varDefault.Id;
                }
            }

            var receta = new Receta
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                EsSubReceta = dto.EsSubReceta,
                IdProducto = dto.EsSubReceta ? null : dto.IdProducto,
                IdVariante = idVarianteFinal,
                IdOpcionModificador = dto.EsSubReceta ? null : dto.IdOpcionModificador,
                Rendimiento = dto.Rendimiento,
                IdUnidadMedidaRendimiento = dto.IdUnidadMedidaRendimiento,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = GetCurrentUserName(),
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = GetCurrentUserName()
            };

            _context.Recetas.Add(receta);
            await _context.SaveChangesAsync();

            // Agregar detalles
            await ActualizarDetallesReceta(receta.Id, dto.Detalles);

            // Recalcular costo unitario
            receta.CostoEstimadoUnitario = await CalcularCostoUnitarioAsync(receta.Id);
            await _context.SaveChangesAsync();

            // Spec 026: Sincronizar PVP directamente con la administración de precios para visualización en POS
            if (idVarianteFinal.HasValue && dto.PrecioVentaActual.HasValue && dto.PrecioVentaActual.Value > 0)
            {
                await SincronizarPrecioVarianteAsync(idVarianteFinal.Value, dto.PrecioVentaActual.Value);
            }

            return await GetRecetaById(receta.Id);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al crear receta: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Response<RecetaDTO>>> ActualizarReceta(int id, [FromBody] ActualizarRecetaDTO dto)
    {
        var response = new Response<RecetaDTO>();

        try
        {
            var receta = await _context.Recetas
                .Include(r => r.Detalles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                response.isSuccess = false;
                response.Message = "La receta especificada no existe.";
                return NotFound(response);
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                receta.Nombre = dto.Nombre.Trim();

            // Spec 026: Resolver variante default si se asignó un producto y no tenía variante
            int? idVarianteFinal = dto.EsSubReceta ? null : dto.IdVariante;
            if (!dto.EsSubReceta && dto.IdProducto.HasValue && dto.IdProducto.Value > 0)
            {
                if (!idVarianteFinal.HasValue || idVarianteFinal.Value <= 0)
                {
                    var varDefault = await _context.VarianteProductos
                        .FirstOrDefaultAsync(v => v.IdProducto == dto.IdProducto.Value && v.EsDefault)
                        ?? await _context.VarianteProductos
                        .FirstOrDefaultAsync(v => v.IdProducto == dto.IdProducto.Value);

                    if (varDefault == null)
                    {
                        varDefault = new Domain.Entities.VarianteProducto
                        {
                            IdProducto = dto.IdProducto.Value,
                            Nombre = "Estándar",
                            EsDefault = true
                        };
                        _context.VarianteProductos.Add(varDefault);
                        await _context.SaveChangesAsync();
                    }
                    idVarianteFinal = varDefault.Id;
                }
            }

            receta.Descripcion = dto.Descripcion?.Trim();
            receta.EsSubReceta = dto.EsSubReceta;
            receta.IdProducto = dto.EsSubReceta ? null : dto.IdProducto;
            receta.IdVariante = idVarianteFinal ?? receta.IdVariante;
            receta.IdOpcionModificador = dto.EsSubReceta ? null : dto.IdOpcionModificador;
            receta.Rendimiento = dto.Rendimiento > 0 ? dto.Rendimiento : 1m;
            receta.IdUnidadMedidaRendimiento = dto.IdUnidadMedidaRendimiento;
            receta.IsActive = dto.IsActive;
            receta.UpdatedAt = DateTime.UtcNow;
            receta.UpdatedBy = GetCurrentUserName();

            await _context.SaveChangesAsync();

            // Reemplazar detalles
            await ActualizarDetallesReceta(receta.Id, dto.Detalles);

            // Recalcular costo unitario
            receta.CostoEstimadoUnitario = await CalcularCostoUnitarioAsync(receta.Id);
            await _context.SaveChangesAsync();

            // Spec 026: Sincronizar PVP actualizado directamente en Precios
            int? idVarParaPrecio = receta.IdVariante ?? idVarianteFinal;
            if (idVarParaPrecio.HasValue && dto.PrecioVentaActual.HasValue && dto.PrecioVentaActual.Value > 0)
            {
                await SincronizarPrecioVarianteAsync(idVarParaPrecio.Value, dto.PrecioVentaActual.Value);
            }

            return await GetRecetaById(receta.Id);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al actualizar receta: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    private async Task SincronizarPrecioVarianteAsync(int idVariante, decimal nuevoMonto)
    {
        var catImpuestos = await _context.CatImpuestos.ToListAsync();
        int idImpuestoIva = catImpuestos.FirstOrDefault(i => i.Descripcion != null && i.Descripcion.ToLower().Contains("iva"))?.Id ?? 1;

        var catMonedas = await _context.CatMonedas.ToListAsync();
        int idMonedaMxn = catMonedas.FirstOrDefault(m => m.Descripcion != null && (m.Descripcion.ToUpper().Contains("MXN") || m.Descripcion.ToLower().Contains("peso")))?.Id ?? 1;

        var precioExistente = await _context.Precios
            .FirstOrDefaultAsync(p => p.IdVariante == idVariante && p.IsActive);

        if (precioExistente != null)
        {
            precioExistente.Monto = nuevoMonto;
            precioExistente.UpdatedAt = DateTime.UtcNow;
            precioExistente.UpdatedBy = GetCurrentUserName();
        }
        else
        {
            var nuevoPrecio = new Precio
            {
                IdVariante = idVariante,
                Monto = nuevoMonto,
                Moneda = "MXN",
                IdImpuesto = idImpuestoIva,
                IdMoneda = idMonedaMxn,
                ValidoDesde = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = GetCurrentUserName(),
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = GetCurrentUserName()
            };
            _context.Precios.Add(nuevoPrecio);
        }
        await _context.SaveChangesAsync();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<bool>>> EliminarReceta(int id)
    {
        var response = new Response<bool>();

        try
        {
            var receta = await _context.Recetas.FindAsync(id);
            if (receta == null)
            {
                response.isSuccess = false;
                response.Message = "La receta especificada no existe.";
                return NotFound(response);
            }

            // Validar si alguna receta activa consume esta como sub-receta
            bool enUso = await _context.RecetaDetalles
                .AnyAsync(d => d.IdSubReceta == id && d.IsActive && d.Receta.IsActive);

            if (enUso)
            {
                response.isSuccess = false;
                response.Message = "No se puede eliminar esta sub-receta porque está siendo consumida por otras recetas activas.";
                return BadRequest(response);
            }

            receta.IsActive = false;
            receta.UpdatedAt = DateTime.UtcNow;
            receta.UpdatedBy = GetCurrentUserName();

            await _context.SaveChangesAsync();

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Receta desactivada con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al eliminar receta: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Helpers Internos

    private async Task ActualizarDetallesReceta(int idReceta, List<CrearRecetaDetalleDTO> nuevosDetalles)
    {
        var detallesPrevios = await _context.RecetaDetalles.Where(d => d.IdReceta == idReceta).ToListAsync();
        _context.RecetaDetalles.RemoveRange(detallesPrevios);

        if (nuevosDetalles == null || !nuevosDetalles.Any()) return;

        var insumoIds = nuevosDetalles.Where(d => d.IdInsumo.HasValue).Select(d => d.IdInsumo!.Value).Distinct().ToList();
        var subRecetaIds = nuevosDetalles.Where(d => d.IdSubReceta.HasValue).Select(d => d.IdSubReceta!.Value).Distinct().ToList();
        var umIds = nuevosDetalles.Select(d => d.IdUnidadMedida).Distinct().ToList();

        var insumos = await _context.Insumos.Include(i => i.UnidadMedidaBase).Where(i => insumoIds.Contains(i.Id)).ToDictionaryAsync(i => i.Id);
        var subRecetas = await _context.Recetas.Where(r => subRecetaIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id);
        var unidades = await _context.UnidadesMedida.Where(u => umIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id);
        var factores = await _context.FactoresConversion.Where(f => f.IsActive).ToListAsync();

        var ahora = DateTime.UtcNow;
        var usuario = GetCurrentUserName();

        foreach (var item in nuevosDetalles)
        {
            decimal costo = 0m;
            unidades.TryGetValue(item.IdUnidadMedida, out var um);
            string umCod = um?.Codigo ?? "PZA";

            if (item.IdInsumo.HasValue && insumos.TryGetValue(item.IdInsumo.Value, out var insumo))
            {
                string umBaseCod = insumo.UnidadMedidaBase?.Codigo ?? umCod;
                decimal? factorBd = factores
                    .FirstOrDefault(f => f.IdUnidadOrigen == item.IdUnidadMedida && f.IdUnidadDestino == insumo.IdUnidadMedidaBase)?
                    .Factor;

                decimal cantBase = ConversionUnidadesHelper.ConvertirCantidad(item.Cantidad, umCod, umBaseCod, factorBd);
                decimal cantConMerma = cantBase * (1m + (item.PorcentajeMermaEsperada / 100m));
                costo = Math.Round(cantConMerma * insumo.CostoPromedio, 4);
            }
            else if (item.IdSubReceta.HasValue && subRecetas.TryGetValue(item.IdSubReceta.Value, out var sub))
            {
                decimal cantConMerma = item.Cantidad * (1m + (item.PorcentajeMermaEsperada / 100m));
                costo = Math.Round(cantConMerma * sub.CostoEstimadoUnitario, 4);
            }

            var detalle = new RecetaDetalle
            {
                IdReceta = idReceta,
                IdInsumo = item.IdInsumo,
                IdSubReceta = item.IdSubReceta,
                Cantidad = item.Cantidad,
                IdUnidadMedida = item.IdUnidadMedida,
                PorcentajeMermaEsperada = item.PorcentajeMermaEsperada,
                CostoCalculado = costo,
                IsActive = true,
                CreatedAt = ahora,
                CreatedBy = usuario,
                UpdatedAt = ahora,
                UpdatedBy = usuario
            };

            _context.RecetaDetalles.Add(detalle);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<decimal> CalcularCostoUnitarioAsync(int idReceta)
    {
        var receta = await _context.Recetas
            .Include(r => r.Detalles)
            .FirstOrDefaultAsync(r => r.Id == idReceta);

        if (receta == null) return 0m;

        decimal costoTotal = receta.Detalles.Where(d => d.IsActive).Sum(d => d.CostoCalculado);
        decimal rendimiento = receta.Rendimiento > 0 ? receta.Rendimiento : 1m;
        return Math.Round(costoTotal / rendimiento, 4);
    }

    private RecetaDTO MapToDTO(Receta r, List<Precio> precios)
    {
        decimal costoTotalLote = r.Detalles.Where(d => d.IsActive).Sum(d => d.CostoCalculado);

        decimal? pvp = null;
        if (r.IdVariante.HasValue)
        {
            var pr = precios.FirstOrDefault(p => p.IdVariante == r.IdVariante.Value);
            if (pr != null) pvp = pr.Monto;
        }
        else if (r.IdOpcionModificador.HasValue && r.OpcionModificador != null)
        {
            pvp = r.OpcionModificador.PrecioExtra;
        }

        decimal costoUnitario = r.CostoEstimadoUnitario > 0
            ? r.CostoEstimadoUnitario
            : (r.Rendimiento > 0 ? Math.Round(costoTotalLote / r.Rendimiento, 4) : costoTotalLote);

        decimal? margenBrutoMonto = pvp.HasValue ? Math.Round(pvp.Value - costoUnitario, 2) : null;
        decimal? margenBrutoPct = (pvp.HasValue && pvp.Value > 0) ? Math.Round(((pvp.Value - costoUnitario) / pvp.Value) * 100m, 2) : null;
        decimal? foodCostPct = (pvp.HasValue && pvp.Value > 0) ? Math.Round((costoUnitario / pvp.Value) * 100m, 2) : null;

        string nivelSalud = "Optimo";
        if (foodCostPct.HasValue)
        {
            if (foodCostPct.Value > 38m) nivelSalud = "Critico";
            else if (foodCostPct.Value >= 30m) nivelSalud = "Ajustado";
        }

        var detallesDto = r.Detalles.Where(d => d.IsActive).Select(d =>
        {
            decimal part = costoTotalLote > 0 ? Math.Round((d.CostoCalculado / costoTotalLote) * 100m, 2) : 0m;
            decimal costoUnitInsumo = d.Insumo != null ? d.Insumo.CostoPromedio : (d.SubReceta != null ? d.SubReceta.CostoEstimadoUnitario : 0m);

            return new RecetaDetalleDTO
            {
                Id = d.Id,
                IdReceta = d.IdReceta,
                IdInsumo = d.IdInsumo,
                InsumoCodigo = d.Insumo?.Codigo,
                InsumoNombre = d.Insumo?.Nombre,
                IdSubReceta = d.IdSubReceta,
                SubRecetaNombre = d.SubReceta?.Nombre,
                Cantidad = d.Cantidad,
                IdUnidadMedida = d.IdUnidadMedida,
                UnidadMedidaCodigo = d.UnidadMedida != null ? d.UnidadMedida.Codigo : "N/A",
                UnidadMedidaNombre = d.UnidadMedida != null ? d.UnidadMedida.Nombre : "N/A",
                PorcentajeMermaEsperada = d.PorcentajeMermaEsperada,
                CostoUnitarioInsumo = costoUnitInsumo,
                CostoCalculado = Math.Round(d.CostoCalculado, 4),
                ParticipacionPct = part
            };
        }).OrderByDescending(d => d.CostoCalculado).ToList();

        return new RecetaDTO
        {
            Id = r.Id,
            IdProducto = r.IdProducto,
            ProductoNombre = r.Producto?.Nombre,
            IdVariante = r.IdVariante,
            VarianteNombre = r.Variante?.Nombre,
            IdOpcionModificador = r.IdOpcionModificador,
            OpcionModificadorNombre = r.OpcionModificador?.Nombre,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion,
            EsSubReceta = r.EsSubReceta,
            Rendimiento = r.Rendimiento,
            IdUnidadMedidaRendimiento = r.IdUnidadMedidaRendimiento,
            UnidadMedidaRendimientoCodigo = r.UnidadMedidaRendimiento?.Codigo ?? "PZA",
            UnidadMedidaRendimientoNombre = r.UnidadMedidaRendimiento?.Nombre ?? "Pieza",
            CostoEstimadoUnitario = Math.Round(costoUnitario, 2),
            CostoTotalLote = Math.Round(costoTotalLote, 2),
            PrecioVentaActual = pvp.HasValue ? Math.Round(pvp.Value, 2) : null,
            MargenBrutoMonto = margenBrutoMonto,
            MargenBrutoPct = margenBrutoPct,
            FoodCostPct = foodCostPct,
            NivelSaludMargen = nivelSalud,
            IsActive = r.IsActive,
            Detalles = detallesDto
        };
    }

    #endregion
}

using Common;
using DTO.Producto;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

[Authorize]
[Route("api/productos")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly IProductoApplication _productoApplication;
    private readonly ApplicationDbContext _context;

    public ProductosController(IProductoApplication productoApplication, ApplicationDbContext context)
    {
        _productoApplication = productoApplication;
        _context = context;
    }

    #region Spec 026: Suite Unificada de Creación de Platillos

    [HttpPost("CrearPlatilloCompleto")]
    public async Task<ActionResult<Response<CrearPlatilloCompletoResultDTO>>> CrearPlatilloCompleto([FromBody] CrearPlatilloCompletoDTO dto)
    {
        var response = new Response<CrearPlatilloCompletoResultDTO>();
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                response.isSuccess = false;
                response.Message = "El nombre del platillo es obligatorio.";
                return BadRequest(response);
            }

            if (dto.IdCategoria <= 0)
            {
                response.isSuccess = false;
                response.Message = "Debe seleccionar una categoría válida.";
                return BadRequest(response);
            }

            if (dto.IdMenu <= 0)
            {
                var primerMenu = await _context.Menus.FirstOrDefaultAsync(m => m.IsActive) ?? await _context.Menus.FirstOrDefaultAsync();
                dto.IdMenu = primerMenu?.Id ?? 1;
            }

            var catImpuestos = await _context.CatImpuestos.ToListAsync();
            int idImpuestoIva = catImpuestos.FirstOrDefault(i => i.Descripcion != null && i.Descripcion.ToLower().Contains("iva"))?.Id ?? 1;

            var catMonedas = await _context.CatMonedas.ToListAsync();
            int idMonedaMxn = catMonedas.FirstOrDefault(m => m.Descripcion != null && (m.Descripcion.ToUpper().Contains("MXN") || m.Descripcion.ToLower().Contains("peso")))?.Id ?? 1;

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // 1. Crear Producto
                var producto = new Domain.Entities.Producto
                {
                    Nombre = dto.Nombre.Trim(),
                    Descripcion = dto.Descripcion?.Trim(),
                    Codigo = !string.IsNullOrWhiteSpace(dto.Codigo) ? dto.Codigo.Trim() : null,
                    IdCategoria = dto.IdCategoria,
                    IdMenu = dto.IdMenu,
                    IdEstacionCocina = dto.IdEstacionCocina > 0 ? dto.IdEstacionCocina : null,
                    Activo = true
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                int idVarianteDefault = 0;
                decimal precioDefault = 0;

                if (!dto.TieneVariantes || dto.Variantes == null || dto.Variantes.Count == 0)
                {
                    // Platillo estándar (85% de los casos)
                    decimal precioMonto = dto.PrecioVenta ?? 0m;
                    var variante = new Domain.Entities.VarianteProducto
                    {
                        IdProducto = producto.Id,
                        Nombre = "Estándar",
                        Codigo = producto.Codigo != null ? $"{producto.Codigo}-STD" : null,
                        EsDefault = true
                    };
                    _context.VarianteProductos.Add(variante);
                    await _context.SaveChangesAsync();

                    idVarianteDefault = variante.Id;
                    precioDefault = precioMonto;

                    var precio = new Domain.Entities.Precio
                    {
                        IdVariante = variante.Id,
                        Monto = precioMonto,
                        Moneda = "MXN",
                        IdImpuesto = idImpuestoIva,
                        IdMoneda = idMonedaMxn,
                        ValidoDesde = DateTime.UtcNow
                    };
                    _context.Precios.Add(precio);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Platillo con múltiples variantes inline
                    bool defaultAsignado = false;
                    for (int i = 0; i < dto.Variantes.Count; i++)
                    {
                        var vDto = dto.Variantes[i];
                        bool esDef = vDto.EsDefault || (!defaultAsignado && i == 0);
                        if (esDef) defaultAsignado = true;

                        var variante = new Domain.Entities.VarianteProducto
                        {
                            IdProducto = producto.Id,
                            Nombre = !string.IsNullOrWhiteSpace(vDto.Nombre) ? vDto.Nombre.Trim() : $"Presentación {i + 1}",
                            Codigo = !string.IsNullOrWhiteSpace(vDto.Codigo) ? vDto.Codigo.Trim() : null,
                            EsDefault = esDef
                        };
                        _context.VarianteProductos.Add(variante);
                        await _context.SaveChangesAsync();

                        if (esDef)
                        {
                            idVarianteDefault = variante.Id;
                            precioDefault = vDto.PrecioVenta;
                        }

                        var precio = new Domain.Entities.Precio
                        {
                            IdVariante = variante.Id,
                            Monto = vDto.PrecioVenta,
                            Moneda = "MXN",
                            IdImpuesto = idImpuestoIva,
                            IdMoneda = idMonedaMxn,
                            ValidoDesde = DateTime.UtcNow
                        };
                        _context.Precios.Add(precio);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                response.Data = new CrearPlatilloCompletoResultDTO
                {
                    IdProducto = producto.Id,
                    Nombre = producto.Nombre,
                    IdVarianteDefault = idVarianteDefault,
                    PrecioVentaDefault = precioDefault
                };
                response.isSuccess = true;
                response.Message = "Platillo creado exitosamente con sus variantes y precios.";
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al crear platillo completo: {ex.Message}";
            return StatusCode(500, response);
        }
    }

    #endregion

    #region Metodos sincronos

    [HttpPost("Insert")]
    public ActionResult<Response<bool>> Insert([FromBody] ProductoDTO producto)
    {
        var response = _productoApplication.Insert(producto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public ActionResult<Response<IEnumerable<ProductoDTO>>> GetAll()
    {
        var response = _productoApplication.GetAll();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public ActionResult<Response<ProductoDTO>> GetById(int id)
    {
        var response = _productoApplication.Get(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public ActionResult<Response<bool>> Update([FromBody] ProductoDTO producto)
    {
        var response = _productoApplication.Update(producto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public ActionResult<Response<bool>> Delete(int id)
    {
        var response = _productoApplication.Delete(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPagination")]
    public ActionResult<ResponsePagination<IEnumerable<ProductoDTO>>> GetAllWithPagination(int page, int pageSize)
    {
        var response = _productoApplication.GetAllWithPagination(page, pageSize);
        return Ok(response);
    }

    [HttpGet("Count")]
    public ActionResult<Response<int>> Count()
    {
        var response = _productoApplication.Count();
        return Ok(response);
    }

    #endregion

    #region Metodos asincronos

    [HttpPost("InsertAsync")]
    public async Task<ActionResult<Response<bool>>> InsertAsync([FromBody] ProductoDTO producto)
    {
        var response = await _productoApplication.InsertAsync(producto);
        return Ok(response);
    }

    [HttpGet("GetAllAsync")]
    public async Task<ActionResult<Response<IEnumerable<ProductoDTO>>>> GetAllAsync()
    {
        var response = await _productoApplication.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<ActionResult<Response<ProductoDTO>>> GetByIdAsync(int id)
    {
        var response = await _productoApplication.GetAsync(id);
        return Ok(response);
    }

    [HttpPut("UpdateAsync")]
    public async Task<ActionResult<Response<bool>>> UpdateAsync([FromBody] ProductoDTO producto)
    {
        var response = await _productoApplication.UpdateAsync(producto);
        return Ok(response);
    }

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAsync(int id)
    {
        var response = await _productoApplication.DeleteAsync(id);
        return Ok(response);
    }

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<ActionResult<ResponsePagination<IEnumerable<ProductoDTO>>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = await _productoApplication.GetAllWithPaginationAsync(page, pageSize);
        return Ok(response);
    }

    [HttpGet("CountAsync")]
    public async Task<ActionResult<Response<int>>> CountAsync()
    {
        var response = await _productoApplication.CountAsync();
        return Ok(response);
    }

    #endregion
}

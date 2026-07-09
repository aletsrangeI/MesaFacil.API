using Common;
using DTO.GenericCatalog;
using Interface.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Controller unificado para todos los catálogos simples (Cat*).
/// La ruta /{catalog} determina qué catálogo se opera. El DI resuelve
/// la implementación correcta de IGenericCatalogApplication mediante Keyed Services.
/// 
/// Endpoints disponibles (reemplazar {catalog} por el nombre de ruta del catálogo):
///   GET    /api/catalogos/{catalog}/GetAll
///   GET    /api/catalogos/{catalog}/GetById/{id}
///   POST   /api/catalogos/{catalog}/Insert
///   PUT    /api/catalogos/{catalog}/Update
///   DELETE /api/catalogos/{catalog}/Delete/{id}
///   GET    /api/catalogos/{catalog}/GetAllWithPagination?page=1&pageSize=10
///   GET    /api/catalogos/{catalog}/Count
///   (+ versiones Async de todos los anteriores)
/// 
/// Catálogos registrados:
///   credenciales | estaciones-cocina | estados-cuenta | estados-item-kds
///   estados-mesa | estados-pedido    | estados-pedido-detalle
///   estados-ticket-cocina | impuestos | metodos-pago
///   monedas | tipos-descuento | tipos-pedido
/// </summary>
[Route("api/catalogos/{catalog}")]
[ApiController]
public class CatalogosController : ControllerBase
{
    private readonly IServiceProvider _sp;

    public CatalogosController(IServiceProvider sp)
        => _sp = sp;

    // -----------------------------------------------------------------
    // Resolución del use case según el segmento de ruta {catalog}
    // -----------------------------------------------------------------
    private IGenericCatalogApplication Resolve(string catalog)
    {
        var svc = _sp.GetKeyedService<IGenericCatalogApplication>(catalog.ToLowerInvariant());
        if (svc is null)
            throw new KeyNotFoundException($"El catálogo '{catalog}' no está registrado.");
        return svc;
    }

    // =================================================================
    //  SÍNCRONOS
    // =================================================================

    [HttpGet("GetAll")]
    public IActionResult GetAll(string catalog)
        => Ok(Resolve(catalog).GetAll());

    [HttpGet("GetById/{id}")]
    public IActionResult GetById(string catalog, int id)
        => Ok(Resolve(catalog).Get(id));

    [HttpPost("Insert")]
    public IActionResult Insert(string catalog, [FromBody] GenericCatalogDTO dto)
        => Ok(Resolve(catalog).Insert(dto));

    [HttpPut("Update")]
    public IActionResult Update(string catalog, [FromBody] GenericCatalogDTO dto)
        => Ok(Resolve(catalog).Update(dto));

    [HttpDelete("Delete/{id}")]
    public IActionResult Delete(string catalog, int id)
        => Ok(Resolve(catalog).Delete(id));

    [HttpGet("GetAllWithPagination")]
    public IActionResult GetAllWithPagination(string catalog, int page, int pageSize)
        => Ok(Resolve(catalog).GetAllWithPagination(page, pageSize));

    [HttpGet("Count")]
    public IActionResult Count(string catalog)
        => Ok(Resolve(catalog).Count());

    // =================================================================
    //  ASÍNCRONOS
    // =================================================================

    [HttpGet("GetAllAsync")]
    public async Task<IActionResult> GetAllAsync(string catalog)
        => Ok(await Resolve(catalog).GetAllAsync());

    [HttpGet("GetByIdAsync/{id}")]
    public async Task<IActionResult> GetByIdAsync(string catalog, int id)
        => Ok(await Resolve(catalog).GetAsync(id));

    [HttpPost("InsertAsync")]
    public async Task<IActionResult> InsertAsync(string catalog, [FromBody] GenericCatalogDTO dto)
        => Ok(await Resolve(catalog).InsertAsync(dto));

    [HttpPut("UpdateAsync")]
    public async Task<IActionResult> UpdateAsync(string catalog, [FromBody] GenericCatalogDTO dto)
        => Ok(await Resolve(catalog).UpdateAsync(dto));

    [HttpDelete("DeleteAsync/{id}")]
    public async Task<IActionResult> DeleteAsync(string catalog, int id)
        => Ok(await Resolve(catalog).DeleteAsync(id));

    [HttpGet("GetAllWithPaginationAsync")]
    public async Task<IActionResult> GetAllWithPaginationAsync(string catalog, int page, int pageSize)
        => Ok(await Resolve(catalog).GetAllWithPaginationAsync(page, pageSize));

    [HttpGet("CountAsync")]
    public async Task<IActionResult> CountAsync(string catalog)
        => Ok(await Resolve(catalog).CountAsync());
}

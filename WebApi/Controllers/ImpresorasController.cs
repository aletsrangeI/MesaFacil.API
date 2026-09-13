using Common;
using DTO.ConfiguracionImpresora;
using DTO.Impresoras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UseCases.Impresoras;

namespace WebApi.Controllers;

/// <summary>
/// Spec 023: Asistente de Autodiagnóstico y Auto-Recuperación de Impresoras Térmicas.
/// CRUD de configuración de impresoras + diagnóstico ESC/POS (socket TCP 9100) + test print.
/// </summary>
[Authorize]
[Route("api/impresoras")]
[ApiController]
public class ImpresorasController : ControllerBase
{
    private readonly IConfiguracionImpresoraService _impresoraService;
    private readonly IImpresoraDiagnosticService _diagnosticoService;

    public ImpresorasController(
        IConfiguracionImpresoraService impresoraService,
        IImpresoraDiagnosticService diagnosticoService)
    {
        _impresoraService = impresoraService;
        _diagnosticoService = diagnosticoService;
    }

    [HttpGet("sucursal/{idSucursal}")]
    public async Task<ActionResult<Response<IEnumerable<ConfiguracionImpresoraDTO>>>> GetBySucursal(int idSucursal)
    {
        var response = await _impresoraService.GetBySucursalAsync(idSucursal);
        return Ok(response);
    }

    [HttpPost("Insert")]
    public async Task<ActionResult<Response<int>>> Insert([FromBody] ConfiguracionImpresoraDTO dto)
    {
        var response = await _impresoraService.InsertAsync(dto);
        return Ok(response);
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<Response<IEnumerable<ConfiguracionImpresoraDTO>>>> GetAll()
    {
        var response = await _impresoraService.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<Response<ConfiguracionImpresoraDTO>>> GetById(int id)
    {
        var response = await _impresoraService.GetByIdAsync(id);
        return Ok(response);
    }

    [HttpPut("Update")]
    public async Task<ActionResult<Response<bool>>> Update([FromBody] ConfiguracionImpresoraDTO dto)
    {
        var response = await _impresoraService.UpdateAsync(dto);
        return Ok(response);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<Response<bool>>> Delete(int id)
    {
        var response = await _impresoraService.DeleteAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// POST /api/impresoras/diagnostico/{idImpresora}. Forma de respuesta EXACTA definida en
    /// la sección 3 del spec.md (sin envolver en Response&lt;T&gt;).
    /// </summary>
    [HttpPost("diagnostico/{idImpresora}")]
    public async Task<ActionResult<DiagnosticoImpresoraResponseDTO>> Diagnostico(int idImpresora)
    {
        var resultado = await _diagnosticoService.DiagnosticarAsync(idImpresora);
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }

    /// <summary>
    /// POST /api/impresoras/test-print/{idImpresora}. Forma de respuesta EXACTA definida en
    /// la sección 3 del spec.md (sin envolver en Response&lt;T&gt;).
    /// </summary>
    [HttpPost("test-print/{idImpresora}")]
    public async Task<ActionResult<TestPrintResponseDTO>> TestPrint(int idImpresora)
    {
        var resultado = await _diagnosticoService.EnviarTestPrintAsync(idImpresora);
        if (resultado == null) return NotFound();
        return Ok(resultado);
    }
}

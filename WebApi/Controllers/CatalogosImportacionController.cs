using Common;
using DTO.Importacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UseCases.Importacion;

namespace WebApi.Controllers;

/// <summary>
/// Spec 022: Importador Inteligente de Menú y Catálogos (Excel / CSV).
/// Wizard de 3 pasos: descarga de plantilla ➔ preview de calidad ➔ confirmación atómica.
/// </summary>
[Authorize]
[Route("api/catalogos/importar-menu")]
[ApiController]
public class CatalogosImportacionController : ControllerBase
{
    private readonly IImportadorMenuService _importadorMenuService;

    public CatalogosImportacionController(IImportadorMenuService importadorMenuService)
    {
        _importadorMenuService = importadorMenuService;
    }

    /// <summary>
    /// GET /api/catalogos/importar-menu/plantilla - descarga Plantilla_Menu_MesaFacil.xlsx
    /// </summary>
    [HttpGet("plantilla")]
    public IActionResult Plantilla()
    {
        var bytes = _importadorMenuService.GenerarPlantilla();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            PlantillaMenuGeneratorService.NombreArchivo);
    }

    /// <summary>
    /// POST /api/catalogos/importar-menu/preview (multipart/form-data)
    /// </summary>
    [HttpPost("preview")]
    public async Task<ActionResult<Response<ImportarMenuPreviewResponseDTO>>> Preview(
        [FromForm] IFormFile archivo,
        [FromForm] string modo,
        [FromForm] int sucursalId)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest(new Response<ImportarMenuPreviewResponseDTO>
            {
                isSuccess = false,
                Message = "Debes adjuntar un archivo .xlsx o .csv."
            });
        }

        await using var stream = archivo.OpenReadStream();
        var response = await _importadorMenuService.PreviewAsync(stream, archivo.FileName, modo, sucursalId);
        return response.isSuccess ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// POST /api/catalogos/importar-menu/confirmar (application/json)
    /// </summary>
    [HttpPost("confirmar")]
    public async Task<ActionResult<Response<ConfirmarImportacionMenuResponseDTO>>> Confirmar(
        [FromBody] ConfirmarImportacionMenuRequestDTO dto)
    {
        var response = await _importadorMenuService.ConfirmarAsync(dto.TokenPreview, dto.Modo, dto.SucursalId);
        return response.isSuccess ? Ok(response) : BadRequest(response);
    }
}

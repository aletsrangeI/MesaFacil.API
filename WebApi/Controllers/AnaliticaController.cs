using Common;
using DTO.Analitica;
using Interface.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnaliticaController : ControllerBase
{
    private readonly IMenuEngineeringService _menuEngineeringService;

    public AnaliticaController(IMenuEngineeringService menuEngineeringService)
    {
        _menuEngineeringService = menuEngineeringService;
    }

    /// <summary>
    /// Genera la Matriz de Ingeniería de Menú (Kasavana &amp; Smith / BCG adaptada a restaurantes)
    /// clasificando platillos en Estrellas, Caballos de Batalla, Puzzles y Perros.
    /// </summary>
    [HttpGet("ingenieria-menu")]
    public async Task<ActionResult<Response<MenuEngineeringReportDTO>>> GetIngenieriaMenu(
        [FromQuery] int? idSucursal = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null,
        [FromQuery] int? idCategoria = null)
    {
        var response = new Response<MenuEngineeringReportDTO>();
        try
        {
            var fInicio = fechaInicio ?? DateTime.UtcNow.AddDays(-30);
            var fFin = fechaFin ?? DateTime.UtcNow;

            // Asegurar que fechaFin cubra el final del día si no tiene componente de hora
            if (fFin.TimeOfDay == TimeSpan.Zero)
            {
                fFin = fFin.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            var result = await _menuEngineeringService.ObtenerReporteIngenieriaMenuAsync(
                idSucursal, fInicio, fFin, idCategoria);

            response.Data = result;
            response.isSuccess = true;
            response.Message = "Reporte de ingeniería de menú generado con éxito.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.isSuccess = false;
            response.Message = $"Error al generar análisis de menú: {ex.Message}";
            return StatusCode(500, response);
        }
    }
}

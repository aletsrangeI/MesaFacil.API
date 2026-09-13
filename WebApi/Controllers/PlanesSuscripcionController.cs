using Common;
using DTO.Suscripcion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

/// <summary>
/// Spec 021: catálogo comercial público de tiers de suscripción (tabla de precios /
/// pantalla de upgrade). No requiere autenticación: se usa también en la landing comercial.
/// </summary>
[AllowAnonymous]
[Route("api/planes-suscripcion")]
[ApiController]
public class PlanesSuscripcionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PlanesSuscripcionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<Response<IEnumerable<CatPlanSuscripcionDTO>>>> GetPlanes()
    {
        var planes = await _context.CatPlanesSuscripcion
            .Where(p => p.IsActive)
            .OrderBy(p => p.Id)
            .Select(p => new CatPlanSuscripcionDTO
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                PrecioMensualMxn = p.PrecioMensualMxn,
                PrecioAnualMxn = p.PrecioAnualMxn,
                MaxSucursales = p.MaxSucursales,
                MaxKdsBase = p.MaxKdsBase,
                PermiteMesas = p.PermiteMesas,
                PermiteSplitBill = p.PermiteSplitBill,
                PermiteRecetas = p.PermiteRecetas,
                PermiteCfdiXml = p.PermiteCfdiXml,
                PermiteCxP = p.PermiteCxP
            })
            .ToListAsync();

        return Ok(new Response<IEnumerable<CatPlanSuscripcionDTO>>
        {
            Data = planes,
            isSuccess = true,
            Message = "Catálogo de planes de suscripción obtenido con éxito."
        });
    }
}

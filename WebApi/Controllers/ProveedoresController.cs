using System.Security.Claims;
using Common;
using Domain.Entities;
using DTO.Compras;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProveedoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProveedoresController(ApplicationDbContext context)
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

    [HttpGet]
    public async Task<ActionResult<Response<List<ProveedorDTO>>>> GetProveedores(
        [FromQuery] string? buscar = null,
        [FromQuery] bool? activo = null,
        [FromQuery] int? idEmpresa = null)
    {
        var response = new Response<List<ProveedorDTO>>();

        var query = _context.Proveedores
            .Include(p => p.Compras)
            .AsNoTracking()
            .AsQueryable();

        if (idEmpresa.HasValue && idEmpresa.Value > 0)
            query = query.Where(p => p.IdEmpresa == idEmpresa.Value);

        if (activo.HasValue)
            query = query.Where(p => p.IsActive == activo.Value);

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.RazonSocial.ToLower().Contains(term) ||
                p.RFC.ToLower().Contains(term) ||
                (p.NombreComercial != null && p.NombreComercial.ToLower().Contains(term)) ||
                (p.Contacto != null && p.Contacto.ToLower().Contains(term)));
        }

        var list = await query
            .OrderBy(p => p.RazonSocial)
            .Select(p => new ProveedorDTO
            {
                Id = p.Id,
                IdEmpresa = p.IdEmpresa,
                RFC = p.RFC,
                RazonSocial = p.RazonSocial,
                NombreComercial = p.NombreComercial,
                Email = p.Email,
                Telefono = p.Telefono,
                Contacto = p.Contacto,
                Direccion = p.Direccion,
                RegimenFiscal = p.RegimenFiscal,
                DiasCredito = p.DiasCredito,
                Banco = p.Banco,
                CuentaBancaria = p.CuentaBancaria,
                IsActive = p.IsActive,
                TotalCompras = p.Compras.Count(c => c.Estado != "Cancelada"),
                MontoTotalComprado = p.Compras.Where(c => c.Estado != "Cancelada").Sum(c => c.Total)
            })
            .ToListAsync();

        response.Data = list;
        response.isSuccess = true;
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Response<ProveedorDTO>>> GetProveedorPorId(int id)
    {
        var response = new Response<ProveedorDTO>();

        var p = await _context.Proveedores
            .Include(prov => prov.Compras)
            .AsNoTracking()
            .FirstOrDefaultAsync(prov => prov.Id == id);

        if (p == null)
        {
            response.isSuccess = false;
            response.Message = "Proveedor no encontrado.";
            return NotFound(response);
        }

        response.Data = new ProveedorDTO
        {
            Id = p.Id,
            IdEmpresa = p.IdEmpresa,
            RFC = p.RFC,
            RazonSocial = p.RazonSocial,
            NombreComercial = p.NombreComercial,
            Email = p.Email,
            Telefono = p.Telefono,
            Contacto = p.Contacto,
            Direccion = p.Direccion,
            RegimenFiscal = p.RegimenFiscal,
            DiasCredito = p.DiasCredito,
            Banco = p.Banco,
            CuentaBancaria = p.CuentaBancaria,
            IsActive = p.IsActive,
            TotalCompras = p.Compras.Count(c => c.Estado != "Cancelada"),
            MontoTotalComprado = p.Compras.Where(c => c.Estado != "Cancelada").Sum(c => c.Total)
        };
        response.isSuccess = true;
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<Response<ProveedorDTO>>> CrearProveedor([FromBody] CrearProveedorDTO dto)
    {
        var response = new Response<ProveedorDTO>();

        // Validación estricta de RFC SAT
        if (!RfcHelper.EsRfcValido(dto.RFC, out var errorRfc))
        {
            response.isSuccess = false;
            response.Message = errorRfc;
            return BadRequest(response);
        }

        if (string.IsNullOrWhiteSpace(dto.RazonSocial))
        {
            response.isSuccess = false;
            response.Message = "La Razón Social es requerida.";
            return BadRequest(response);
        }

        var rfcNorm = dto.RFC.Trim().ToUpperInvariant();
        var existeRfc = await _context.Proveedores
            .AnyAsync(p => p.RFC == rfcNorm && p.IsActive);

        if (existeRfc)
        {
            response.isSuccess = false;
            response.Message = $"Ya existe un proveedor registrado con el RFC '{rfcNorm}'.";
            return BadRequest(response);
        }

        var proveedor = new Proveedor
        {
            IdEmpresa = dto.IdEmpresa > 0 ? dto.IdEmpresa : 1,
            RFC = rfcNorm,
            RazonSocial = dto.RazonSocial.Trim(),
            NombreComercial = dto.NombreComercial?.Trim(),
            Email = dto.Email?.Trim(),
            Telefono = dto.Telefono?.Trim(),
            Contacto = dto.Contacto?.Trim(),
            Direccion = dto.Direccion?.Trim(),
            RegimenFiscal = dto.RegimenFiscal?.Trim(),
            DiasCredito = dto.DiasCredito >= 0 ? dto.DiasCredito : 0,
            Banco = dto.Banco?.Trim(),
            CuentaBancaria = dto.CuentaBancaria?.Trim(),
            IsActive = true
        };

        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();

        response.Data = new ProveedorDTO
        {
            Id = proveedor.Id,
            IdEmpresa = proveedor.IdEmpresa,
            RFC = proveedor.RFC,
            RazonSocial = proveedor.RazonSocial,
            NombreComercial = proveedor.NombreComercial,
            Email = proveedor.Email,
            Telefono = proveedor.Telefono,
            Contacto = proveedor.Contacto,
            Direccion = proveedor.Direccion,
            RegimenFiscal = proveedor.RegimenFiscal,
            DiasCredito = proveedor.DiasCredito,
            Banco = proveedor.Banco,
            CuentaBancaria = proveedor.CuentaBancaria,
            IsActive = proveedor.IsActive,
            TotalCompras = 0,
            MontoTotalComprado = 0
        };
        response.isSuccess = true;
        response.Message = "Proveedor registrado exitosamente.";
        return CreatedAtAction(nameof(GetProveedorPorId), new { id = proveedor.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Response<ProveedorDTO>>> ActualizarProveedor(int id, [FromBody] ActualizarProveedorDTO dto)
    {
        var response = new Response<ProveedorDTO>();

        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null)
        {
            response.isSuccess = false;
            response.Message = "Proveedor no encontrado.";
            return NotFound(response);
        }

        if (!RfcHelper.EsRfcValido(dto.RFC, out var errorRfc))
        {
            response.isSuccess = false;
            response.Message = errorRfc;
            return BadRequest(response);
        }

        var rfcNorm = dto.RFC.Trim().ToUpperInvariant();
        var existeRfc = await _context.Proveedores
            .AnyAsync(p => p.RFC == rfcNorm && p.Id != id && p.IsActive);

        if (existeRfc)
        {
            response.isSuccess = false;
            response.Message = $"Ya existe otro proveedor registrado con el RFC '{rfcNorm}'.";
            return BadRequest(response);
        }

        proveedor.RFC = rfcNorm;
        proveedor.RazonSocial = dto.RazonSocial.Trim();
        proveedor.NombreComercial = dto.NombreComercial?.Trim();
        proveedor.Email = dto.Email?.Trim();
        proveedor.Telefono = dto.Telefono?.Trim();
        proveedor.Contacto = dto.Contacto?.Trim();
        proveedor.Direccion = dto.Direccion?.Trim();
        proveedor.RegimenFiscal = dto.RegimenFiscal?.Trim();
        proveedor.DiasCredito = dto.DiasCredito >= 0 ? dto.DiasCredito : 0;
        proveedor.Banco = dto.Banco?.Trim();
        proveedor.CuentaBancaria = dto.CuentaBancaria?.Trim();
        proveedor.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        response.Data = new ProveedorDTO
        {
            Id = proveedor.Id,
            IdEmpresa = proveedor.IdEmpresa,
            RFC = proveedor.RFC,
            RazonSocial = proveedor.RazonSocial,
            NombreComercial = proveedor.NombreComercial,
            Email = proveedor.Email,
            Telefono = proveedor.Telefono,
            Contacto = proveedor.Contacto,
            Direccion = proveedor.Direccion,
            RegimenFiscal = proveedor.RegimenFiscal,
            DiasCredito = proveedor.DiasCredito,
            Banco = proveedor.Banco,
            CuentaBancaria = proveedor.CuentaBancaria,
            IsActive = proveedor.IsActive
        };
        response.isSuccess = true;
        response.Message = "Proveedor actualizado exitosamente.";
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Response<bool>>> EliminarProveedor(int id)
    {
        var response = new Response<bool>();

        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null)
        {
            response.isSuccess = false;
            response.Message = "Proveedor no encontrado.";
            return NotFound(response);
        }

        // Soft delete
        proveedor.IsActive = false;
        await _context.SaveChangesAsync();

        response.Data = true;
        response.isSuccess = true;
        response.Message = "Proveedor deshabilitado exitosamente.";
        return Ok(response);
    }

    #region Mapeos SAT ➔ Insumos

    [HttpGet("{id:int}/Mapeos")]
    public async Task<ActionResult<Response<List<MapeoInsumoProveedorDTO>>>> GetMapeosProveedor(int id)
    {
        var response = new Response<List<MapeoInsumoProveedorDTO>>();

        var mapeos = await _context.MapeosInsumoProveedor
            .Include(m => m.Proveedor)
            .Include(m => m.Insumo)
                .ThenInclude(i => i!.UnidadMedidaBase)
            .Where(m => m.IdProveedor == id && m.IsActive)
            .OrderBy(m => m.DescripcionSAT)
            .Select(m => new MapeoInsumoProveedorDTO
            {
                Id = m.Id,
                IdProveedor = m.IdProveedor,
                ProveedorNombre = m.Proveedor != null ? m.Proveedor.RazonSocial : string.Empty,
                DescripcionSAT = m.DescripcionSAT,
                ClaveProdServ = m.ClaveProdServ,
                UnidadSAT = m.UnidadSAT,
                IdInsumo = m.IdInsumo,
                InsumoCodigo = m.Insumo != null ? m.Insumo.Codigo : null,
                InsumoNombre = m.Insumo != null ? m.Insumo.Nombre : string.Empty,
                UnidadMedidaNombre = m.Insumo != null && m.Insumo.UnidadMedidaBase != null ? m.Insumo.UnidadMedidaBase.Nombre : "PZA",
                FactorConversion = m.FactorConversion,
                FechaRegistro = m.FechaRegistro,
                FechaUltimaCompra = m.FechaUltimaCompra
            })
            .ToListAsync();

        response.Data = mapeos;
        response.isSuccess = true;
        return Ok(response);
    }

    [HttpPost("{id:int}/Mapeos")]
    public async Task<ActionResult<Response<MapeoInsumoProveedorDTO>>> GuardarMapeo(int id, [FromBody] CrearOActualizarMapeoDTO dto)
    {
        var response = new Response<MapeoInsumoProveedorDTO>();

        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null)
        {
            response.isSuccess = false;
            response.Message = "Proveedor no encontrado.";
            return NotFound(response);
        }

        var insumo = await _context.Insumos
            .Include(i => i.UnidadMedidaBase)
            .FirstOrDefaultAsync(i => i.Id == dto.IdInsumo);

        if (insumo == null || !insumo.IsActive)
        {
            response.isSuccess = false;
            response.Message = "Insumo no válido o inactivo.";
            return BadRequest(response);
        }

        var mapeoExistente = await _context.MapeosInsumoProveedor
            .FirstOrDefaultAsync(m => m.IdProveedor == id &&
                                     m.ClaveProdServ == dto.ClaveProdServ.Trim() &&
                                     m.DescripcionSAT == dto.DescripcionSAT.Trim());

        if (mapeoExistente != null)
        {
            mapeoExistente.IdInsumo = dto.IdInsumo;
            mapeoExistente.FactorConversion = dto.FactorConversion > 0 ? dto.FactorConversion : 1.0m;
            mapeoExistente.UnidadSAT = dto.UnidadSAT?.Trim();
            mapeoExistente.IsActive = true;
        }
        else
        {
            mapeoExistente = new MapeoInsumoProveedor
            {
                IdProveedor = id,
                ClaveProdServ = dto.ClaveProdServ.Trim(),
                DescripcionSAT = dto.DescripcionSAT.Trim(),
                UnidadSAT = dto.UnidadSAT?.Trim(),
                IdInsumo = dto.IdInsumo,
                FactorConversion = dto.FactorConversion > 0 ? dto.FactorConversion : 1.0m,
                FechaRegistro = DateTime.UtcNow,
                IsActive = true
            };
            _context.MapeosInsumoProveedor.Add(mapeoExistente);
        }

        await _context.SaveChangesAsync();

        response.Data = new MapeoInsumoProveedorDTO
        {
            Id = mapeoExistente.Id,
            IdProveedor = id,
            ProveedorNombre = proveedor.RazonSocial,
            DescripcionSAT = mapeoExistente.DescripcionSAT,
            ClaveProdServ = mapeoExistente.ClaveProdServ,
            UnidadSAT = mapeoExistente.UnidadSAT,
            IdInsumo = insumo.Id,
            InsumoCodigo = insumo.Codigo,
            InsumoNombre = insumo.Nombre,
            UnidadMedidaNombre = insumo.UnidadMedidaBase?.Nombre ?? "PZA",
            FactorConversion = mapeoExistente.FactorConversion,
            FechaRegistro = mapeoExistente.FechaRegistro,
            FechaUltimaCompra = mapeoExistente.FechaUltimaCompra
        };
        response.isSuccess = true;
        response.Message = "Asociación inteligente guardada exitosamente.";
        return Ok(response);
    }

    [HttpDelete("{id:int}/Mapeos/{idMapeo:int}")]
    public async Task<ActionResult<Response<bool>>> EliminarMapeo(int id, int idMapeo)
    {
        var response = new Response<bool>();

        var mapeo = await _context.MapeosInsumoProveedor
            .FirstOrDefaultAsync(m => m.Id == idMapeo && m.IdProveedor == id);

        if (mapeo == null)
        {
            response.isSuccess = false;
            response.Message = "Mapeo no encontrado.";
            return NotFound(response);
        }

        _context.MapeosInsumoProveedor.Remove(mapeo);
        await _context.SaveChangesAsync();

        response.Data = true;
        response.isSuccess = true;
        response.Message = "Mapeo eliminado exitosamente.";
        return Ok(response);
    }

    #endregion
}

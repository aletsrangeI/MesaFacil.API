using Common;
using Domain.Entities;
using DTO.ConfiguracionImpresora;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.Impresoras;

/// <summary>
/// Spec 023: CRUD de configuración de impresoras térmicas por sucursal.
/// </summary>
public interface IConfiguracionImpresoraService
{
    Task<Response<int>> InsertAsync(ConfiguracionImpresoraDTO dto);
    Task<Response<bool>> UpdateAsync(ConfiguracionImpresoraDTO dto);
    Task<Response<bool>> DeleteAsync(int id);
    Task<Response<ConfiguracionImpresoraDTO>> GetByIdAsync(int id);
    Task<Response<IEnumerable<ConfiguracionImpresoraDTO>>> GetAllAsync();
    Task<Response<IEnumerable<ConfiguracionImpresoraDTO>>> GetBySucursalAsync(int idSucursal);
}

public class ConfiguracionImpresoraService : IConfiguracionImpresoraService
{
    private readonly ApplicationDbContext _context;

    public ConfiguracionImpresoraService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Response<int>> InsertAsync(ConfiguracionImpresoraDTO dto)
    {
        var response = new Response<int>();
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                response.Message = "El nombre de la impresora es requerido.";
                return response;
            }

            var sucursal = await _context.Sucursales.FindAsync(dto.IdSucursal);
            if (sucursal == null)
            {
                response.Message = "La sucursal especificada no existe.";
                return response;
            }

            var entity = MapToEntity(dto, new ConfiguracionImpresora());
            entity.IsActive = true;

            await _context.ConfiguracionesImpresora.AddAsync(entity);
            await _context.SaveChangesAsync();

            response.Data = entity.Id;
            response.isSuccess = true;
            response.Message = "Impresora registrada correctamente.";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(ConfiguracionImpresoraDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = await _context.ConfiguracionesImpresora.FindAsync(dto.Id);
            if (entity == null)
            {
                response.Message = "La impresora especificada no existe.";
                return response;
            }

            MapToEntity(dto, entity);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Impresora modificada correctamente.";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var response = new Response<bool>();
        try
        {
            var entity = await _context.ConfiguracionesImpresora.FindAsync(id);
            if (entity == null)
            {
                response.Message = "La impresora especificada no existe.";
                return response;
            }

            _context.ConfiguracionesImpresora.Remove(entity);
            await _context.SaveChangesAsync();

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Impresora eliminada correctamente.";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<Response<ConfiguracionImpresoraDTO>> GetByIdAsync(int id)
    {
        var response = new Response<ConfiguracionImpresoraDTO>();
        try
        {
            var entity = await _context.ConfiguracionesImpresora.FindAsync(id);
            if (entity == null)
            {
                response.Message = "La impresora especificada no existe.";
                return response;
            }

            response.Data = MapToDto(entity);
            response.isSuccess = true;
            response.Message = "Impresora encontrada.";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<Response<IEnumerable<ConfiguracionImpresoraDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<ConfiguracionImpresoraDTO>>();
        try
        {
            var lista = await _context.ConfiguracionesImpresora.ToListAsync();
            response.Data = lista.Select(MapToDto).ToList();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<Response<IEnumerable<ConfiguracionImpresoraDTO>>> GetBySucursalAsync(int idSucursal)
    {
        var response = new Response<IEnumerable<ConfiguracionImpresoraDTO>>();
        try
        {
            var lista = await _context.ConfiguracionesImpresora
                .Where(x => x.IdSucursal == idSucursal)
                .ToListAsync();
            response.Data = lista.Select(MapToDto).ToList();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }
        return response;
    }

    private static ConfiguracionImpresoraDTO MapToDto(ConfiguracionImpresora entity) => new()
    {
        Id = entity.Id,
        IdSucursal = entity.IdSucursal,
        Nombre = entity.Nombre,
        TipoConexion = entity.TipoConexion.ToString(),
        AnchoPapel = (int)entity.AnchoPapel,
        DireccionIp = entity.DireccionIp,
        Puerto = entity.Puerto,
        AperturaCajon = entity.AperturaCajon,
        Autocorte = entity.Autocorte,
        EstacionAsociada = entity.EstacionAsociada
    };

    private static ConfiguracionImpresora MapToEntity(ConfiguracionImpresoraDTO dto, ConfiguracionImpresora entity)
    {
        entity.IdSucursal = dto.IdSucursal;
        entity.Nombre = dto.Nombre;
        entity.TipoConexion = Enum.TryParse<TipoConexionImpresora>(dto.TipoConexion, true, out var tipo)
            ? tipo
            : TipoConexionImpresora.RedLAN;
        entity.AnchoPapel = dto.AnchoPapel == 58 ? AnchoPapelImpresora.Mm58 : AnchoPapelImpresora.Mm80;
        entity.DireccionIp = dto.DireccionIp;
        entity.Puerto = dto.Puerto > 0 ? dto.Puerto : 9100;
        entity.AperturaCajon = dto.AperturaCajon;
        entity.Autocorte = dto.Autocorte;
        entity.EstacionAsociada = dto.EstacionAsociada;
        return entity;
    }
}

using AutoMapper;
using Common;
using Domain.Entities;
using DTO.CorteCaja;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UseCases.CorteCajas;

public class CorteCajaApplication : ICorteCajaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly CorteCajaDTOValidator _validationRules;
    private readonly IAppLogger<CorteCajaApplication> _logger;

    public CorteCajaApplication(
        IUnitOfWork unitOfWork,
        ApplicationDbContext context,
        IMapper mapper,
        CorteCajaDTOValidator validationRules,
        IAppLogger<CorteCajaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(CorteCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CorteCaja>(dto);
            response.Data = _unitOfWork.CorteCajas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(CorteCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CorteCaja>(dto);
            response.Data = _unitOfWork.CorteCajas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Delete(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = _unitOfWork.CorteCajas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<CorteCajaDTO> Get(int id)
    {
        var response = new Response<CorteCajaDTO>();
        try
        {
            var entity = _unitOfWork.CorteCajas.Get(id);
            response.Data = _mapper.Map<CorteCajaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<CorteCajaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CorteCajaDTO>>();
        try
        {
            var list = _unitOfWork.CorteCajas.GetAll();
            response.Data = _mapper.Map<IEnumerable<CorteCajaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<CorteCajaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CorteCajaDTO>>();
        try
        {
            var list = _unitOfWork.CorteCajas.GetAllWithPagination(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CorteCajaDTO>>(list);
                response.isSuccess = true;
                response.Message = "CorteCajas encontradas";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<int> Count()
    {
        var response = new Response<int>();
        try
        {
            response.Data = _unitOfWork.CorteCajas.Count();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    #endregion

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(CorteCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CorteCaja>(dto);
            response.Data = await _unitOfWork.CorteCajas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CorteCajaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CorteCaja>(dto);
            response.Data = await _unitOfWork.CorteCajas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> DeleteAsync(int id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = await _unitOfWork.CorteCajas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<CorteCajaDTO>> GetAsync(int id)
    {
        var response = new Response<CorteCajaDTO>();
        try
        {
            var entity = await _unitOfWork.CorteCajas.GetAsync(id);
            response.Data = _mapper.Map<CorteCajaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CorteCaja encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<CorteCajaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CorteCajaDTO>>();
        try
        {
            var list = await _unitOfWork.CorteCajas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CorteCajaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CorteCajaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CorteCajaDTO>>();
        try
        {
            var list = await _unitOfWork.CorteCajas.GetAllWithPaginationAsync(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CorteCajaDTO>>(list);
                response.isSuccess = true;
                response.Message = "CorteCajas encontradas";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<int>> CountAsync()
    {
        var response = new Response<int>();
        try
        {
            response.Data = await _unitOfWork.CorteCajas.CountAsync();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<ResumenCorteDTO>> ObtenerResumenActualAsync(int? idSucursal, int? idTurno)
    {
        var response = new Response<ResumenCorteDTO>();
        try
        {
            int sucursalId = idSucursal ?? 1;

            Turno? turno = null;
            if (idTurno.HasValue && idTurno.Value > 0)
            {
                turno = await _context.Turnos.FirstOrDefaultAsync(t => t.Id == idTurno.Value);
            }
            else
            {
                turno = await _context.Turnos
                    .Where(t => t.IdSucursal == sucursalId && t.Cierre == null)
                    .OrderByDescending(t => t.Apertura)
                    .FirstOrDefaultAsync();
            }

            DateTime fechaInicio;
            decimal cajaInicial = 0;
            if (turno != null)
            {
                fechaInicio = turno.Apertura;
                cajaInicial = turno.CajaInicial;
            }
            else
            {
                var ultimoCorte = await _context.CorteCajas
                    .Where(c => c.IdSucursal == sucursalId)
                    .OrderByDescending(c => c.FechaFin)
                    .FirstOrDefaultAsync();

                fechaInicio = ultimoCorte?.FechaFin ?? DateTime.UtcNow.Date;
            }

            DateTime fechaFin = DateTime.UtcNow;

            // Sincronizar automáticamente pedidos de delivery/mostrador entregados sin pago registrado en este turno
            var pedidosSinPago = await _context.Pedidos
                .Include(p => p.Cuentas).ThenInclude(c => c.Pagos)
                .Include(p => p.Detalles)
                .Include(p => p.TipoPedido)
                .Where(p => p.IsActive
                    && p.AbiertoEn >= fechaInicio
                    && (p.IdMesa == null || (p.TipoPedido != null && !p.TipoPedido.IsComedor))
                    && (p.IdEstadoPedido >= 4 && p.IdEstadoPedido != 6)
                    && (!p.Cuentas.Any() || !p.Cuentas.Any(c => c.Pagos.Any(pg => pg.IsActive))))
                .ToListAsync();

            if (sucursalId > 0)
            {
                pedidosSinPago = pedidosSinPago.Where(p => p.IdSucursal == sucursalId).ToList();
            }

            if (pedidosSinPago.Any())
            {
                foreach (var ped in pedidosSinPago)
                {
                    decimal total = ped.Detalles.Where(d => d.IsActive && !d.Cancelado).Sum(d => d.Cantidad * d.PrecioUnitario);
                    if (total <= 0) continue;

                    int metodoId = 1; // Default Efectivo
                    if (!string.IsNullOrEmpty(ped.CanalOrigen))
                    {
                        if (ped.CanalOrigen.Contains("uber", StringComparison.OrdinalIgnoreCase)) metodoId = 3;
                        else if (ped.CanalOrigen.Contains("rappi", StringComparison.OrdinalIgnoreCase)) metodoId = 4;
                        else if (ped.CanalOrigen.Contains("didi", StringComparison.OrdinalIgnoreCase)) metodoId = 5;
                    }

                    var cta = ped.Cuentas.FirstOrDefault(c => c.IsActive);
                    if (cta == null)
                    {
                        cta = new Domain.Entities.Cuenta
                        {
                            IdPedido = ped.Id,
                            Subtotal = total,
                            Total = total,
                            IdEstadoCuenta = 1,
                            IsActive = true,
                            CreatedAt = ped.AbiertoEn,
                            CreatedBy = "AutoLiquidacionDelivery"
                        };
                        _context.Cuentas.Add(cta);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        cta.IdEstadoCuenta = 1;
                    }

                    var pg = new Domain.Entities.Pago
                    {
                        IdCuenta = cta.Id,
                        Monto = total,
                        IdMetodoDePago = metodoId,
                        PagadoEn = ped.EntregadoEn ?? ped.AbiertoEn,
                        Referencia = ped.IdExterno ?? ped.CanalOrigen,
                        IsActive = true,
                        CreatedAt = ped.EntregadoEn ?? ped.AbiertoEn,
                        CreatedBy = "AutoLiquidacionDelivery",
                        Moneda = "MXN"
                    };
                    _context.Pagos.Add(pg);
                }
                await _context.SaveChangesAsync();
            }

            var pagosQuery = _context.Pagos
                .Include(p => p.Cuenta)
                    .ThenInclude(c => c.Pedido)
                .Include(p => p.MetodoDePago)
                .Where(p => p.PagadoEn >= fechaInicio && p.PagadoEn <= fechaFin && p.IsActive);

            if (sucursalId > 0)
            {
                pagosQuery = pagosQuery.Where(p => p.Cuenta.Pedido.IdSucursal == sucursalId);
            }

            var pagos = await pagosQuery.ToListAsync();

            var pagosEfectivo = pagos.Where(p => p.IdMetodoDePago == 1 || (p.MetodoDePago != null && p.MetodoDePago.Descripcion.Contains("efectivo", StringComparison.OrdinalIgnoreCase))).ToList();
            var pagosTarjeta = pagos.Where(p => p.IdMetodoDePago == 2 || (p.MetodoDePago != null && (p.MetodoDePago.Descripcion.Contains("tarjeta", StringComparison.OrdinalIgnoreCase) || p.MetodoDePago.Descripcion.Contains("card", StringComparison.OrdinalIgnoreCase)))).ToList();
            var pagosPlataformas = pagos.Where(p => (p.IdMetodoDePago >= 3 && p.IdMetodoDePago <= 5) || (p.MetodoDePago != null && (p.MetodoDePago.Descripcion.Contains("uber", StringComparison.OrdinalIgnoreCase) || p.MetodoDePago.Descripcion.Contains("rappi", StringComparison.OrdinalIgnoreCase) || p.MetodoDePago.Descripcion.Contains("didi", StringComparison.OrdinalIgnoreCase)))).ToList();
            var pagosOtros = pagos.Except(pagosEfectivo).Except(pagosTarjeta).Except(pagosPlataformas).ToList();

            decimal totalEfectivo = pagosEfectivo.Sum(p => p.Monto);
            decimal totalTarjeta = pagosTarjeta.Sum(p => p.Monto);
            decimal totalPlataformas = pagosPlataformas.Sum(p => p.Monto);
            decimal totalOtros = pagosOtros.Sum(p => p.Monto);
            decimal totalVentas = totalEfectivo + totalTarjeta + totalPlataformas + totalOtros;

            decimal propinasEfectivo = pagosEfectivo.Sum(p => p.Propina);
            decimal propinasTarjeta = pagosTarjeta.Sum(p => p.Propina);
            decimal totalPropinas = propinasEfectivo + propinasTarjeta;
            decimal totalPagos = totalVentas + totalPropinas;

            decimal totalIngresos = 0;
            decimal totalEgresos = 0;
            if (turno != null)
            {
                totalEgresos = await _context.MovimientosCaja
                    .Where(m => m.IdTurno == turno.Id && m.Tipo == "Egreso" && m.IsActive)
                    .SumAsync(m => m.Monto);

                totalIngresos = await _context.MovimientosCaja
                    .Where(m => m.IdTurno == turno.Id && m.Tipo == "Ingreso" && m.IsActive)
                    .SumAsync(m => m.Monto);
            }

            decimal cajaEsperada = cajaInicial + totalEfectivo + totalIngresos - totalEgresos;
            var cuentasPagadasCount = pagos.Select(p => p.IdCuenta).Distinct().Count();

            response.Data = new ResumenCorteDTO
            {
                IdTurno = turno?.Id,
                IdSucursal = sucursalId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                CajaInicial = cajaInicial,
                TotalVentas = totalVentas,
                TotalPagos = totalPagos,
                TotalEfectivo = totalEfectivo,
                TotalTarjeta = totalTarjeta,
                TotalPlataformas = totalPlataformas,
                TotalOtros = totalOtros,
                TotalPropinas = totalPropinas,
                TotalPropinasTarjeta = propinasTarjeta,
                TotalPropinasEfectivo = propinasEfectivo,
                TotalIngresos = totalIngresos,
                TotalEgresos = totalEgresos,
                CajaEsperada = cajaEsperada,
                CantidadCuentasPagadas = cuentasPagadasCount
            };
            response.isSuccess = true;
            response.Message = "Resumen de corte generado correctamente.";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<CorteCajaDTO>> RealizarCorteAsync(RealizarCorteRequestDTO dto, int? idUsuario)
    {
        var response = new Response<CorteCajaDTO>();
        try
        {
            var resumenRes = await ObtenerResumenActualAsync(dto.IdSucursal, dto.IdTurno);
            if (!resumenRes.isSuccess || resumenRes.Data == null)
            {
                response.Message = "No se pudo calcular el resumen del corte: " + resumenRes.Message;
                return response;
            }

            var r = resumenRes.Data;
            decimal diferencia = dto.Declarado - r.CajaEsperada;

            var corte = new CorteCaja
            {
                IdTurno = r.IdTurno,
                IdSucursal = dto.IdSucursal,
                FechaInicio = r.FechaInicio,
                FechaFin = r.FechaFin,
                TotalVentas = r.TotalVentas,
                TotalPagos = r.TotalPagos,
                TotalEfectivo = r.TotalEfectivo,
                TotalTarjeta = r.TotalTarjeta,
                TotalEgresos = r.TotalEgresos,
                CajaEsperada = r.CajaEsperada,
                Declarado = dto.Declarado,
                Diferencia = diferencia,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = idUsuario,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = idUsuario?.ToString() ?? "System"
            };

            await _context.CorteCajas.AddAsync(corte);

            if (r.IdTurno.HasValue)
            {
                var turno = await _context.Turnos.FirstOrDefaultAsync(t => t.Id == r.IdTurno.Value);
                if (turno != null)
                {
                    turno.Cierre = DateTime.UtcNow;
                    turno.CajaFinal = dto.Declarado;
                    turno.UpdatedAt = DateTime.UtcNow;
                    turno.UpdatedBy = idUsuario?.ToString() ?? "System";
                }
            }

            await _context.SaveChangesAsync();

            response.Data = _mapper.Map<CorteCajaDTO>(corte);
            response.isSuccess = true;
            response.Message = "Corte de caja realizado exitosamente.";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<ResumenHistorialCortesDTO>> ObtenerHistorialAsync(DateTime? fechaInicio, DateTime? fechaFin, int? idSucursal)
    {
        var response = new Response<ResumenHistorialCortesDTO>();
        try
        {
            var query = _context.CorteCajas
                .Include(c => c.Sucursal)
                .Include(c => c.Turno)
                    .ThenInclude(t => t!.Usuario)
                .Include(c => c.CreadoPorUsuario)
                .Where(c => c.IsActive);

            if (idSucursal.HasValue && idSucursal.Value > 0)
            {
                query = query.Where(c => c.IdSucursal == idSucursal.Value);
            }

            if (fechaInicio.HasValue)
            {
                var fInicio = DateTime.SpecifyKind(fechaInicio.Value, DateTimeKind.Utc);
                query = query.Where(c => c.FechaFin >= fInicio);
            }

            if (fechaFin.HasValue)
            {
                var fFin = DateTime.SpecifyKind(fechaFin.Value, DateTimeKind.Utc);
                query = query.Where(c => c.FechaInicio <= fFin);
            }

            var cortes = await query
                .OrderByDescending(c => c.FechaFin)
                .ToListAsync();

            var items = new List<CorteCajaHistorialItemDTO>();

            foreach (var c in cortes)
            {
                string cajero = c.CreadoPorUsuario?.NombreCompleto
                    ?? c.Turno?.Usuario?.NombreCompleto
                    ?? "Cajero";

                decimal cajaInicial = c.Turno?.CajaInicial ?? 0;

                var pagosCorte = await _context.Pagos
                    .Include(p => p.MetodoDePago)
                    .Where(p => p.PagadoEn >= c.FechaInicio && p.PagadoEn <= c.FechaFin && p.IsActive)
                    .ToListAsync();

                var pagosTarjeta = pagosCorte.Where(p => p.IdMetodoDePago != 1 && (p.MetodoDePago == null || p.MetodoDePago.Descripcion.Contains("tarjeta", StringComparison.OrdinalIgnoreCase))).ToList();
                decimal propinasTarjeta = pagosTarjeta.Sum(p => p.Propina);
                decimal totalPropinas = pagosCorte.Sum(p => p.Propina);
                int cuentasPagadas = pagosCorte.Select(p => p.IdCuenta).Distinct().Count();

                decimal totalIngresos = 0;
                if (c.IdTurno.HasValue)
                {
                    totalIngresos = await _context.MovimientosCaja
                        .Where(m => m.IdTurno == c.IdTurno.Value && m.Tipo == "Ingreso" && m.IsActive)
                        .SumAsync(m => m.Monto);
                }

                items.Add(new CorteCajaHistorialItemDTO
                {
                    Id = c.Id,
                    IdTurno = c.IdTurno,
                    IdSucursal = c.IdSucursal ?? 1,
                    NombreSucursal = c.Sucursal?.Nombre ?? "Sucursal Principal",
                    NombreCajero = cajero,
                    FechaInicio = c.FechaInicio,
                    FechaFin = c.FechaFin,
                    CajaInicial = cajaInicial,
                    TotalVentas = c.TotalVentas,
                    TotalPagos = c.TotalPagos,
                    TotalEfectivo = c.TotalEfectivo,
                    TotalTarjeta = c.TotalTarjeta,
                    TotalOtros = Math.Max(0, c.TotalVentas - c.TotalEfectivo - c.TotalTarjeta),
                    TotalPropinas = totalPropinas,
                    TotalPropinasTarjeta = propinasTarjeta,
                    TotalPropinasEfectivo = Math.Max(0, totalPropinas - propinasTarjeta),
                    TotalIngresos = totalIngresos,
                    TotalEgresos = c.TotalEgresos,
                    CajaEsperada = c.CajaEsperada,
                    Declarado = c.Declarado,
                    Diferencia = c.Diferencia,
                    Observaciones = null,
                    CantidadCuentasPagadas = cuentasPagadas,
                    CreadoEn = c.CreadoEn
                });
            }

            response.Data = new ResumenHistorialCortesDTO
            {
                TotalVentas = items.Sum(i => i.TotalVentas),
                TotalEfectivo = items.Sum(i => i.TotalEfectivo),
                TotalTarjeta = items.Sum(i => i.TotalTarjeta),
                TotalPropinasTarjeta = items.Sum(i => i.TotalPropinasTarjeta),
                DiferenciaNeta = items.Sum(i => i.Diferencia),
                CantidadCortes = items.Count,
                Cortes = items
            };
            response.isSuccess = true;
            response.Message = "Historial de cortes obtenido correctamente.";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    #endregion
}
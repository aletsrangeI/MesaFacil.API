using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.Cuenta;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Cuentas;

public class CuentaApplication : ICuentaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly CuentaDTOValidator _validationRules;
    private readonly IAppLogger<CuentaApplication> _logger;
    private readonly ISupervisorPinSecurityService? _supervisorPinSecurityService;

    public CuentaApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        CuentaDTOValidator validationRules,
        IAppLogger<CuentaApplication> logger,
        ISupervisorPinSecurityService? supervisorPinSecurityService = null)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
        _supervisorPinSecurityService = supervisorPinSecurityService;
    }

    #region Metodos sincronos

    public Response<bool> Insert(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = _unitOfWork.Cuentas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = _unitOfWork.Cuentas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta modificado correctamente";
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
            response.Data = _unitOfWork.Cuentas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<CuentaDTO> Get(int id)
    {
        var response = new Response<CuentaDTO>();
        try
        {
            var entity = _unitOfWork.Cuentas.Get(id);
            response.Data = _mapper.Map<CuentaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Cuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<CuentaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CuentaDTO>>();
        try
        {
            var list = _unitOfWork.Cuentas.GetAll();
            response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<CuentaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CuentaDTO>>();
        try
        {
            var list = _unitOfWork.Cuentas.GetAllWithPagination(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
                response.isSuccess = true;
                response.Message = "Cuentas obtenidas";
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
            response.Data = _unitOfWork.Cuentas.Count();
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

    public async Task<Response<bool>> InsertAsync(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = await _unitOfWork.Cuentas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CuentaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Cuenta>(dto);
            response.Data = await _unitOfWork.Cuentas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta modificado correctamente";
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
            response.Data = await _unitOfWork.Cuentas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<CuentaDTO>> GetAsync(int id)
    {
        var response = new Response<CuentaDTO>();
        try
        {
            var entity = await _unitOfWork.Cuentas.GetAsync(id);
            response.Data = _mapper.Map<CuentaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Cuenta encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<CuentaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CuentaDTO>>();
        try
        {
            var list = await _unitOfWork.Cuentas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CuentaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CuentaDTO>>();
        try
        {
            var list = await _unitOfWork.Cuentas.GetAllWithPaginationAsync(page, pageSize);
            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CuentaDTO>>(list);
                response.isSuccess = true;
                response.Message = "Cuentas obtenidas";
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
            response.Data = await _unitOfWork.Cuentas.CountAsync();
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

    public async Task<Response<CuentaDTO>> GenerarCuentaAsync(Guid idPedido)
    {
        var response = new Response<CuentaDTO>();
        try
        {
            var pedido = await _unitOfWork.Pedidos.GetAsync(idPedido);
            if (pedido == null) throw new Exception("Pedido no encontrado");

            var detallesAll = await _unitOfWork.PedidoDetalles.GetAllAsync();
            var detalles = detallesAll.Where(d => d.IdPedido == idPedido && !d.Cancelado).ToList();

            decimal total = detalles.Sum(d => d.PrecioUnitario * d.Cantidad);
            decimal impuestoTotal = detalles.Sum(d => d.MontoImpuesto);
            if (impuestoTotal == 0) impuestoTotal = total - (total / 1.16m);
            decimal subtotal = total - impuestoTotal;

            // Buscar si ya existe una cuenta abierta para este pedido (IdEstadoCuenta == 2)
            var cuentasAll = await _unitOfWork.Cuentas.GetAllAsync();
            var cuentaExistente = cuentasAll
                .Where(c => c.IdPedido == idPedido && c.IdEstadoCuenta == 2 && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefault();

            Cuenta cuenta;
            if (cuentaExistente != null)
            {
                cuenta = cuentaExistente;
                cuenta.Subtotal = subtotal;
                cuenta.ImpuestoTotal = impuestoTotal;
                cuenta.Total = Math.Max(0m, (subtotal + impuestoTotal) - cuenta.DescuentoTotal);
                cuenta.UpdatedAt = DateTime.UtcNow;
                cuenta.UpdatedBy = "System";
                await _unitOfWork.Cuentas.UpdateAsync(cuenta);
            }
            else
            {
                cuenta = new Cuenta
                {
                    IdPedido = idPedido,
                    Subtotal = subtotal,
                    ImpuestoTotal = impuestoTotal,
                    DescuentoTotal = 0m,
                    Total = total,
                    IdEstadoCuenta = 2, // Abierta
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Cuentas.InsertAsync(cuenta);
            }

            // Consultar los pagos registrados para esta cuenta
            var pagosAll = await _unitOfWork.Pagos.GetAllAsync();
            var pagosCuenta = pagosAll.Where(p => p.IdCuenta == cuenta.Id && p.IsActive).OrderBy(p => p.PagadoEn).ToList();

            decimal totalPagado = pagosCuenta.Sum(p => p.Monto);
            decimal saldoRestante = Math.Max(0, cuenta.Total - totalPagado);

            var dto = _mapper.Map<CuentaDTO>(cuenta);
            dto.TotalPagado = totalPagado;
            dto.SaldoRestante = saldoRestante;
            dto.PagosRealizados = pagosCuenta.Select(p => new PagoResumenDTO
            {
                Id = p.Id,
                Monto = p.Monto,
                Propina = p.Propina,
                MetodoDePago = p.IdMetodoDePago == 1 ? "Efectivo" : "Tarjeta",
                PagadoEn = p.PagadoEn
            }).ToList();

            // Spec 028: Buscar evento de descuento reciente para enriquecer el DTO de la cuenta
            if (_unitOfWork.EventosPedido != null)
            {
                var eventosAll = await _unitOfWork.EventosPedido.GetAllAsync();
                if (eventosAll != null)
                {
                    var eventoDescuento = eventosAll
                        .Where(e => e.IdPedido == idPedido && (e.TipoEvento == "DescuentoAutorizado" || e.TipoEvento == "DescuentoAplicado") && e.IsActive)
                        .OrderByDescending(e => e.CreatedAt)
                        .FirstOrDefault();

                    if (eventoDescuento != null)
                    {
                        dto.PorcentajeDescuento = eventoDescuento.PorcentajeDescuento ?? 0m;
                        if (!string.IsNullOrWhiteSpace(eventoDescuento.Payload))
                        {
                            try
                            {
                                using var doc = System.Text.Json.JsonDocument.Parse(eventoDescuento.Payload);
                                if (doc.RootElement.TryGetProperty("Motivo", out var m)) dto.MotivoDescuento = m.GetString();
                            }
                            catch { }
                        }

                        if (eventoDescuento.IdUsuarioSupervisor.HasValue && _unitOfWork.Usuarios != null)
                        {
                            var supervisor = await _unitOfWork.Usuarios.GetAsync(eventoDescuento.IdUsuarioSupervisor.Value);
                            if (supervisor != null)
                            {
                                dto.AutorizadoPor = supervisor.NombreCompleto;
                            }
                        }
                    }
                }
            }

            // Metadatos de Mesa y Sucursal
            if (pedido.IdMesa.HasValue)
            {
                var mesa = await _unitOfWork.Mesas.GetAsync(pedido.IdMesa.Value);
                if (mesa != null)
                {
                    dto.MesaNombre = mesa.Codigo;
                }
            }

            var sucursal = await _unitOfWork.Sucursales.GetAsync(pedido.IdSucursal);
            if (sucursal != null)
            {
                dto.SucursalNombre = sucursal.Nombre;
            }

            // Desglose de ítems activos del pedido
            var modificadoresAll = await _unitOfWork.PedidoModificadores.GetAllAsync();
            var detallesIds = detalles.Select(d => d.Id).ToHashSet();
            var modifsPedido = modificadoresAll.Where(m => detallesIds.Contains(m.IdDetalle)).ToList();

            dto.Items = detalles.Select(d => new CuentaItemDTO
            {
                ProductoNombre = string.IsNullOrWhiteSpace(d.ProductoNombre) ? "Producto" : d.ProductoNombre,
                VarianteNombre = d.VarianteNombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Modificadores = modifsPedido
                    .Where(m => m.IdDetalle == d.Id)
                    .Select(m => m.OpcionNombre)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList()
            }).ToList();

            response.Data = dto;
            response.isSuccess = true;
            response.Message = "Cuenta generada exitosamente";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<CuentaDTO> GenerarCuenta(Guid idPedido)
    {
        return GenerarCuentaAsync(idPedido).GetAwaiter().GetResult();
    }

    public async Task<Response<CuentaDTO>> AplicarDescuentoAsync(AplicarDescuentoCuentaDTO dto)
    {
        var response = new Response<CuentaDTO>();
        try
        {
            var cuenta = await _unitOfWork.Cuentas.GetAsync(dto.IdCuenta);
            if (cuenta == null) throw new Exception("Cuenta no encontrada");

            int? supervisorId = null;
            if (dto.PorcentajeDescuento > 10.0m)
            {
                if (_supervisorPinSecurityService == null ||
                    !_supervisorPinSecurityService.ValidarTokenDescuento(dto.SupervisorAuthToken, cuenta.IdPedido, out supervisorId))
                {
                    response.isSuccess = false;
                    response.Message = "El descuento superior al 10% requiere autorización válida de supervisor.";
                    return response;
                }
            }

            decimal montoDescuento = dto.MontoDescuento;
            if (montoDescuento <= 0 && dto.PorcentajeDescuento > 0)
            {
                var baseTotal = cuenta.Subtotal + cuenta.ImpuestoTotal;
                montoDescuento = Math.Round(baseTotal * (dto.PorcentajeDescuento / 100m), 2);
            }

            cuenta.DescuentoTotal = montoDescuento;
            cuenta.Total = Math.Max(0m, (cuenta.Subtotal + cuenta.ImpuestoTotal) - cuenta.DescuentoTotal);
            cuenta.UpdatedAt = DateTime.UtcNow;
            cuenta.UpdatedBy = supervisorId.HasValue ? $"Supervisor #{supervisorId}" : "CuentaApplication";
            await _unitOfWork.Cuentas.UpdateAsync(cuenta);

            var eventoDescuento = new EventoPedido
            {
                IdPedido = cuenta.IdPedido,
                IdUsuarioSupervisor = supervisorId,
                TipoEvento = dto.PorcentajeDescuento > 10.0m ? "DescuentoAutorizado" : "DescuentoAplicado",
                MontoCancelado = montoDescuento,
                PorcentajeDescuento = dto.PorcentajeDescuento,
                Payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    IdCuenta = cuenta.Id,
                    dto.PorcentajeDescuento,
                    MontoDescuento = montoDescuento,
                    Motivo = dto.MotivoDescuento,
                    IdSupervisor = supervisorId
                }),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = supervisorId.HasValue ? $"Supervisor #{supervisorId}" : "CuentaApplication"
            };
            await _unitOfWork.EventosPedido.InsertAsync(eventoDescuento);

            return await GenerarCuentaAsync(cuenta.IdPedido);
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }
}
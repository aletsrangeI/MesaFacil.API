using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Pago;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Pagos;

public class PagoApplication : IPagoApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly PagoDTOValidator _validationRules;
    private readonly IAppLogger<PagoApplication> _logger;
    private readonly IDescuentoInventarioService? _descuentoInventarioService;
    private readonly ISupervisorPinSecurityService? _supervisorPinSecurityService;

    public PagoApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        PagoDTOValidator validationRules,
        IAppLogger<PagoApplication> logger,
        IDescuentoInventarioService? descuentoInventarioService = null,
        ISupervisorPinSecurityService? supervisorPinSecurityService = null)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
        _descuentoInventarioService = descuentoInventarioService;
        _supervisorPinSecurityService = supervisorPinSecurityService;
    }

    #region Metodos sincronos

    public Response<bool> Insert(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = _unitOfWork.Pagos.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = _unitOfWork.Pagos.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Delete(Guid id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = _unitOfWork.Pagos.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<PagoDTO> Get(Guid id)
    {
        var response = new Response<PagoDTO>();
        try
        {
            var entity = _unitOfWork.Pagos.Get(id);
            response.Data = _mapper.Map<PagoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<PagoDTO>> GetAll()
    {
        var response = new Response<IEnumerable<PagoDTO>>();
        try
        {
            var list = _unitOfWork.Pagos.GetAll();
            response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<PagoDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PagoDTO>>();
        try
        {
            var list = _unitOfWork.Pagos.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
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
            response.Data = _unitOfWork.Pagos.Count();
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

    public async Task<Response<bool>> InsertAsync(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = await _unitOfWork.Pagos.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(PagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Pago>(dto);
            response.Data = await _unitOfWork.Pagos.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago modificado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> DeleteAsync(Guid id)
    {
        var response = new Response<bool>();
        try
        {
            response.Data = await _unitOfWork.Pagos.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Pago eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<PagoDTO>> GetAsync(Guid id)
    {
        var response = new Response<PagoDTO>();
        try
        {
            var entity = await _unitOfWork.Pagos.GetAsync(id);
            response.Data = _mapper.Map<PagoDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<PagoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<PagoDTO>>();
        try
        {
            var list = await _unitOfWork.Pagos.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<PagoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<PagoDTO>>();
        try
        {
            var list = await _unitOfWork.Pagos.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<PagoDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
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
            response.Data = await _unitOfWork.Pagos.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Pago encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    #endregion

    public async Task<Response<bool>> RegistrarPagoAsync(DTO.Pago.RegistrarPagoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var cuenta = await _unitOfWork.Cuentas.GetAsync(dto.IdCuenta);
            if (cuenta == null) throw new Exception("Cuenta no encontrada");

            // Spec 028: Validación de candado de supervisor para descuentos
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

            // Aplicar descuento a la cuenta si viene indicado en el cobro
            decimal montoDescuento = dto.MontoDescuento;
            if (montoDescuento <= 0 && dto.PorcentajeDescuento > 0)
            {
                var baseTotal = cuenta.Subtotal + cuenta.ImpuestoTotal;
                montoDescuento = Math.Round(baseTotal * (dto.PorcentajeDescuento / 100m), 2);
            }

            if (montoDescuento > 0 || dto.PorcentajeDescuento > 0)
            {
                cuenta.DescuentoTotal = montoDescuento;
                cuenta.Total = Math.Max(0m, (cuenta.Subtotal + cuenta.ImpuestoTotal) - cuenta.DescuentoTotal);
                cuenta.UpdatedAt = DateTime.UtcNow;
                cuenta.UpdatedBy = supervisorId.HasValue ? $"Supervisor #{supervisorId}" : "PagoApplication";
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
                    CreatedBy = supervisorId.HasValue ? $"Supervisor #{supervisorId}" : "PagoApplication"
                };
                await _unitOfWork.EventosPedido.InsertAsync(eventoDescuento);
            }

            var pago = new Pago
            {
                IdCuenta = dto.IdCuenta,
                Monto = dto.Monto,
                Propina = dto.Propina,
                IdMetodoDePago = dto.IdMetodoDePago > 0 ? dto.IdMetodoDePago : 1,
                Referencia = dto.Referencia ?? (cuenta.Total == 0 ? "Cortesia 100%" : null),
                PagadoEn = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System",
                CreatedAt = DateTime.UtcNow,
                Moneda = "MXN"
            };

            await _unitOfWork.Pagos.InsertAsync(pago);

            // Validar si la cuenta ya se liquidÃ³
            var pagosAll = await _unitOfWork.Pagos.GetAllAsync();
            var pagosCuenta = pagosAll.Where(p => p.IdCuenta == dto.IdCuenta).ToList();
            var totalPagado = pagosCuenta.Sum(p => p.Monto) + dto.Monto;

            // Registrar evento de auditorÃ­a de pago
            var eventoPago = new EventoPedido
            {
                IdPedido = cuenta.IdPedido,
                TipoEvento = "PagoRegistrado",
                Payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    IdPago = pago.Id,
                    IdCuenta = dto.IdCuenta,
                    Monto = dto.Monto,
                    Propina = dto.Propina,
                    IdMetodoDePago = dto.IdMetodoDePago,
                    Referencia = dto.Referencia,
                    TotalPagado = totalPagado,
                    TotalCuenta = cuenta.Total
                }),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };
            await _unitOfWork.EventosPedido.InsertAsync(eventoPago);

            if (totalPagado >= cuenta.Total)
            {
                cuenta.IdEstadoCuenta = 1; // Pagada
                await _unitOfWork.Cuentas.UpdateAsync(cuenta);

                var pedido = await _unitOfWork.Pedidos.GetAsync(cuenta.IdPedido);
                if (pedido != null)
                {
                    pedido.IdEstadoPedido = 5; // Cerrado
                    await _unitOfWork.Pedidos.UpdateAsync(pedido);

                    if (pedido.IdMesa.HasValue)
                    {
                        var mesa = await _unitOfWork.Mesas.GetAsync(pedido.IdMesa.Value);
                        if (mesa != null)
                        {
                            int idPrincipal = mesa.IdMesaPrincipal ?? mesa.Id;
                            var mesasGrupo = new List<Mesa> { mesa };
                            var todasMesas = await _unitOfWork.Mesas.GetAllAsync();
                            if (todasMesas != null)
                            {
                                var relacionadas = todasMesas.Where(m => (m.IdMesaPrincipal == idPrincipal || m.Id == idPrincipal) && m.Id != mesa.Id);
                                mesasGrupo.AddRange(relacionadas);
                            }

                            foreach (var mg in mesasGrupo)
                            {
                                mg.IdEstadoMesa = EstadosMesaConst.Sucia; // 5 (Sucia / En Limpieza)
                                mg.IdMesaPrincipal = null;
                                mg.UpdatedAt = DateTime.UtcNow;
                                await _unitOfWork.Mesas.UpdateAsync(mg);
                            }
                        }
                    }

                    // Registrar evento de cierre de pedido
                    var eventoCierre = new EventoPedido
                    {
                        IdPedido = cuenta.IdPedido,
                        TipoEvento = "PedidoCerrado",
                        Payload = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            IdPedido = cuenta.IdPedido,
                            IdMesa = pedido.IdMesa,
                            TotalLiquidado = totalPagado,
                            CerradoEn = DateTime.UtcNow
                        }),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };
                    await _unitOfWork.EventosPedido.InsertAsync(eventoCierre);
                }

                // Spec 015: Descuento automÃ¡tico de inventario y escandallos al liquidar la cuenta
                if (_descuentoInventarioService != null)
                {
                    try
                    {
                        await _descuentoInventarioService.DescontarPorCuentaPagadaAsync(dto.IdCuenta);
                    }
                    catch (Exception exDescuento)
                    {
                        // En piso de venta el POS nunca se bloquea
                        _logger.LogError($"Fallo no bloqueante al descontar inventario en cuenta #{dto.IdCuenta}: {exDescuento.Message}");
                    }
                }
            }

            response.Data = true;
            response.isSuccess = true;
            response.Message = "Pago registrado exitosamente";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> RegistrarPago(DTO.Pago.RegistrarPagoDTO dto)
    {
        return RegistrarPagoAsync(dto).GetAwaiter().GetResult();
    }
}
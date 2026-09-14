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

    public PagoApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        PagoDTOValidator validationRules,
        IAppLogger<PagoApplication> logger,
        IDescuentoInventarioService? descuentoInventarioService = null)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
        _descuentoInventarioService = descuentoInventarioService;
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

            var pago = new Pago
            {
                IdCuenta = dto.IdCuenta,
                Monto = dto.Monto,
                Propina = dto.Propina,
                IdMetodoDePago = dto.IdMetodoDePago,
                Referencia = dto.Referencia,
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
                            mesa.IdEstadoMesa = 4; // Sucia
                            await _unitOfWork.Mesas.UpdateAsync(mesa);
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
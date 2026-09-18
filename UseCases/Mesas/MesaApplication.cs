using AutoMapper;
using Common;
using Domain.Entities;
using DTO.Mesa;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Mesas;

public class MesaApplication : IMesaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly MesaDTOValidator _validationRules;
    private readonly IAppLogger<MesaApplication> _logger;

    public MesaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        MesaDTOValidator validationRules,
        IAppLogger<MesaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = _unitOfWork.Mesas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = _unitOfWork.Mesas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa modificado correctamente";
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
            response.Data = _unitOfWork.Mesas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<MesaDTO> Get(int id)
    {
        var response = new Response<MesaDTO>();
        try
        {
            var entity = _unitOfWork.Mesas.Get(id);
            response.Data = _mapper.Map<MesaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<MesaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<MesaDTO>>();
        try
        {
            var list = _unitOfWork.Mesas.GetAll();
            response.Data = _mapper.Map<IEnumerable<MesaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<MesaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MesaDTO>>();
        try
        {
            var list = _unitOfWork.Mesas.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<MesaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
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
            response.Data = _unitOfWork.Mesas.Count();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> SolicitarCuenta(int idMesa)
    {
        var response = new Response<bool>();
        try
        {
            var mesa = _unitOfWork.Mesas.Get(idMesa);
            if (mesa == null)
            {
                response.isSuccess = false;
                response.Message = "Mesa no encontrada";
                return response;
            }

            mesa.IdEstadoMesa = EstadosMesaConst.PidiendoCuenta;
            mesa.UpdatedAt = DateTime.UtcNow;
            mesa.UpdatedBy = "Mesero";
            response.Data = _unitOfWork.Mesas.Update(mesa);
            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta solicitada correctamente";
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

    #region Metodos asincronos

    public async Task<Response<bool>> InsertAsync(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = await _unitOfWork.Mesas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(MesaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Mesa>(dto);
            response.Data = await _unitOfWork.Mesas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa modificado correctamente";
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
            response.Data = await _unitOfWork.Mesas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Mesa eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<MesaDTO>> GetAsync(int id)
    {
        var response = new Response<MesaDTO>();
        try
        {
            var entity = await _unitOfWork.Mesas.GetAsync(id);
            response.Data = _mapper.Map<MesaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<MesaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<MesaDTO>>();
        try
        {
            var list = await _unitOfWork.Mesas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<MesaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<MesaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<MesaDTO>>();
        try
        {
            var list = await _unitOfWork.Mesas.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<MesaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
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
            response.Data = await _unitOfWork.Mesas.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Mesa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> SolicitarCuentaAsync(int idMesa)
    {
        var response = new Response<bool>();
        try
        {
            var mesa = await _unitOfWork.Mesas.GetAsync(idMesa);
            if (mesa == null)
            {
                response.isSuccess = false;
                response.Message = "Mesa no encontrada";
                return response;
            }

            mesa.IdEstadoMesa = EstadosMesaConst.PidiendoCuenta;
            mesa.UpdatedAt = DateTime.UtcNow;
            mesa.UpdatedBy = "Mesero";
            response.Data = await _unitOfWork.Mesas.UpdateAsync(mesa);
            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Cuenta solicitada correctamente";
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
}
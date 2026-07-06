using AutoMapper;
using Common;
using Domain.Entities;
using DTO.CatCredencial;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.CatCredenciales;

public class CatCredencialApplication : ICatCredencialApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CatCredencialDTOValidator _validationRules;
    private readonly IAppLogger<CatCredencialApplication> _logger;

    public CatCredencialApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CatCredencialDTOValidator validationRules,
        IAppLogger<CatCredencialApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(CatCredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatCredencial>(dto);
            response.Data = _unitOfWork.CatCredenciales.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(CatCredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatCredencial>(dto);
            response.Data = _unitOfWork.CatCredenciales.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial modificado correctamente";
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
            response.Data = _unitOfWork.CatCredenciales.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<CatCredencialDTO> Get(int id)
    {
        var response = new Response<CatCredencialDTO>();
        try
        {
            var entity = _unitOfWork.CatCredenciales.Get(id);
            response.Data = _mapper.Map<CatCredencialDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<CatCredencialDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CatCredencialDTO>>();
        try
        {
            var list = _unitOfWork.CatCredenciales.GetAll();
            response.Data = _mapper.Map<IEnumerable<CatCredencialDTO>>(list);
            
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<CatCredencialDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CatCredencialDTO>>();
        try
        {
            var list = _unitOfWork.CatCredenciales.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CatCredencialDTO>>(list);
                response.isSuccess = true;
                response.Message = "CatCredencial encontrados";
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
            response.Data = _unitOfWork.CatCredenciales.Count();
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

    public async Task<Response<bool>> InsertAsync(CatCredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatCredencial>(dto);
            response.Data = await _unitOfWork.CatCredenciales.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CatCredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatCredencial>(dto);
            response.Data = await _unitOfWork.CatCredenciales.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial modificado correctamente";
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
            response.Data = await _unitOfWork.CatCredenciales.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<CatCredencialDTO>> GetAsync(int id)
    {
        var response = new Response<CatCredencialDTO>();
        try
        {
            var entity = await _unitOfWork.CatCredenciales.GetAsync(id);
            response.Data = _mapper.Map<CatCredencialDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<CatCredencialDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CatCredencialDTO>>();
        try
        {
            var list = await _unitOfWork.CatCredenciales.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CatCredencialDTO>>(list);
            
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "CatCredencial encontrados";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CatCredencialDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CatCredencialDTO>>();
        try
        {
            var list = await _unitOfWork.CatCredenciales.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CatCredencialDTO>>(list);
                response.isSuccess = true;
                response.Message = "CatCredencial encontrados";
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
            response.Data = await _unitOfWork.CatCredenciales.CountAsync();
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
}
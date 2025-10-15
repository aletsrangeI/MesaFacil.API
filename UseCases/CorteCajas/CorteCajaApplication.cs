using AutoMapper;
using Common;
using Domain.Entities;
using DTO.CorteCaja;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.CorteCajas;

public class CorteCajaApplication : ICorteCajaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CorteCajaDTOValidator _validationRules;
    private readonly IAppLogger<CorteCajaApplication> _logger;

    public CorteCajaApplication(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CorteCajaDTOValidator validationRules,
        IAppLogger<CorteCajaApplication> logger)
    {
        _unitOfWork = unitOfWork;
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

    #endregion
}
using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.Empresa;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Empresas;

public class EmpresaApplication : IEmpresaApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly EmpresaDTOValidator _validationRules;
    private readonly IAppLogger<EmpresaApplication> _logger;

    public EmpresaApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        EmpresaDTOValidator validationRules,
        IAppLogger<EmpresaApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(EmpresaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Empresa>(dto);
            response.Data = _unitOfWork.Empresas.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Empresa creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<bool> Update(EmpresaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Empresa>(dto);
            response.Data = _unitOfWork.Empresas.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Empresa modificado correctamente";
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
            response.Data = _unitOfWork.Empresas.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Empresa eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<EmpresaDTO> Get(int id)
    {
        var response = new Response<EmpresaDTO>();
        try
        {
            var entity = _unitOfWork.Empresas.Get(id);
            response.Data = _mapper.Map<EmpresaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Empresa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public Response<IEnumerable<EmpresaDTO>> GetAll()
    {
        var response = new Response<IEnumerable<EmpresaDTO>>();
        try
        {
            var list = _unitOfWork.Empresas.GetAll();
            response.Data = _mapper.Map<IEnumerable<EmpresaDTO>>(list);
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Empresa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public ResponsePagination<IEnumerable<EmpresaDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<EmpresaDTO>>();
        try
        {
            var list = _unitOfWork.Empresas.GetAllWithPagination(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<EmpresaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Empresa encontrado";
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
            response.Data = _unitOfWork.Empresas.Count();
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

    public async Task<Response<bool>> InsertAsync(EmpresaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Empresa>(dto);
            response.Data = await _unitOfWork.Empresas.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Empresa creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(EmpresaDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Empresa>(dto);
            response.Data = await _unitOfWork.Empresas.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Empresa modificado correctamente";
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
            response.Data = await _unitOfWork.Empresas.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Empresa eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<EmpresaDTO>> GetAsync(int id)
    {
        var response = new Response<EmpresaDTO>();
        try
        {
            var entity = await _unitOfWork.Empresas.GetAsync(id);
            response.Data = _mapper.Map<EmpresaDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Empresa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<EmpresaDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<EmpresaDTO>>();
        try
        {
            var list = await _unitOfWork.Empresas.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<EmpresaDTO>>(list);
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Empresa encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<EmpresaDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<EmpresaDTO>>();
        try
        {
            var list = await _unitOfWork.Empresas.GetAllWithPaginationAsync(page, pageSize);
            
            if (list != null)
            {
            	response.Data = _mapper.Map<IEnumerable<EmpresaDTO>>(list);
            	response.isSuccess = true;
            	response.Message = "Empresa encontrado";
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
            response.Data = await _unitOfWork.Empresas.CountAsync();
            
            if (response.Data != null)
            {
            	response.isSuccess = true;
            	response.Message = "Empresa encontrado";
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
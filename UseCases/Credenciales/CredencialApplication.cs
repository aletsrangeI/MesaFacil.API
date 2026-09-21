using Interface.Mapping;
using Common;
using Domain.Entities;
using DTO.Credencial;
using Interface.Persistence;
using Interface.UseCases;
using Validator;

namespace UseCases.Credenciales;

public class CredencialApplication : ICredencialApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppMapper _mapper;
    private readonly CredencialDTOValidator _validationRules;
    private readonly IAppLogger<CredencialApplication> _logger;

    public CredencialApplication(
        IUnitOfWork unitOfWork,
        IAppMapper mapper,
        CredencialDTOValidator validationRules,
        IAppLogger<CredencialApplication> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    #region Metodos sincronos

    public Response<bool> Insert(CredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Credencial>(dto);
            response.Data = _unitOfWork.Credenciales.Insert(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Credencial creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<bool> Update(CredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Credencial>(dto);
            response.Data = _unitOfWork.Credenciales.Update(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Credencial modificado correctamente";
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
            response.Data = _unitOfWork.Credenciales.Delete(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Credencial eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<CredencialDTO> Get(int id)
    {
        var response = new Response<CredencialDTO>();
        try
        {
            var entity = _unitOfWork.Credenciales.Get(id);
            response.Data = _mapper.Map<CredencialDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Credencial encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public Response<IEnumerable<CredencialDTO>> GetAll()
    {
        var response = new Response<IEnumerable<CredencialDTO>>();
        try
        {
            var list = _unitOfWork.Credenciales.GetAll();
            response.Data = _mapper.Map<IEnumerable<CredencialDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public ResponsePagination<IEnumerable<CredencialDTO>> GetAllWithPagination(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CredencialDTO>>();
        try
        {
            var list = _unitOfWork.Credenciales.GetAllWithPagination(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CredencialDTO>>(list);
                response.isSuccess = true;
                response.Message = "Credenciales encontradas con exito";
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
            response.Data = _unitOfWork.Credenciales.Count();
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

    public async Task<Response<bool>> InsertAsync(CredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Credencial>(dto);
            response.Data = await _unitOfWork.Credenciales.InsertAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Credencial creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<bool>> UpdateAsync(CredencialDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<Credencial>(dto);
            response.Data = await _unitOfWork.Credenciales.UpdateAsync(entity);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Credencial modificado correctamente";
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
            response.Data = await _unitOfWork.Credenciales.DeleteAsync(id);

            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "Credencial eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<CredencialDTO>> GetAsync(int id)
    {
        var response = new Response<CredencialDTO>();
        try
        {
            var entity = await _unitOfWork.Credenciales.GetAsync(id);
            response.Data = _mapper.Map<CredencialDTO>(entity);

            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "Credencial encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<Response<IEnumerable<CredencialDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<CredencialDTO>>();
        try
        {
            var list = await _unitOfWork.Credenciales.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<CredencialDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }

        return response;
    }

    public async Task<ResponsePagination<IEnumerable<CredencialDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<CredencialDTO>>();
        try
        {
            var list = await _unitOfWork.Credenciales.GetAllWithPaginationAsync(page, pageSize);

            if (list != null)
            {
                response.Data = _mapper.Map<IEnumerable<CredencialDTO>>(list);
                response.isSuccess = true;
                response.Message = "Credenciales encontradas con exito";
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
            response.Data = await _unitOfWork.Credenciales.CountAsync();
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
using AutoMapper;
using Common;
using Domain.Entities;
using DTO.TiposPedido;
using Interface.Persistence;
using Interface.UseCases.TiposPedido;
using Validator.TiposPedido;

namespace UseCases.TiposPedido;

public class TipoPedidoApplication : ITipoPedidoApplication
{
    private readonly IGenericRepository<CatTipoPedido> _repo;
    private readonly IMapper _mapper;
    private readonly TipoPedidoDTOValidator _validationRules;
    private readonly IAppLogger<TipoPedidoApplication> _logger;

    public TipoPedidoApplication(
        IGenericRepository<CatTipoPedido> repo,
        IMapper mapper,
        TipoPedidoDTOValidator validationRules,
        IAppLogger<TipoPedidoApplication> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _validationRules = validationRules;
        _logger = logger;
    }

    public async Task<Response<bool>> InsertAsync(TipoPedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatTipoPedido>(dto);
            response.Data = await _repo.InsertAsync(entity);
            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TipoPedido creado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<bool>> UpdateAsync(TipoPedidoDTO dto)
    {
        var response = new Response<bool>();
        try
        {
            var entity = _mapper.Map<CatTipoPedido>(dto);
            response.Data = await _repo.UpdateAsync(entity);
            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TipoPedido modificado correctamente";
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
            response.Data = await _repo.DeleteAsync(id);
            if (response.Data)
            {
                response.isSuccess = true;
                response.Message = "TipoPedido eliminado correctamente";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<TipoPedidoDTO>> GetAsync(int id)
    {
        var response = new Response<TipoPedidoDTO>();
        try
        {
            var entity = await _repo.GetAsync(id);
            response.Data = _mapper.Map<TipoPedidoDTO>(entity);
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "TipoPedido encontrado";
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<Response<IEnumerable<TipoPedidoDTO>>> GetAllAsync()
    {
        var response = new Response<IEnumerable<TipoPedidoDTO>>();
        try
        {
            var list = await _repo.GetAllAsync();
            response.Data = _mapper.Map<IEnumerable<TipoPedidoDTO>>(list);
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }

    public async Task<ResponsePagination<IEnumerable<TipoPedidoDTO>>> GetAllWithPaginationAsync(int page, int pageSize)
    {
        var response = new ResponsePagination<IEnumerable<TipoPedidoDTO>>();
        try
        {
            var list = await _repo.GetAllWithPaginationAsync(page, pageSize);
            response.Data = _mapper.Map<IEnumerable<TipoPedidoDTO>>(list);
            if (response.Data != null)
            {
                response.isSuccess = true;
                response.Message = "TiposPedido encontrados";
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
            response.Data = await _repo.CountAsync();
            response.isSuccess = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            _logger.LogError(ex.Message);
        }
        return response;
    }
}
